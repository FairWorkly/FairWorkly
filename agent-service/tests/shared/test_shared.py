import io
import json
import sys
from pathlib import Path
from types import SimpleNamespace

import pytest
from fastapi import HTTPException, UploadFile
from starlette.datastructures import Headers
from langchain_core.documents import Document

sys.path.append(str(Path(__file__).resolve().parents[2]))

import shared.document_manager as document_manager_module
import shared.document_processor as document_processor_module
import shared.query_handler as query_handler_module
from shared.document_manager import DocumentManager
from shared.document_processor import DocumentProcessor
from shared.query_handler import QueryHandler

ASSETS_PDF = Path(__file__).resolve().parents[2] / "assets" / "retail_award.pdf"


def _upload_file(name: str, content: bytes, content_type: str = "application/pdf") -> UploadFile:
    headers = Headers({"content-type": content_type})
    return UploadFile(filename=name, file=io.BytesIO(content), headers=headers)


class DummyStore:
    def __init__(self):
        self.added = []
        self.saved = None

    def add_documents(self, docs):
        self.added.extend(docs)

    def save_local(self, path):
        self.saved = path


def test_document_manager_uploads_new_file(tmp_path, monkeypatch):
    paths = {
        "ingested_docs_file": str(tmp_path / "ingested.json"),
        "document_faiss_path": str(tmp_path / "doc_store"),
    }
    config = {"paths": paths, "faiss": {}}
    monkeypatch.setattr(document_manager_module, "CONFIG", config, raising=False)

    processor = SimpleNamespace(
        paths=paths,
        document_vector_store=DummyStore(),
        process_document=lambda file: ["chunk"],
    )

    manager = DocumentManager(processor)
    upload = _upload_file("policy.pdf", b"dummy")
    resp = manager.upload_document(upload)

    assert resp["message"].startswith("Document 'policy.pdf' uploaded")
    assert processor.document_vector_store.added == ["chunk"]
    assert processor.document_vector_store.saved == paths["document_faiss_path"]
    assert json.loads(Path(paths["ingested_docs_file"]).read_text()) == ["policy.pdf"]


def test_document_manager_skips_duplicate(tmp_path, monkeypatch):
    paths = {
        "ingested_docs_file": str(tmp_path / "ingested.json"),
        "document_faiss_path": str(tmp_path / "doc_store"),
    }
    config = {"paths": paths, "faiss": {}}
    Path(paths["ingested_docs_file"]).write_text(json.dumps(["policy.pdf"]))
    monkeypatch.setattr(document_manager_module, "CONFIG", config, raising=False)

    processor = SimpleNamespace(
        paths=paths,
        document_vector_store=DummyStore(),
        process_document=lambda file: ["chunk"],
    )

    manager = DocumentManager(processor)
    upload = _upload_file("policy.pdf", b"dummy")
    resp = manager.upload_document(upload)

    assert resp["message"] == "Document 'policy.pdf' has already been ingested."
    assert processor.document_vector_store.added == []


def test_query_handler_returns_response(monkeypatch):
    docs = [Document(page_content="Content A", metadata={"source": "x"})]

    class Store:
        def similarity_search(self, query, k):
            return docs

    llm = SimpleNamespace(invoke=lambda prompt: "Answer from local")
    processor = SimpleNamespace(
        document_vector_store=Store(),
        prompt_template="Docs: {documents_text}",
        ensure_chat_model=lambda: llm,
    )

    config = {
        "faiss": {"similarity_search_k_docs": 1},
        "model_params": {"deployment_mode_llm": "local"},
    }
    monkeypatch.setattr(query_handler_module, "CONFIG", config, raising=False)

    handler = QueryHandler(processor)
    resp = handler.submit_query("What is policy?")

    assert resp["response"] == "Answer from local"
    assert resp["sources"][0]["content"] == "Content A"


def test_query_handler_raises_on_failure(monkeypatch):
    class BrokenStore:
        def similarity_search(self, query, k):
            raise RuntimeError("boom")

    processor = SimpleNamespace(
        document_vector_store=BrokenStore(),
        prompt_template="Docs: {documents_text}",
        ensure_chat_model=lambda: SimpleNamespace(invoke=lambda prompt: "unused"),
    )
    config = {
        "faiss": {"similarity_search_k_docs": 1},
        "model_params": {"deployment_mode_llm": "local"},
    }
    monkeypatch.setattr(query_handler_module, "CONFIG", config, raising=False)

    handler = QueryHandler(processor)
    with pytest.raises(HTTPException) as excinfo:
        handler.submit_query("fail")

    assert excinfo.value.status_code == 500


def test_document_processor_processes_assets_pdf(tmp_path, monkeypatch):
    prompt_file = tmp_path / "prompt.yaml"
    prompt_file.write_text("prompt_template: 'Docs: {documents_text}'")

    config = {
        "model_params": {
            "deployment_mode_embedding": "local",
            "deployment_mode_llm": "local",
            "local_embedding_model": "stub-model",
            "local_model_task_type": "text-generation",
            "local_model_id": "stub-llm",
            "local_max_tokens": 32,
            "local_top_k": 5,
            "local_top_p": 0.9,
            "local_repetition_penalty": 1.0,
            "hf_model_repo_id": "stub",
            "online_model_name": "stub",
            "openai_api_base": "https://example.com",
            "temperature": 0.1,
        },
        "paths": {
            "document_faiss_path": str(tmp_path / "doc_store"),
            "ingested_docs_file": str(tmp_path / "ingested.json"),
            "log_file": str(tmp_path / "app.log"),
            "prompt_file": str(prompt_file),
        },
        "faiss": {"similarity_search_k_docs": 1},
        "text_splitter": {"chunk_size": 256, "chunk_overlap": 32},
    }

    monkeypatch.setattr(document_processor_module, "CONFIG", config, raising=False)
    monkeypatch.setattr(document_processor_module, "get_embedding_model", lambda mode: SimpleNamespace(), raising=False)
    monkeypatch.setattr(document_processor_module, "get_language_model", lambda mode: SimpleNamespace(), raising=False)
    monkeypatch.setattr(document_processor_module, "initialize_vector_store", lambda *args, **kwargs: DummyStore(), raising=False)

    processor = DocumentProcessor(prompt_file=str(prompt_file))
    upload = _upload_file("retail_award.pdf", ASSETS_PDF.read_bytes())

    chunks = processor.process_document(upload)
    assert chunks
    assert chunks[0].metadata["source"] == "retail_award.pdf"

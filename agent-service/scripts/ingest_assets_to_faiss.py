#!/usr/bin/env python3
"""Offline ingestion script to build FAISS index from assets PDFs."""

from __future__ import annotations

import logging
from pathlib import Path
from typing import Dict, List

from langchain_community.vectorstores import FAISS
from langchain_core.documents import Document
from langchain_text_splitters import RecursiveCharacterTextSplitter
from pypdf import PdfReader

from agents.shared.llm.embeddings_factory import create_embeddings
from master_agent.config import load_config, resolve_document_faiss_path


LOGGER_NAME = "assets_ingestion"


def project_root() -> Path:
    return Path(__file__).resolve().parent.parent


def discover_assets_dir(root: Path) -> Path:
    candidates = [root / "assets", root / "agents" / "shared" / "assets"]
    for candidate in candidates:
        if candidate.exists():
            return candidate
    raise FileNotFoundError(
        "Could not find assets directory. Expected 'assets/' or 'agents/shared/assets/'."
    )


def extract_documents(pdf_path: Path) -> List[Document]:
    reader = PdfReader(str(pdf_path))
    documents: List[Document] = []

    for idx, page in enumerate(reader.pages, start=1):
        text = (page.extract_text() or "").strip()
        if not text:
            continue
        documents.append(
            Document(
                page_content=text,
                metadata={"source": pdf_path.name, "page": idx},
            )
        )

    return documents


def chunk_documents(documents: List[Document], config: Dict[str, Dict]) -> List[Document]:
    splitter_config = config.get("text_splitter", {})
    chunk_size = splitter_config.get("chunk_size", 1000)
    chunk_overlap = splitter_config.get("chunk_overlap", 200)
    splitter = RecursiveCharacterTextSplitter(
        chunk_size=chunk_size,
        chunk_overlap=chunk_overlap,
    )
    return splitter.split_documents(documents)


def log_chunk_stats(chunks: List[Document], logger: logging.Logger) -> None:
    per_file: Dict[str, int] = {}
    for chunk in chunks:
        source = chunk.metadata.get("source", "unknown")
        per_file[source] = per_file.get(source, 0) + 1

    for source, count in sorted(per_file.items()):
        logger.info("File %s -> %d chunks", source, count)

    logger.info("Total chunks: %d", len(chunks))


def save_faiss(chunks: List[Document], config: Dict, logger: logging.Logger) -> None:
    embeddings = create_embeddings(config, logger=logger)
    vectorstore = FAISS.from_documents(chunks, embeddings)
    embedding_mode = config.get("model_params", {}).get("deployment_mode_embedding")
    relative_path = resolve_document_faiss_path(config, embedding_mode)
    output_path = project_root() / relative_path
    output_path.mkdir(parents=True, exist_ok=True)
    vectorstore.save_local(str(output_path))
    logger.info("Saved FAISS index to %s", output_path)


def ingest() -> None:
    logger = logging.getLogger(LOGGER_NAME)
    config = load_config()

    assets_dir = discover_assets_dir(project_root())
    logger.info("Loading PDFs from %s", assets_dir)

    documents: List[Document] = []
    for pdf_file in sorted(assets_dir.glob("*.pdf")):
        logger.info("Parsing %s", pdf_file.name)
        documents.extend(extract_documents(pdf_file))

    if not documents:
        raise RuntimeError("No PDF documents found for ingestion.")

    chunks = chunk_documents(documents, config)
    log_chunk_stats(chunks, logger)
    save_faiss(chunks, config, logger)


def main() -> None:
    logging.basicConfig(level=logging.INFO, format="%(levelname)s:%(name)s:%(message)s")
    try:
        ingest()
    except Exception:
        logging.getLogger(LOGGER_NAME).exception("Failed to ingest assets into FAISS")
        raise


if __name__ == "__main__":
    main()

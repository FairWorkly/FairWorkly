from typing import Dict, Any
import logging
import threading

from master_agent.feature_registry import FeatureBase
from master_agent.config import load_config, CONFIG_PATH, resolve_document_faiss_path
from agents.shared.llm.factory import LLMProvider
from agents.shared.llm.embeddings_factory import create_embeddings
from agents.shared.vector_db import load_faiss
from agents.shared.rag_retriever import RAGRetriever, RetrievalResult


_RESOURCE_LOCK = threading.Lock()
_EMBEDDINGS = None
_VECTORSTORE = None
_RETRIEVER = None


def _ensure_retriever(config, logger):
    """Ensure embeddings/vector store/retriever exist exactly once per process."""
    global _EMBEDDINGS, _VECTORSTORE, _RETRIEVER

    if _RETRIEVER is not None and _VECTORSTORE is not None:
        return _RETRIEVER, _VECTORSTORE

    with _RESOURCE_LOCK:
        if _RETRIEVER is not None and _VECTORSTORE is not None:
            return _RETRIEVER, _VECTORSTORE

        if _EMBEDDINGS is None:
            _EMBEDDINGS = create_embeddings(config, logger=logger)

        if _VECTORSTORE is None:
            store_relative = resolve_document_faiss_path(config)
            store_absolute = (CONFIG_PATH.parent / store_relative).resolve()
            _VECTORSTORE = load_faiss(str(store_absolute), _EMBEDDINGS, logger=logger)

        _RETRIEVER = RAGRetriever(_VECTORSTORE, logger=logger)
        return _RETRIEVER, _VECTORSTORE


class ComplianceFeature(FeatureBase):
    """Compliance Feature - Fair Work compliance checks."""

    def __init__(self):
        self.logger = logging.getLogger(__name__)
        self.config = load_config()
        self.system_prompt = self.config.get("prompt", {}).get(
            "system_prompt",
            "You are FairWorkly's compliance co-pilot. Answer using Fair Work context.",
        )
        self.retriever = None
        self.vectorstore = None

        try:
            self.llm = LLMProvider()
        except Exception as exc:
            self.logger.warning("LLM provider unavailable: %s", exc)
            self.llm = None

        try:
            self.retriever, self.vectorstore = _ensure_retriever(self.config, self.logger)
        except FileNotFoundError as exc:
            self.logger.warning("Vector store unavailable: %s", exc, exc_info=True)
        except Exception as exc:
            self.logger.error("Failed to initialize retriever: %s", exc, exc_info=True)
            self.retriever = None

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        """Process Compliance requests."""
        message = (payload.get("message") or "").strip()
        file_name = payload.get("file_name")

        if not message:
            return {
                "type": "compliance",
                "message": "Please provide a question for compliance assistance.",
                "file_name": file_name,
                "note": "MESSAGE_REQUIRED",
            }

        if not self.llm:
            return {
                "type": "compliance",
                "message": f"Compliance Feature placeholder - Received message: {message}",
                "file_name": file_name,
                "note": "LLM provider not configured",
            }

        retrieval: RetrievalResult | None = None
        docs_text = "Retriever unavailable - please run the ingestion script."
        metadata_sources: list[Dict[str, Any]] = []
        top_k = self.config.get("faiss", {}).get("similarity_search_k_docs", 3)

        if not self.retriever:
            try:
                self.retriever, self.vectorstore = _ensure_retriever(self.config, self.logger)
            except FileNotFoundError as exc:
                self.logger.warning("Vector store unavailable: %s", exc, exc_info=True)
            except Exception as exc:
                self.logger.error("Failed to initialize retriever: %s", exc, exc_info=True)

        if self.retriever:
            try:
                retrieval = await self.retriever.retrieve(message, top_k=top_k)
                metadata_sources = retrieval.metadatas
                docs_text = self.retriever.format_context_for_llm(retrieval)
            except Exception as exc:
                self.logger.error("Document retrieval failed: %s", exc, exc_info=True)
                docs_text = "Document retrieval failed. Proceeding without context."

        rag_system_prompt = f"{self.system_prompt}\n\nRelevant Documents:\n{docs_text}"

        messages = [
            {"role": "system", "content": rag_system_prompt},
            {"role": "user", "content": message},
        ]

        try:
            llm_response = await self.llm.generate(messages, temperature=0.3, max_tokens=800)
            answer = llm_response.get("content", "").strip()
        except Exception as exc:
            self.logger.error("LLM call failed: %s", exc, exc_info=True)
            return {
                "type": "compliance",
                "message": f"Compliance Feature placeholder - Received message: {message}",
                "file_name": file_name,
                "note": "LLM invocation failed",
            }

        return {
            "type": "compliance",
            "message": answer or "No answer generated.",
            "file_name": file_name,
            "model": llm_response.get("model"),
            "sources": metadata_sources,
        }

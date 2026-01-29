"""RAG retriever manager with process-wide caching."""

import threading
from typing import Any

from master_agent.config import CONFIG_PATH, resolve_document_faiss_path
from agents.shared.llm.embeddings_factory import create_embeddings
from agents.shared.vector_db import load_faiss
from agents.shared.rag_retriever import RAGRetriever

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
        logger.info("RAG resources initialized")
        return _RETRIEVER, _VECTORSTORE


# Public alias to minimize call site churn.
ensure_retriever = _ensure_retriever

"""High-level retrieval helpers for RAG workflows."""

from __future__ import annotations

import logging
from typing import List, Dict, Any, Optional

from langchain_community.vectorstores import FAISS


class RetrievalResult:
    """Container for retrieved documents and metadata."""

    def __init__(
        self,
        documents: List[str],
        scores: List[float],
        metadatas: List[Dict[str, Any]],
    ):
        self.documents = documents
        self.scores = scores
        self.metadatas = metadatas


class RAGRetriever:
    """Simple retriever that queries a FAISS vector store."""

    def __init__(
        self,
        vectorstore: FAISS,
        *,
        logger: Optional[logging.Logger] = None,
    ):
        if vectorstore is None:
            raise ValueError("vectorstore must be provided")

        self.vectorstore = vectorstore
        self.logger = logger or logging.getLogger(__name__)

    async def retrieve(
        self,
        query: str,
        top_k: int = 5,
        filter: Optional[Dict[str, Any]] = None,
    ) -> RetrievalResult:
        """Retrieve the top_k most similar chunks for query."""
        if not query:
            return RetrievalResult(documents=[], scores=[], metadatas=[])

        if filter:
            self.logger.debug("FAISS filter ignored (not supported): %s", filter)

        docs_with_scores = self.vectorstore.similarity_search_with_score(query, k=top_k)

        documents, scores, metadatas = [], [], []
        for doc, score in docs_with_scores:
            documents.append(doc.page_content)
            scores.append(score)
            metadatas.append(doc.metadata or {})

        return RetrievalResult(documents=documents, scores=scores, metadatas=metadatas)

    async def retrieve_with_threshold(
        self,
        query: str,
        similarity_threshold: float = 0.7,
        max_results: int = 10,
    ) -> RetrievalResult:
        """Retrieve documents whose similarity score exceeds the threshold."""
        base_results = await self.retrieve(query, top_k=max_results)
        docs, scores, metadatas = [], [], []

        for doc, score, metadata in zip(
            base_results.documents, base_results.scores, base_results.metadatas
        ):
            if score >= similarity_threshold:
                docs.append(doc)
                scores.append(score)
                metadatas.append(metadata)

        return RetrievalResult(documents=docs, scores=scores, metadatas=metadatas)

    def format_context_for_llm(self, result: RetrievalResult) -> str:
        """Format retrieved documents for injection into an LLM prompt."""
        if not result.documents:
            return "No relevant documents found."

        formatted = "Based on these Fair Work documents:\n\n"
        for i, (doc, metadata) in enumerate(zip(result.documents, result.metadatas), 1):
            source = metadata.get("source", "Unknown source")
            page = metadata.get("page")
            formatted += f"[{i}] {source}"
            if page is not None:
                formatted += f" (page {page})"
            formatted += f": {doc}\n\n"

        return formatted

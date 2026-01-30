"""Shared RAG execution entrypoint."""

from typing import Dict, Any

from shared.llm.factory import LLMProvider
from shared.rag_retriever import RetrievalResult
from shared.rag.retriever_manager import ensure_retriever


async def run(
    message: str,
    *,
    system_prompt: str,
    config: Dict[str, Any],
    logger,
) -> Dict[str, Any]:
    """Run retrieval + LLM generation with a shared prompt template."""
    try:
        llm = LLMProvider()
    except Exception as exc:
        logger.warning("LLM provider unavailable: %s", exc)
        return {
            "content": f"Compliance Feature placeholder - Received message: {message}",
            "note": "LLM provider not configured",
        }

    retriever = None
    vectorstore = None
    try:
        retriever, vectorstore = ensure_retriever(config, logger)
    except FileNotFoundError as exc:
        logger.warning("Vector store unavailable: %s", exc, exc_info=True)
    except Exception as exc:
        logger.error("Failed to initialize retriever: %s", exc, exc_info=True)

    retrieval: RetrievalResult | None = None
    docs_text = "Retriever unavailable - please run the ingestion script."
    metadata_sources: list[Dict[str, Any]] = []
    top_k = config.get("faiss", {}).get("similarity_search_k_docs", 3)

    if not retriever:
        try:
            retriever, vectorstore = ensure_retriever(config, logger)
        except FileNotFoundError as exc:
            logger.warning("Vector store unavailable: %s", exc, exc_info=True)
        except Exception as exc:
            logger.error("Failed to initialize retriever: %s", exc, exc_info=True)

    if retriever:
        try:
            retrieval = await retriever.retrieve(message, top_k=top_k)
            metadata_sources = retrieval.metadatas
            docs_text = retriever.format_context_for_llm(retrieval)
        except Exception as exc:
            logger.error("Document retrieval failed: %s", exc, exc_info=True)
            docs_text = "Document retrieval failed. Proceeding without context."

    rag_system_prompt = f"{system_prompt}\n\nRelevant Documents:\n{docs_text}"

    messages = [
        {"role": "system", "content": rag_system_prompt},
        {"role": "user", "content": message},
    ]

    try:
        llm_response = await llm.generate(messages, temperature=0.3, max_tokens=800)
        answer = llm_response.get("content", "").strip()
    except Exception as exc:
        logger.error("LLM call failed: %s", exc, exc_info=True)
        return {
            "content": f"Compliance Feature placeholder - Received message: {message}",
            "note": "LLM invocation failed",
        }

    return {
        "content": answer or "No answer generated.",
        "model": llm_response.get("model"),
        "sources": metadata_sources,
    }

"""Shared RAG execution entrypoint."""

from typing import Any, Dict

from shared.llm.factory import LLMProvider
from shared.llm.history_utils import normalize_chat_history
from shared.rag.retriever_manager import ensure_retriever


def _build_fallback_response(reason: str, note: str) -> Dict[str, Any]:
    return {
        "content": (
            "I cannot provide a compliance answer right now.\n"
            f"Reason: {reason}\n"
            "Next steps:\n"
            "1) Retry in a few minutes.\n"
            "2) Check AI service configuration (API key, model, and vector store).\n"
            "3) Contact support if the issue persists."
        ),
        "note": note,
    }


async def run(
    message: str,
    *,
    system_prompt: str,
    config: Dict[str, Any],
    logger,
    history_messages: Any = None,
) -> Dict[str, Any]:
    """Run retrieval + LLM generation with a shared prompt template."""
    try:
        llm = LLMProvider()
    except Exception as exc:
        logger.warning("LLM provider unavailable: %s", exc)
        return _build_fallback_response(
            reason="AI provider is not configured.",
            note="LLM provider not configured",
        )

    retriever = None
    try:
        retriever, _ = ensure_retriever(config, logger)
    except FileNotFoundError as exc:
        logger.warning("Vector store unavailable: %s", exc, exc_info=True)
    except Exception as exc:
        logger.error("Failed to initialize retriever: %s", exc, exc_info=True)

    metadata_sources: list[Dict[str, Any]] = []
    docs_text: str | None = None
    top_k = config.get("faiss", {}).get("similarity_search_k_docs", 3)

    if retriever:
        try:
            retrieval = await retriever.retrieve(message, top_k=top_k)
            metadata_sources = retrieval.metadatas
            docs_text = retriever.format_context_for_llm(retrieval)
        except Exception as exc:
            logger.error("Document retrieval failed: %s", exc, exc_info=True)

    if docs_text:
        rag_system_prompt = f"{system_prompt}\n\nRelevant Documents:\n{docs_text}"
    else:
        rag_system_prompt = system_prompt

    messages = [{"role": "system", "content": rag_system_prompt}]
    messages.extend(normalize_chat_history(history_messages))
    messages.append({"role": "user", "content": message})

    try:
        llm_response = await llm.generate(messages, temperature=0.3, max_tokens=800)
        answer = llm_response.get("content", "").strip()
    except Exception as exc:
        logger.error("LLM call failed: %s", exc, exc_info=True)
        return _build_fallback_response(
            reason="AI provider request failed.",
            note="LLM invocation failed",
        )

    return {
        "content": answer or "No answer generated.",
        "model": llm_response.get("model"),
        "sources": metadata_sources,
    }

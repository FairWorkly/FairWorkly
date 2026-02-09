"""Factory for creating embedding models used by the agents."""

from __future__ import annotations

import logging
from typing import Optional, Dict, Any

from langchain_core.embeddings import Embeddings
from langchain_huggingface import HuggingFaceEmbeddings
from langchain_openai import OpenAIEmbeddings


def create_embeddings(
    config: Dict[str, Any],
    logger: Optional[logging.Logger] = None,
) -> Embeddings:
    """Return embeddings instance based on deployment mode."""
    logger = logger or logging.getLogger(__name__)
    model_params = config.get("model_params", {})
    mode = model_params.get("deployment_mode_embedding", "local").lower()

    if mode == "local":
        model_name = model_params.get("local_embedding_model")
        if not model_name:
            raise ValueError("local_embedding_model must be set for local embeddings")
        logger.debug("Creating local embeddings using %s", model_name)
        return HuggingFaceEmbeddings(model_name=model_name)

    if mode == "online":
        logger.debug("Creating OpenAI embeddings using environment credentials")
        return OpenAIEmbeddings()

    raise ValueError(f"Unsupported deployment_mode_embedding: {mode}")

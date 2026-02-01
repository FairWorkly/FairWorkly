"""Utilities for loading FAISS vector stores."""

from __future__ import annotations

import logging
from pathlib import Path
from typing import Optional

from langchain_community.vectorstores import FAISS
from langchain_core.embeddings import Embeddings


def load_faiss(
    path: str,
    embeddings: Embeddings,
    logger: Optional[logging.Logger] = None,
) -> FAISS:
    """Load a FAISS vector store from disk."""
    logger = logger or logging.getLogger(__name__)
    store_path = Path(path)

    if not store_path.exists():
        raise FileNotFoundError(
            f"FAISS store not found at '{store_path}'. "
            "Please run scripts/ingest_assets_to_faiss.py first."
        )

    logger.debug("Loading FAISS index from %s", store_path)
    return FAISS.load_local(
        str(store_path),
        embeddings,
        allow_dangerous_deserialization=True,
    )

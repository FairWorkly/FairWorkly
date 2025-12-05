import os
from fastapi import HTTPException
from langchain_community.vectorstores import FAISS

from shared.logging import logger


def initialize_vector_store(path: str, index_name: str, embeddings) -> FAISS:
    logger.debug("Initializing %s at path %s", index_name, path)
    try:
        if os.path.exists(path):
            logger.info("Loading existing %s from %s", index_name, path)
            store = FAISS.load_local(path, embeddings, allow_dangerous_deserialization=True)
            logger.debug("Loaded %s successfully", index_name)
        else:
            logger.info("Creating new %s at %s", index_name, path)
            store = FAISS.from_texts([""], embeddings)
            store.save_local(path)
            logger.debug("Created and saved new %s", index_name)
        return store
    except Exception as exc:
        logger.error("Error initializing %s: %s", index_name, exc, exc_info=True)
        raise HTTPException(status_code=500, detail=f"Error initializing {index_name}: {exc}") from exc

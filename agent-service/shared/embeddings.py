from langchain_community.embeddings.openai import OpenAIEmbeddings
from langchain_huggingface import HuggingFaceEmbeddings

from shared.config import CONFIG
from shared.logging import logger


def get_embedding_model(deployment_mode: str):
    if deployment_mode == "online":
        return OpenAIEmbeddings()
    if deployment_mode == "local":
        model_name = CONFIG["model_params"]["local_embedding_model"]
        logger.debug("Loading local embedding model %s", model_name)
        return HuggingFaceEmbeddings(model_name=model_name)
    raise ValueError(f"Unsupported embedding deployment mode: {deployment_mode}")

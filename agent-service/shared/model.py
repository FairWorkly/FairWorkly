import os
from functools import lru_cache
from pathlib import Path

from dotenv import load_dotenv
from huggingface_hub import InferenceClient
from langchain_core.messages import HumanMessage, SystemMessage
from langchain_huggingface import HuggingFacePipeline
from langchain_openai import ChatOpenAI
from transformers import AutoModelForCausalLM, AutoTokenizer, pipeline

from shared.config import CONFIG
from shared.errors import LLMResponseError

load_dotenv()

DEFAULT_MODEL = "gpt-4o-mini"
DEFAULT_TEMPERATURE = 0.0


@lru_cache(maxsize=None)
def _cached_openai_client(model: str, temperature: float) -> ChatOpenAI:
    return ChatOpenAI(model=model, temperature=temperature)


def get_language_model(deployment_mode: str):
    params = CONFIG["model_params"]
    if deployment_mode == "online":
        model = params.get("open_ai_model", DEFAULT_MODEL)
        temperature = float(params.get("temperature", DEFAULT_TEMPERATURE))
        return _cached_openai_client(model, temperature)
    if deployment_mode == "huggingface":
        model_id = params["hf_model_repo_id"]
        return {
            "client": InferenceClient(model=model_id, token=os.environ["huggingfacehub_api_token"]),
            "model_id": model_id,
            "config": {
                "max_new_tokens": params["local_max_tokens"],
                "top_k": params["local_top_k"],
                "top_p": params["local_top_p"],
                "temperature": params["temperature"],
                "repetition_penalty": params["local_repetition_penalty"],
            },
        }
    if deployment_mode == "local":
        model_id = params["local_model_id"]
        tokenizer = AutoTokenizer.from_pretrained(model_id)
        model = AutoModelForCausalLM.from_pretrained(model_id)
        pipe = pipeline(
            params["local_model_task_type"],
            model=model,
            tokenizer=tokenizer,
            max_new_tokens=params["local_max_tokens"],
            top_k=params["local_top_k"],
            top_p=params["local_top_p"],
            temperature=params["temperature"],
            repetition_penalty=params["local_repetition_penalty"],
            do_sample=True,
        )
        return HuggingFacePipeline(pipeline=pipe)
    raise ValueError(f"Unsupported deployment mode: {deployment_mode}")


def load_prompt(prompt_path: Path | str, fallback: str) -> str:
    path = Path(prompt_path)
    try:
        return path.read_text(encoding="utf-8").strip()
    except FileNotFoundError:
        return fallback


def generate_reply(
    system_prompt: str,
    message: str,
    response_format: dict | None = None,
) -> str:
    messages = [
        SystemMessage(content=system_prompt),
        HumanMessage(content=message),
    ]
    kwargs = {}
    if response_format:
        kwargs["response_format"] = response_format
    try:
        deployment_mode = CONFIG["model_params"]["deployment_mode_llm"]
        client = get_language_model(deployment_mode)
        response = client.invoke(messages, **kwargs)
        return response.content
    except Exception as exc:
        raise LLMResponseError("LLM invocation failed") from exc

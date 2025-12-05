import io
from pathlib import Path

from fastapi import UploadFile
from starlette.datastructures import Headers

from shared.document_manager import DocumentManager
from shared.document_processor import DocumentProcessor
from shared.logging import logger

PROMPT_PATH = Path(__file__).resolve().parent / "prompt.yaml"
ASSETS_DIR = Path(__file__).resolve().parents[2] / "assets"
DEFAULT_ASSETS = [
    ("retail_award.pdf", ASSETS_DIR / "retail_award.pdf", "application/pdf"),
]

_ingested_defaults = False


def _make_upload_file(path: Path, filename: str, content_type: str) -> UploadFile:
    data = path.read_bytes()
    headers = Headers({"content-type": content_type})
    return UploadFile(filename=filename, file=io.BytesIO(data), headers=headers)


def ensure_default_documents_ingested() -> None:
    global _ingested_defaults
    if _ingested_defaults:
        return

    processor = DocumentProcessor(prompt_file=str(PROMPT_PATH))
    manager = DocumentManager(processor)
    ingested = set(manager.load_ingested_docs())

    for filename, path, content_type in DEFAULT_ASSETS:
        if filename in ingested:
            continue
        if not path.exists():
            logger.warning("Default compliance asset %s missing at %s", filename, path)
            continue
        upload = _make_upload_file(path, filename, content_type)
        try:
            manager.upload_document(upload)
        except Exception:
            logger.exception("Failed to ingest default asset %s", filename)
            continue
    _ingested_defaults = True

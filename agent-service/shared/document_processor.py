import io
import yaml
from shared.config import CONFIG
from shared.embeddings import get_embedding_model
from shared.logging import logger
from shared.model import get_language_model
from shared.vector_store import initialize_vector_store
from pypdf import PdfReader
import docx2txt
from langchain_core.documents import Document
from langchain_text_splitters import RecursiveCharacterTextSplitter
from fastapi import UploadFile
from typing import List

class DocumentProcessor:
    def __init__(self, prompt_file: str | None = None, load_chat_model: bool = False):
        logger.debug("DocumentProcessor initialized")
        params = CONFIG["model_params"]
        self.embeddings = get_embedding_model(params["deployment_mode_embedding"])
        self._model_params = params
        self.chat_model = get_language_model(params["deployment_mode_llm"]) if load_chat_model else None
        self.paths = CONFIG["paths"]
        self.faiss_config = CONFIG["faiss"]
        self.text_splitter_config = CONFIG["text_splitter"]
        logger.debug(f"Paths: {self.paths}, FAISS Config: {self.faiss_config}, Text Splitter Config: {self.text_splitter_config}")

        # Initialize document vector store
        self.document_vector_store = initialize_vector_store(
            self.paths["document_faiss_path"],
            "document FAISS index",
            self.embeddings,
        )
        prompt_path = prompt_file or self.paths.get("prompt_file")
        if not prompt_path:
            raise ValueError("prompt_file must be provided via argument or config['paths']['prompt_file']")
        self.prompt_path = prompt_path
        logger.debug(f"Loading prompt template from {self.prompt_path}")
        try:
            with open(self.prompt_path, "r", encoding="utf-8") as f:
                data = yaml.safe_load(f) or {}
                self.prompt_template = data.get("prompt_template", "")
            logger.debug(f"Prompt template loaded: {self.prompt_template[:100]}...")
        except Exception as e:
            logger.error(f"Error loading prompt template: {str(e)}", exc_info=True)
            raise
    
    def ensure_chat_model(self):
        if self.chat_model is None:
            self.chat_model = get_language_model(self._model_params["deployment_mode_llm"])
        return self.chat_model
    
    def preprocess_text(self,text: str) -> str:
        """Preprocess text by removing newlines and extra spaces."""
        logger.debug("Preprocessing text")
        return text.replace("\n", " ").replace("  ", " ")

    def process_document(self, file: UploadFile) -> List[Document]:
        """Process uploaded document and return split chunks."""
        logger.debug(f"Processing document: {file.filename}, Content-Type: {file.content_type}")
        try:
            documents = []
            if file.content_type == "application/pdf":
                logger.debug("Processing PDF document")
                reader = PdfReader(io.BytesIO(file.file.read()))
                for page_num, page in enumerate(reader.pages):
                    text = page.extract_text()
                    text = self.preprocess_text(text)
                    if text:
                        documents.append(Document(page_content=text, metadata={"source": file.filename, "page": page_num}))
                        logger.debug(f"Extracted text from page {page_num}: {text[:50]}...")
            elif file.content_type == "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                logger.debug("Processing DOCX document")
                text = docx2txt.process(io.BytesIO(file.file.read()))
                text = self.preprocess_text(text)
                if text:
                    documents = [Document(page_content=text, metadata={"source": file.filename})]
                    logger.debug(f"Extracted text: {text[:50]}...")
            else:
                logger.warning(f"Unsupported file type: {file.content_type}")
                raise ValueError("Unsupported file type")

            # Split into chunks
            logger.debug("Splitting document into chunks")
            text_splitter = RecursiveCharacterTextSplitter(
                chunk_size=self.text_splitter_config["chunk_size"],
                chunk_overlap=self.text_splitter_config["chunk_overlap"]
            )
            chunks = text_splitter.split_documents(documents)
            logger.debug(f"Document split into {len(chunks)} chunks")
            return chunks
        except Exception as e:
            logger.error(f"Error processing document {file.filename}: {str(e)}", exc_info=True)
            raise

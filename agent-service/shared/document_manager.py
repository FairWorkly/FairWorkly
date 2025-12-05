import json
import os
from typing import Dict, List

from fastapi import HTTPException, UploadFile
from shared.config import CONFIG
from shared.document_processor import DocumentProcessor
from shared.logging import logger


class DocumentManager:
    """Class to manage document ingestion and retrieval."""
    
    def __init__(self, processor: DocumentProcessor):
        logger.debug("Initializing DocumentManager")
        self.processor = processor
        self.paths = CONFIG["paths"]
        self.faiss_config = CONFIG["faiss"]

    def load_ingested_docs(self) -> List[str]:
        """Load the list of ingested documents from JSON file."""
        logger.debug(f"Loading ingested documents from {self.paths['ingested_docs_file']}")
        try:
            if os.path.exists(self.paths["ingested_docs_file"]):
                with open(self.paths["ingested_docs_file"], "r", encoding="utf-8") as f:
                    ingested_docs = json.load(f)
                logger.debug(f"Loaded ingested documents: {ingested_docs}")
                return ingested_docs
            logger.info("No ingested_documents.json found; starting with an empty list.")
            return []
        except Exception as e:
            logger.error(f"Error loading ingested documents: {str(e)}", exc_info=True)
            raise

    def save_ingested_docs(self, ingested_docs: List[str]) -> None:
        """Save the updated list of ingested documents to JSON file."""
        logger.debug(f"Saving ingested documents to {self.paths['ingested_docs_file']}: {ingested_docs}")
        try:
            with open(self.paths["ingested_docs_file"], "w", encoding="utf-8") as f:
                json.dump(ingested_docs, f, ensure_ascii=False, indent=4)
            logger.debug("Ingested documents saved successfully")
        except Exception as e:
            logger.error(f"Error saving ingested documents: {str(e)}", exc_info=True)
            raise

    def upload_document(self, file: UploadFile) -> Dict[str, str]:
        """Upload and ingest a document into the FAISS vector store."""
        logger.info(f"Received upload request for document: {file.filename}")
        try:
            ingested_docs = self.load_ingested_docs()

            if file.filename in ingested_docs:
                logger.info(f"Document '{file.filename}' has already been ingested.")
                response = {"message": f"Document '{file.filename}' has already been ingested."}
                logger.debug(f"Upload response: {response}")
                return response

            chunks = self.processor.process_document(file)
            logger.debug(f"Adding {len(chunks)} chunks to document vector store")
            self.processor.document_vector_store.add_documents(chunks)
            logger.debug(f"Saving document vector store to {self.processor.paths['document_faiss_path']}")
            self.processor.document_vector_store.save_local(self.processor.paths["document_faiss_path"])

            ingested_docs.append(file.filename)
            self.save_ingested_docs(ingested_docs)

            logger.info(f"Successfully ingested document: {file.filename}")
            response = {"message": f"Document '{file.filename}' uploaded and indexed successfully."}
            logger.debug(f"Upload response: {response}")
            return response
        except ValueError as ve:
            logger.error(f"Invalid file type for {file.filename}: {str(ve)}", exc_info=True)
            raise HTTPException(status_code=400, detail=f"Invalid file type: {str(ve)}")
        except Exception as e:
            logger.error(f"Failed to process document {file.filename}: {str(e)}", exc_info=True)
            raise HTTPException(status_code=500, detail=f"Error processing document: {str(e)}")

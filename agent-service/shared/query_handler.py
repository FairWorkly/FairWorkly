from typing import Any, Dict, List

from fastapi import HTTPException
from langchain_core.messages import HumanMessage, SystemMessage
from shared.config import CONFIG
from shared.document_processor import DocumentProcessor
from shared.logging import logger
from langchain_core.documents import Document


class QueryHandler:
    def __init__(self, processor: DocumentProcessor):
        logger.debug("Initializing QueryHandler")
        self.processor = processor
        self.faiss_config = CONFIG["faiss"]

    def retrieve_documents(self, query: str) -> List[Document]:
        """Retrieve relevant documents from the FAISS vector store."""
        logger.debug(f"Retrieving documents for query: {query}")
        try:
            retrieved_docs = self.processor.document_vector_store.similarity_search(
                query, k=self.faiss_config["similarity_search_k_docs"]
            )
            # Filter out empty or irrelevant documents
            retrieved_docs = [doc for doc in retrieved_docs if doc.page_content and doc.page_content.strip()]
            logger.info(f"Retrieved {len(retrieved_docs)} documents for query: {query}. Content: {[doc.page_content[:50] for doc in retrieved_docs]}")
            return retrieved_docs
        except Exception as e:
            logger.error(f"Error retrieving documents for query '{query}': {str(e)}", exc_info=True)
            raise
        
    def submit_query(self, query: str) -> Dict[str, Any]:
        """Handle query submission and response generation."""
        logger.info(f"Received query request: {query}")
        try:
            # Retrieve relevant documents
            retrieved_docs = self.retrieve_documents(query)

            # Substitute placeholders in the prompt template
            documents_text = "\n\n".join([doc.page_content for doc in retrieved_docs])
            logger.debug(f"Documents text: {documents_text[:200]}...")
            formatted_prompt = self.processor.prompt_template.format(
                documents_text=documents_text if documents_text else "No documents retrieved.",
            )
            logger.debug(f"Formatted prompt: {formatted_prompt[:200]}...")

            # Construct system and human messages
            system_message = SystemMessage(content=formatted_prompt)
            human_message = HumanMessage(content=query)
            logger.debug(f"System message: {system_message.content[:200]}...")
            logger.debug(f"Human message: {human_message.content}")

            # Generate response using the LLM
            logger.debug("Generating response using LLM")
            deployment_mode = CONFIG["model_params"]["deployment_mode_llm"]
            if deployment_mode == 'local':
                llm = self.processor.ensure_chat_model()
                response = llm.invoke(f"{formatted_prompt}\n\nUser Query:{query}")
            elif deployment_mode == 'huggingface':
                # Use HuggingFace InferenceClient directly with chat completion
                client_config = self.processor.ensure_chat_model()
                full_prompt = f"{formatted_prompt}\n\nUser Query:{query}"

                # Use chat_completion to avoid provider detection issues
                messages = [{"role": "user", "content": full_prompt}]
                completion = client_config["client"].chat_completion(
                    messages=messages,
                    model=client_config["model_id"],
                    max_tokens=client_config["config"]["max_new_tokens"],
                    top_p=client_config["config"]["top_p"],
                    temperature=client_config["config"]["temperature"]
                )
                response = completion.choices[0].message.content
            else:
                # For online/OpenAI models, use chat messages
                llm = self.processor.ensure_chat_model()
                response = llm.invoke([system_message, human_message]).content
            logger.info(f"Response generated: {response}...")

            # Prepare the sources list with content and metadata
            sources = [
                {"content": doc.page_content, "metadata": doc.metadata}
                for doc in retrieved_docs
            ]
            logger.info(f"Final sources list: {sources}")

            response_data = {
                "response": response,
                "sources": sources
            }
            logger.info(f"Returning response: {response_data}")
            return response_data
        except Exception as e:
            logger.error(f"Error processing query '{query}': {str(e)}", exc_info=True)
            raise HTTPException(status_code=500, detail=f"Error generating response: {str(e)}")

"""
ChromaDB Client

TODO: Connect to vector database for document storage.

ChromaDB stores document embeddings (vectors) so we can:
1. Store Fair Work documents
2. Search for similar documents when user asks question
3. Return most relevant documents to LLM

This is the LOW-LEVEL interface to ChromaDB.
For HIGH-LEVEL retrieval, use RAGRetriever instead.
"""

from typing import List, Dict, Any
import chromadb


class ChromaClient:
    """
    Client for ChromaDB vector database
    
    Usage:
        client = ChromaClient()
        client.add_documents(["Fair Work says...", "Award says..."])
        results = client.search("What is penalty rate?")
    """
    
    def __init__(self, collection_name: str = "fairwork_docs"):
        """
        Initialize ChromaDB client
        
        Args:
            collection_name: Name of the collection to use
            
        TODO: Initialize ChromaDB connection
        """
        # TODO: Connect to ChromaDB
        # self.client = chromadb.PersistentClient(path="./chroma_db")
        # self.collection = self.client.get_or_create_collection(collection_name)
        
        self.collection_name = collection_name
    
    def add_documents(
        self, 
        documents: List[str], 
        metadatas: List[Dict[str, Any]] = None,
        ids: List[str] = None
    ):
        """
        Add documents to vector database
        
        Args:
            documents: List of text chunks
            metadatas: Optional metadata for each document
            ids: Optional IDs for each document
            
        Example:
            client.add_documents(
                documents=["Saturday penalty is 150%"],
                metadatas=[{"source": "Retail Award", "section": "12.4"}],
                ids=["doc_001"]
            )
            
        TODO: Implement document insertion
        """
        # TODO: Add documents to ChromaDB
        # self.collection.add(
        #     documents=documents,
        #     metadatas=metadatas,
        #     ids=ids
        # )
        
        raise NotImplementedError("TODO: Implement add_documents")
    
    def search(
        self, 
        query: str, 
        top_k: int = 5,
        filter: Dict[str, Any] = None
    ) -> List[Dict[str, Any]]:
        """
        Search for similar documents
        
        Args:
            query: Question to search for
            top_k: How many results to return
            filter: Optional metadata filter
            
        Returns:
            List of matching documents with scores
            
        Example:
            results = client.search(
                query="What is Saturday penalty rate?",
                top_k=5,
                filter={"source": "Retail Award"}
            )
            
        TODO: Implement similarity search
        """
        # TODO: Search ChromaDB
        # results = self.collection.query(
        #     query_texts=[query],
        #     n_results=top_k,
        #     where=filter
        # )
        
        raise NotImplementedError("TODO: Implement search")
    
    def delete_collection(self):
        """
        Delete entire collection (careful!)
        
        TODO: Implement collection deletion
        """
        # TODO: Delete collection
        # self.client.delete_collection(self.collection_name)
        
        raise NotImplementedError("TODO: Implement delete_collection")
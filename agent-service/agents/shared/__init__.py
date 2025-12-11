"""
Shared Services for FairWorkly Agents

This package contains reusable code for all agents:
- LLM providers (Claude, OpenAI)
- RAG retrieval (vector database)
- File handling (Excel, CSV)
- Prompt building

Usage:
    from agents.shared.llm import LLMProvider
    from agents.shared import RAGRetriever, FileHandler
"""

# Make imports easier
from .file_handler import FileHandler
from .prompt_builder_base import PromptBuilderBase

# RAG-related (if using vector database)
try:
    from .rag_retriever import RAGRetriever
except ImportError:
    # RAG not implemented yet
    pass

# LLM providers
try:
    from .llm.factory import LLMProvider, LLMProviderFactory
except ImportError:
    # LLM not implemented yet
    pass

"""
LLM Provider Base Class

TODO: Define the interface that all LLM providers must implement.

This is the parent class for Claude, OpenAI, etc.
All providers must have these methods:
- generate() - Get AI response
- count_tokens() - Count how many tokens in text
"""

from abc import ABC, abstractmethod
from typing import List, Dict, Any


class LLMProviderBase(ABC):
    """
    Base class for all LLM providers (Claude, OpenAI, etc.)
    """
    
    @abstractmethod
    async def generate(
        self, 
        messages: List[Dict[str, str]], 
        temperature: float = 0.7,
        max_tokens: int = 4096
    ) -> Dict[str, Any]:
        """
        Generate AI response
        
        Args:
            messages: List of {role: "user", content: "question"}
            temperature: Creativity level (0-1)
            max_tokens: Maximum response length
            
        Returns:
            dict with 'content', 'model', 'usage'
        """
        pass
    
    @abstractmethod
    def count_tokens(self, text: str) -> int:
        """
        Count tokens in text
        
        Args:
            text: Text to count
            
        Returns:
            Number of tokens
        """
        pass
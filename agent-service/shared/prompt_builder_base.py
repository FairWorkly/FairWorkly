"""
Prompt Builder Base Class

TODO: Define how to build prompts for different use cases.

This helps create good prompts for:
- Compliance questions
- Payroll analysis
- Employee self-service
Each type needs different prompts.
"""

from abc import ABC, abstractmethod
from typing import List, Dict, Any


class PromptBuilderBase(ABC):
    """
    Base class for building AI prompts
    
    Each agent (Compliance, Payroll, etc.) should create their own
    PromptBuilder that inherits from this.
    """
    
    @abstractmethod
    def build_system_prompt(self, role: str = "default") -> str:
        """
        Build the system prompt (tells AI its role)
        
        Args:
            role: "admin", "manager", "employee", etc.
            
        Returns:
            System prompt text
            
        Example:
            "You are a Fair Work compliance expert helping Australian SMEs..."
        """
        pass
    
    @abstractmethod
    def build_user_prompt(
        self, 
        question: str, 
        context: List[str] = None
    ) -> str:
        """
        Build the user prompt (the actual question)
        
        Args:
            question: User's question
            context: Optional context documents from RAG
            
        Returns:
            User prompt text
            
        Example:
            Question: "What is Saturday penalty rate?"
            Context: ["Retail Award says 150%...", "NES requires..."]
            
            Returns: "Based on these documents: [...], answer: What is Saturday penalty rate?"
        """
        pass
    
    def build_messages(
        self, 
        question: str, 
        context: List[str] = None,
        role: str = "default"
    ) -> List[Dict[str, str]]:
        """
        Build complete message list for LLM
        
        Returns:
            [
                {"role": "system", "content": "You are..."},
                {"role": "user", "content": "Question..."}
            ]
        """
        messages = []
        
        # Add system prompt
        system_prompt = self.build_system_prompt(role)
        messages.append({"role": "system", "content": system_prompt})
        
        # Add user prompt
        user_prompt = self.build_user_prompt(question, context)
        messages.append({"role": "user", "content": user_prompt})
        
        return messages
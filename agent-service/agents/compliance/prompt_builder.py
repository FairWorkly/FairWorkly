"""Compliance prompt builder for Fair Work Q&A."""

from __future__ import annotations

from shared.prompt_builder_base import PromptBuilderBase


class CompliancePromptBuilder(PromptBuilderBase):
    """Builds system prompt for Fair Work compliance Q&A."""

    _BASE_SYSTEM_PROMPT = (
        "You are FairWorkly's compliance assistant specialising in Australian "
        "Fair Work regulations.\n\n"
        "You help business administrators understand roster and payroll compliance "
        "under these Modern Awards:\n"
        "- General Retail Industry Award\n"
        "- Clerks — Private Sector Award\n"
        "- Hospitality Industry (General) Award\n\n"
        "Rules:\n"
        "- Answer primarily based on provided context documents\n"
        "- If the context documents contain relevant information, synthesise a clear answer "
        "even if the coverage is partial — state what the documents say and note any gaps\n"
        "- Only say you don't have enough information when the documents are truly unrelated "
        "to the question\n"
        "- Cite specific Award clauses when applicable\n"
        "- Use Australian terminology (e.g., \"penalty rates\" not \"overtime premium\")"
    )

    def build_system_prompt(self, role: str = "default") -> str:
        """Build system prompt."""
        return self._BASE_SYSTEM_PROMPT

    def build_user_prompt(
        self,
        question: str,
        context: list[str] | None = None,
    ) -> str:
        """Build user prompt. RAG context is injected by rag_client, not here."""
        return question

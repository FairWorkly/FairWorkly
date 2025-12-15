# General chat
from typing import Dict, Any


class ChatHandler:
    """Handle general employee chat and FAQs."""

    async def handle(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        message = payload.get("message", "")

        return {
            "type": "employee_chat",
            "message": f"Employee chat placeholder - received: {message}",
            "note": "TODO: connect to LLM for general employee queries.",
        }

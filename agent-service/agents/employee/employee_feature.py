#Main entry (routes to handlers)
from typing import Dict, Any
import sys

sys.path.append("../..")
from master_agent.feature_registry import FeatureBase
from .chat_handler import ChatHandler
from .payslip_handler import PayslipHandler
from .roster_handler import RosterHandler
from .leave_handler import LeaveHandler


class EmployeeFeature(FeatureBase):
    """Employee feature routes employee requests to dedicated handlers."""

    def __init__(self) -> None:
        self.chat_handler = ChatHandler()
        self.payslip_handler = PayslipHandler()
        self.roster_handler = RosterHandler()
        self.leave_handler = LeaveHandler()

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        """
        Route requests to dedicated handlers.

        Args:
            payload: {
                "message": str,  # User message
                "action": str,   # chat | payslip | roster | leave
                "metadata": dict,
                "attachments": list | UploadFile
            }
        """
        action = (payload.get("action") or "").lower()
        message = payload.get("message", "")

        if action == "payslip":
            return await self.payslip_handler.handle(payload)
        if action == "roster":
            return await self.roster_handler.handle(payload)
        if action == "leave":
            return await self.leave_handler.handle(payload)

        # Default to general chat
        return await self.chat_handler.handle({**payload, "message": message})

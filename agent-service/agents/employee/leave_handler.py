# Leave requests

from typing import Dict, Any


class LeaveHandler:
    """Handle leave balances and requests."""

    async def handle(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        employee_id = payload.get("employee_id")
        leave_type = payload.get("leave_type")
        dates = payload.get("dates")

        return {
            "type": "employee_leave",
            "employee_id": employee_id,
            "leave_type": leave_type,
            "dates": dates,
            "note": "TODO: validate balances, submit requests, and return status.",
        }

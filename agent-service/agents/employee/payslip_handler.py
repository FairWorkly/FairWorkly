# View payslip

from typing import Dict, Any


class PayslipHandler:
    """Handle payslip-related queries and retrieval."""

    async def handle(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        employee_id = payload.get("employee_id")
        period = payload.get("period")

        return {
            "type": "employee_payslip",
            "employee_id": employee_id,
            "period": period,
            "note": "TODO: fetch payslip data, perform access checks, and return file or summary.",
        }

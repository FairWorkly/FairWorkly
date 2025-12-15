# View roster

from typing import Dict, Any


class RosterHandler:
    """Handle roster lookups for an employee."""

    async def handle(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        employee_id = payload.get("employee_id")
        date_range = payload.get("date_range")

        return {
            "type": "employee_roster",
            "employee_id": employee_id,
            "date_range": date_range,
            "note": "TODO: retrieve roster entries and format response.",
        }

"""HTTP client for communicating with the .NET backend API."""

import os
from typing import Any

import httpx


class BackendClient:
    """Client for calling .NET backend API endpoints."""

    def __init__(self):
        self.base_url = os.getenv("BACKEND_API_URL", "http://localhost:5000")
        self.api_key = os.getenv("BACKEND_API_KEY", "")
        self.timeout = float(os.getenv("BACKEND_TIMEOUT", "30.0"))

    def _get_headers(self, org_id: str) -> dict[str, str]:
        """Build headers for API requests."""
        headers = {
            "Content-Type": "application/json",
            "X-Organization-Id": org_id,
        }
        if self.api_key:
            headers["X-Service-Key"] = self.api_key
        return headers

    async def create_roster(
        self, entries: list[dict[str, Any]], org_id: str
    ) -> dict[str, Any]:
        """
        Create a roster from parsed shift entries.

        Args:
            entries: List of shift entries parsed from roster file
            org_id: Organization ID

        Returns:
            Response containing rosterId, totalShifts, etc.

        Raises:
            httpx.HTTPError: If the API request fails
        """
        async with httpx.AsyncClient(timeout=self.timeout) as client:
            response = await client.post(
                f"{self.base_url}/api/roster",
                json={"entries": entries},
                headers=self._get_headers(org_id),
            )
            response.raise_for_status()
            return response.json()

    async def validate_roster(self, roster_id: str, org_id: str) -> dict[str, Any]:
        """
        Validate a roster against compliance rules.

        Args:
            roster_id: The roster ID to validate
            org_id: Organization ID

        Returns:
            Validation response with issues and summary

        Raises:
            httpx.HTTPError: If the API request fails
        """
        async with httpx.AsyncClient(timeout=self.timeout) as client:
            response = await client.post(
                f"{self.base_url}/api/roster/validate",
                json={"rosterId": roster_id},
                headers=self._get_headers(org_id),
            )
            response.raise_for_status()
            return response.json()

    async def health_check(self) -> bool:
        """Check if the backend is healthy."""
        try:
            async with httpx.AsyncClient(timeout=5.0) as client:
                response = await client.get(f"{self.base_url}/health")
                return response.status_code == 200
        except Exception:
            return False

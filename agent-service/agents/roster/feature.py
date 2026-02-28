from typing import Dict, Any
import logging
import tempfile
from pathlib import Path

from master_agent.feature_registry import FeatureBase, feature_response
from .services.roster_import import RosterExcelParser, ParseMode


class RosterFeature(FeatureBase):
    """Roster Feature - Handle roster file upload and parsing."""

    def __init__(self):
        self.logger = logging.getLogger(__name__)
        self.roster_parser = RosterExcelParser()

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        """Process roster file upload and parsing."""
        file_upload = payload.get("file")
        file_name = payload.get("file_name")

        # Check if file has required attributes (duck typing instead of isinstance)
        if not file_upload or not hasattr(file_upload, 'read') or not hasattr(file_upload, 'filename'):
            return feature_response(
                type="roster",
                message="Roster file is required",
                note="FILE_REQUIRED",
            )

        return await self._parse_roster_file(file_upload, file_name)

    async def _parse_roster_file(
        self, file: Any, file_name: str
    ) -> Dict[str, Any]:
        """Parse roster Excel file and return structured data."""
        temp_file_path = None
        try:
            # Save uploaded file to temporary location
            with tempfile.NamedTemporaryFile(
                mode="wb", suffix=".xlsx", delete=False
            ) as temp_file:
                content = await file.read()
                temp_file.write(content)
                temp_file_path = temp_file.name

            # Parse the roster file using LENIENT mode (allow warnings)
            parse_response = self.roster_parser.parse_roster_excel(
                file_path=temp_file_path, mode=ParseMode.LENIENT
            )

            # Return model_dump() at top level — the .NET backend deserializes
            # this directly as ParseResponse (result, issues, summary).
            # Standard fields (type, message, etc.) are merged alongside.
            result = parse_response.model_dump()
            result.update({
                "type": "roster",
                "message": f"Roster file '{file_name}' parsed successfully.",
                "model": None,
                "sources": [],
                "note": None,
            })
            return result

        except Exception as exc:
            self.logger.error(f"Failed to parse roster file: {exc}", exc_info=True)
            return feature_response(
                type="roster",
                message=f"Failed to parse roster file: {str(exc)}",
                note="PARSE_ERROR",
            )
        finally:
            # Clean up temporary file
            if temp_file_path and Path(temp_file_path).exists():
                Path(temp_file_path).unlink()

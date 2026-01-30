"""RosterExcelParser orchestrator."""

from __future__ import annotations

from typing import Any, Optional
from datetime import date, time, datetime
from pathlib import Path

from fastapi import UploadFile

from .excel_reader import read_excel
from .header_map import HeaderMap, EXTRA_KEY
from .issues import build_response
from .models import (
    EmployeeParseResult,
    ParseIssue,
    ParseIssueError,
    ParseIssueSeverity,
    ParseMode,
    ParseResponse,
    RosterParseResult,
)
from .row_parsers import parse_employee_row, parse_roster_row
from .utils import build_raw_row


class RosterExcelParser:
    """
    Parses roster Excel files into typed shift entries.
    """

    ROSTER_REQUIRED_KEYS = ["employee_email", "date", "start_time", "end_time"]
    EMPLOYEE_REQUIRED_KEYS = ["name", "email", "role"]

    def __init__(self) -> None:
        self._header_map = HeaderMap()

    def read_excel(
        self,
        file_path: str,
        sheet_name: Optional[str] = None,
        header_row: Optional[int] = None,
    ) -> list[dict[str, Any]]:
        return read_excel(file_path, sheet_name, header_row=header_row)

    def validate_headers(
        self,
        data: list[dict[str, Any]],
        required_keys: list[str],
    ) -> tuple[bool, list[str]]:
        if not data:
            return False, required_keys

        normalized = self._header_map.normalize_headers(data[0])
        available_keys = {key.lower() for key in normalized.keys()}

        missing = []
        for required in required_keys:
            if required.lower() not in available_keys:
                missing.append(required)

        return len(missing) == 0, missing

    def parse_roster_excel(
        self,
        file_path: str,
        sheet_name: Optional[str] = None,
        header_row: Optional[int] = None,
        mode: ParseMode = ParseMode.LENIENT,
    ) -> ParseResponse:
        mode = self._coerce_mode(mode)
        data, error_response = self._read_and_validate_excel(
            file_path, sheet_name, header_row, self.ROSTER_REQUIRED_KEYS, RosterParseResult
        )
        if error_response:
            return error_response

        entries = []
        issues: list[ParseIssue] = []
        raw_rows: list[dict[str, Any]] = []

        for row in data:
            real_row = row.get("__row__", 0)
            normalized = self._header_map.normalize_headers(row)
            raw_rows.append(build_raw_row(normalized, real_row, EXTRA_KEY))
            try:
                entry, row_warnings = parse_roster_row(row, real_row, self._header_map, mode=mode)
                entries.append(entry)
                issues.extend(row_warnings)
            except ParseIssueError as exc:
                issues.append(exc.issue)
            except Exception as exc:
                issues.append(
                    ParseIssue(
                        row=real_row,
                        severity=ParseIssueSeverity.ERROR,
                        code="ROW_PARSE_ERROR",
                        message=str(exc),
                    )
                )

        return build_response(RosterParseResult(entries=entries, raw_rows=raw_rows), issues)

    def parse_employee_excel(
        self,
        file_path: str,
        sheet_name: Optional[str] = None,
        header_row: Optional[int] = None,
        mode: ParseMode = ParseMode.LENIENT,
    ) -> ParseResponse:
        mode = self._coerce_mode(mode)
        data, error_response = self._read_and_validate_excel(
            file_path, sheet_name, header_row, self.EMPLOYEE_REQUIRED_KEYS, EmployeeParseResult
        )
        if error_response:
            return error_response

        entries = []
        issues: list[ParseIssue] = []
        raw_rows: list[dict[str, Any]] = []

        for row in data:
            real_row = row.get("__row__", 0)
            try:
                normalized = self._header_map.normalize_headers(row)
                raw_rows.append(build_raw_row(normalized, real_row, EXTRA_KEY))
                entry, row_warnings = parse_employee_row(row, real_row, self._header_map, mode=mode)
                entries.append(entry)
                issues.extend(row_warnings)
            except ParseIssueError as exc:
                issues.append(exc.issue)
            except Exception as exc:
                issues.append(
                    ParseIssue(
                        row=real_row,
                        severity=ParseIssueSeverity.ERROR,
                        code="ROW_PARSE_ERROR",
                        message=str(exc),
                    )
                )

        return build_response(EmployeeParseResult(entries=entries, raw_rows=raw_rows), issues)

    async def parse_excel(
        self,
        file: UploadFile,
        header_row: Optional[int] = None,
    ) -> list[dict[str, Any]]:
        import os
        import tempfile

        suffix = Path(file.filename).suffix if file.filename else ".xlsx"
        tmp_path: Optional[str] = None
        try:
            with tempfile.NamedTemporaryFile(delete=False, suffix=suffix) as tmp:
                tmp_path = tmp.name
                content = await file.read()
                tmp.write(content)
            return self.read_excel(tmp_path, header_row=header_row)
        finally:
            if tmp_path:
                try:
                    os.unlink(tmp_path)
                except FileNotFoundError:
                    pass

    def _coerce_mode(self, mode: ParseMode | str) -> ParseMode:
        if isinstance(mode, ParseMode):
            return mode
        return ParseMode(mode)

    def _read_and_validate_excel(
        self,
        file_path: str,
        sheet_name: Optional[str],
        header_row: Optional[int],
        required_keys: list[str],
        result_class: type,
    ) -> tuple[Optional[list[dict[str, Any]]], Optional[ParseResponse]]:
        """
        Common logic for reading and validating Excel files.

        Returns (data, error_response). If error_response is not None, caller should return it.
        """
        try:
            data = self.read_excel(file_path, sheet_name, header_row=header_row)
        except FileNotFoundError as exc:
            issues = [
                ParseIssue(
                    row=0,
                    severity=ParseIssueSeverity.ERROR,
                    code="FILE_NOT_FOUND",
                    message=str(exc),
                )
            ]
            return None, build_response(result_class(entries=[], raw_rows=[]), issues)
        except (ValueError, OSError) as exc:
            code = "INVALID_HEADER_ROW" if "Header row not found" in str(exc) else "FILE_READ_ERROR"
            issues = [
                ParseIssue(
                    row=0,
                    severity=ParseIssueSeverity.ERROR,
                    code=code,
                    message=str(exc),
                )
            ]
            return None, build_response(result_class(entries=[], raw_rows=[]), issues)

        if not data:
            issues = [
                ParseIssue(
                    row=0,
                    severity=ParseIssueSeverity.ERROR,
                    code="EMPTY_FILE",
                    message="Excel file is empty or contains no data rows",
                )
            ]
            return None, build_response(result_class(entries=[], raw_rows=[]), issues)

        is_valid, missing = self.validate_headers(data, required_keys)
        if not is_valid:
            issues = [
                ParseIssue(
                    row=0,
                    severity=ParseIssueSeverity.ERROR,
                    code="MISSING_REQUIRED_COLUMNS",
                    message=f"Missing required columns: {', '.join(missing)}",
                )
            ]
            return None, build_response(result_class(entries=[], raw_rows=[]), issues)

        return data, None

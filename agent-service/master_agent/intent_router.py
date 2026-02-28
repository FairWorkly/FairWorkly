class IntentRouter:
    """Routes incoming requests to the appropriate feature based on intent_hint."""

    _HINT_MAP = {
        "roster_explain": "roster_explain",
        "roster": "roster",
        "payroll": "payroll_verify",
        "payroll_explain": "payroll_verify",
        "compliance": "compliance_qa",
        "compliance_qa": "compliance_qa",
    }

    def route(self, message: str, file_name: str = None, intent_hint: str = None) -> str:
        """Determine which feature handles this request.

        Primary: explicit intent_hint from the client.
        Fallback: file upload → roster, otherwise → compliance_qa.
        """
        if intent_hint:
            hint = intent_hint.lower().strip()
            resolved = self._HINT_MAP.get(hint)
            if resolved:
                # roster hint without a file means the user wants to explain results
                if hint == "roster" and not file_name:
                    return "roster_explain"
                return resolved

        # Fallback: file upload → roster, no file → compliance_qa
        if file_name:
            return "roster"
        return "compliance_qa"

import json
import logging
import re
from typing import Any, Dict, Optional

from master_agent.config import load_config
from master_agent.feature_registry import FeatureBase, feature_response
from shared.llm.factory import LLMProviderFactory
from shared.llm.history_utils import normalize_chat_history


class RosterExplainFeature(FeatureBase):
    """Explain existing roster validation results without re-running checks."""
    _ACTION_PLAN_REQUEST_MARKERS = (
        "next step",
        "next steps",
        "how to fix",
        "fix",
        "action plan",
        "what should i do",
        "what do i do",
        "implementation plan",
        "step by step",
    )
    _CHECK_TEMPLATES: Dict[str, Dict[str, str]] = {
        "consecutivedays": {
            "title": "Limit consecutive work days",
            "what_to_change": "Insert a rest day and rebalance shifts so no employee exceeds the consecutive-day cap.",
            "why": "Excess consecutive days increases fatigue risk and triggers roster compliance breaches.",
            "expected_outcome": "Consecutive-days warnings and related fatigue risks are reduced.",
            "risk_if_ignored": "Repeated fatigue-related breaches may expose the business to compliance and safety risk.",
        },
        "mealbreak": {
            "title": "Fix meal break scheduling",
            "what_to_change": "Update shift templates to include compliant meal break length and timing for affected shifts.",
            "why": "Missing or short meal breaks are direct award non-compliance for eligible shift durations.",
            "expected_outcome": "Meal-break errors are removed for affected employees and shifts.",
            "risk_if_ignored": "Ongoing meal-break breaches can lead to repeated compliance failures and employee disputes.",
        },
        "weeklyhourslimit": {
            "title": "Rebalance weekly hours",
            "what_to_change": "Reduce hours for over-limit employees and redistribute workload to available staff.",
            "why": "Weekly-hour overages indicate roster load is outside configured legal/award limits.",
            "expected_outcome": "Weekly-hours issues are resolved while maintaining coverage.",
            "risk_if_ignored": "Persistent over-allocation may result in overtime and compliance exposure.",
        },
        "minimumshifthours": {
            "title": "Adjust short shifts to minimum duration",
            "what_to_change": "Extend or merge short shifts so each affected shift meets minimum required hours.",
            "why": "Under-minimum shifts breach roster minimum engagement rules.",
            "expected_outcome": "Minimum-shift-hour errors are eliminated for impacted shifts.",
            "risk_if_ignored": "Employees may be under-engaged against award requirements, causing repeat failures.",
        },
        "restperiod": {
            "title": "Enforce minimum rest between shifts",
            "what_to_change": "Move start times or reassign adjacent shifts to restore compliant rest intervals.",
            "why": "Insufficient rest periods are high-risk fatigue and compliance violations.",
            "expected_outcome": "Rest-period breaches are resolved and roster fatigue risk is reduced.",
            "risk_if_ignored": "Continued rest-period breaches may create safety risk and legal exposure.",
        },
        "dataquality": {
            "title": "Correct roster data integrity issues",
            "what_to_change": "Fix inconsistent break/shift data and validate roster fields before publish.",
            "why": "Invalid roster data can create false calculations and hide true compliance risks.",
            "expected_outcome": "Validation signals become reliable and false positives are reduced.",
            "risk_if_ignored": "Dirty data can cause repeated validation failures and poor scheduling decisions.",
        },
        "default": {
            "title": "Resolve remaining compliance issues",
            "what_to_change": "Review flagged shifts, apply targeted schedule edits, and re-run validation.",
            "why": "Unresolved issues keep the roster in failed status.",
            "expected_outcome": "Issue count declines and roster moves toward compliant status.",
            "risk_if_ignored": "Compliance failures will continue across publish cycles.",
        },
    }
    _FOLLOW_UP_TEMPLATES = (
        {
            "label_suffix": "Shift edits",
            "prompt": (
                "For {title}, provide concrete shift-level edits using only the affected shifts. "
                "Output per employee/date: before -> after, and explain why each edit resolves the issue."
            ),
        },
        {
            "label_suffix": "Manager checklist",
            "prompt": (
                "For {title}, create an execution checklist for the roster manager. "
                "Include sequencing, owner, and completion criteria for each step."
            ),
        },
        {
            "label_suffix": "Validation checks",
            "prompt": (
                "For {title}, list the exact validation checks to run after schedule changes. "
                "Include pass criteria and what to do if a check still fails."
            ),
        },
    )

    def __init__(self):
        self.logger = logging.getLogger(__name__)
        self.config = load_config()

    def _resolve_provider_type(self) -> str:
        features = self.config.get("features", {})
        roster_feature = features.get("roster", {})
        return roster_feature.get("llm_provider", "openai")

    def _extract_validation(self, context_payload: Any) -> Optional[Dict[str, Any]]:
        if not isinstance(context_payload, dict):
            return None

        nested = context_payload.get("validation")
        if isinstance(nested, dict):
            return nested

        # Support sending raw validation payload directly.
        if "issues" in context_payload and "validationId" in context_payload:
            return context_payload

        return None

    def _build_trimmed_context(self, validation: Dict[str, Any]) -> Dict[str, Any]:
        issues = validation.get("issues") or []
        issue_preview = issues[:25] if isinstance(issues, list) else []

        return {
            "validation_id": validation.get("validationId"),
            "status": validation.get("status"),
            "totals": {
                "total_shifts": validation.get("totalShifts"),
                "passed_shifts": validation.get("passedShifts"),
                "failed_shifts": validation.get("failedShifts"),
                "total_issues": validation.get("totalIssues"),
                "critical_issues": validation.get("criticalIssues"),
                "affected_employees": validation.get("affectedEmployees"),
            },
            "week_start_date": validation.get("weekStartDate"),
            "week_end_date": validation.get("weekEndDate"),
            "issues_preview": issue_preview,
            "issues_preview_count": len(issue_preview),
            "issues_total_count": len(issues) if isinstance(issues, list) else 0,
        }

    def _fallback_message(
        self,
        validation: Dict[str, Any],
        question: str,
        reason: Optional[str] = None,
    ) -> str:
        total_issues = validation.get("totalIssues", 0)
        critical_issues = validation.get("criticalIssues", 0)
        affected = validation.get("affectedEmployees", 0)
        reason_text = (
            f"Reason: {reason}. "
            if reason
            else "Reason: explanation model unavailable. "
        )
        return (
            "Roster explanation context loaded. "
            f"Found {total_issues} issue(s), including {critical_issues} critical issue(s), "
            f"affecting {affected} employee(s). "
            f"Your question was: \"{question}\". "
            f"{reason_text}"
            "This is a summary-only response."
        )

    def _normalize_response_format(self, text: str) -> str:
        """Normalize common markdown styling into plain text lines."""
        if not text:
            return text

        lines: list[str] = []
        for line in text.splitlines():
            normalized = re.sub(r"^\s{0,3}#{1,6}\s+", "", line)
            normalized = re.sub(r"^\s*[-*]\s+", "", normalized)
            normalized = re.sub(r"^\s*\d+[.)]\s+", "", normalized)
            normalized = re.sub(r"(\*\*|__)(.*?)\1", r"\2", normalized)
            normalized = re.sub(r"(?<!\*)\*(?!\*)(.*?)\*(?<!\*)", r"\1", normalized)
            normalized = re.sub(r"_(.*?)_", r"\1", normalized)
            lines.append(normalized)

        return "\n".join(lines).strip()

    def _classify_llm_error(self, exc: Exception) -> tuple[str, str]:
        message = str(exc).lower()
        error_type = exc.__class__.__name__.lower()

        if (
            "api key" in message
            or "api_key" in message
            or "authentication" in message
            or "unauthorized" in message
            or "401" in message
            or "permission denied" in message
        ):
            return (
                "LLM_AUTH_ERROR",
                "provider authentication failed (check API key and permissions)",
            )

        if (
            "rate limit" in message
            or "quota" in message
            or "too many requests" in message
            or "429" in message
        ):
            return ("LLM_RATE_LIMITED", "provider rate limit or quota reached")

        if (
            "timeout" in message
            or "timed out" in message
            or "connection" in message
            or "network" in message
            or "dns" in message
            or "name or service not known" in message
            or "temporary failure" in message
            or "service unavailable" in message
        ):
            return ("LLM_NETWORK_ERROR", "network/connectivity issue while calling provider")

        if (
            "model" in message and "not found" in message
            or "does not exist" in message
            or "invalid model" in message
            or "not supported" in message
            or "badrequesterror" in error_type
        ):
            return ("LLM_MODEL_ERROR", "model configuration is invalid or unavailable")

        return ("LLM_UNAVAILABLE", "unexpected provider error")

    def _is_follow_up_question(self, question: str, history_messages: list[dict]) -> bool:
        text = (question or "").strip().lower()
        if not text:
            return False
        if not history_messages:
            return False

        follow_up_markers = [
            "more",
            "detail",
            "details",
            "explain more",
            "elaborate",
            "clarify",
            "why",
            "how",
            "next step",
            "next steps",
            "further",
            "again",
            "that",
            "those",
            "this",
            "it",
            "them",
        ]
        marker_matched = any(marker in text for marker in follow_up_markers)
        if marker_matched:
            return True

        # Quick-follow-up buttons generate imperative prompts like:
        # "For <title>, provide concrete shift-level edits..."
        # Treat these as follow-up turns so we avoid repeating the full recap.
        if text.startswith("for ") and (
            "provide concrete shift-level edits" in text
            or "execution checklist" in text
            or "validation checks" in text
        ):
            return True

        # No explicit marker: if there is prior turn history, treat short referential questions as follow-up.
        if len(text) <= 48:
            referential_terms = ["that", "this", "it", "those", "them"]
            return any(term in text for term in referential_terms)

        return False

    def _build_follow_up_context(self, validation: Dict[str, Any]) -> Dict[str, Any]:
        issues = validation.get("issues") or []
        issue_preview = issues[:25] if isinstance(issues, list) else []
        return {
            "validation_id": validation.get("validationId"),
            "issues_for_action": issue_preview,
            "issues_count_for_action": len(issue_preview),
        }

    def _normalize_check_key(self, raw_check_type: Any) -> str:
        text = str(raw_check_type or "").strip().lower()
        if not text:
            return "default"
        normalized = re.sub(r"[^a-z0-9]", "", text)
        return normalized or "default"

    def _severity_weight(self, raw_severity: Any) -> int:
        text = str(raw_severity or "").strip().lower()
        if text == "error":
            return 3
        if text == "warning":
            return 2
        if text == "info":
            return 1
        return 1

    def _summarize_affected(self, affected_items: list[dict]) -> str:
        if not affected_items:
            return "Review the flagged shifts in this category."

        snippets: list[str] = []
        for item in affected_items[:3]:
            employee = item.get("employee") or "Unknown employee"
            dates = item.get("dates") or "affected dates"
            snippets.append(f"{employee} ({dates})")
        return "; ".join(snippets)

    def _build_action_plan(self, validation: Dict[str, Any]) -> Optional[Dict[str, Any]]:
        issues = validation.get("issues") or []
        if not isinstance(issues, list) or len(issues) == 0:
            total_issues = int(validation.get("totalIssues") or 0)
            critical_issues = int(validation.get("criticalIssues") or 0)
            fallback_actions = [
                {
                    "id": "action_1",
                    "priority": "P1",
                    "title": "Prioritize critical roster breaches",
                    "owner": "Roster manager",
                    "check_type": "General",
                    "issue_count": critical_issues or total_issues,
                    "critical_count": critical_issues,
                    "affected_shifts": [],
                    "what_to_change": "Review and fix the highest-severity failed shifts first, then sequence remaining fixes.",
                    "why": "Resolving critical breaches first reduces legal and fatigue risk fastest.",
                    "expected_outcome": "Critical issue count decreases and immediate compliance risk is reduced.",
                    "risk_if_ignored": "Critical breaches may continue and expose the business to regulatory action.",
                    "focus_examples": "Start with shifts linked to failed/critical checks in the latest validation.",
                },
                {
                    "id": "action_2",
                    "priority": "P2",
                    "title": "Adjust breaks, hours, and rest windows",
                    "owner": "Roster manager",
                    "check_type": "General",
                    "issue_count": total_issues,
                    "critical_count": critical_issues,
                    "affected_shifts": [],
                    "what_to_change": "Update shift timing, break allocations, and weekly load balancing for impacted employees.",
                    "why": "Most roster failures are driven by break, hours, and rest constraints.",
                    "expected_outcome": "A broader set of compliance failures is resolved in one pass.",
                    "risk_if_ignored": "Repeated scheduling patterns will continue to trigger failed validations.",
                    "focus_examples": "Use the failed-check list from validation output to target exact shifts.",
                },
                {
                    "id": "action_3",
                    "priority": "P3",
                    "title": "Re-run validation and close remaining gaps",
                    "owner": "Roster manager",
                    "check_type": "General",
                    "issue_count": total_issues,
                    "critical_count": critical_issues,
                    "affected_shifts": [],
                    "what_to_change": "After edits, run validation again and resolve any remaining failed checks until pass.",
                    "why": "Compliance status only updates after a successful re-validation cycle.",
                    "expected_outcome": "Roster moves from failed toward compliant with measurable issue reduction.",
                    "risk_if_ignored": "Unverified fixes may leave hidden non-compliance in production rosters.",
                    "focus_examples": "Confirm all previously failed checks now pass before publishing.",
                },
            ]

            quick_follow_ups = []
            for index, action in enumerate(fallback_actions):
                template = self._FOLLOW_UP_TEMPLATES[index % len(self._FOLLOW_UP_TEMPLATES)]
                quick_follow_ups.append(
                    {
                        "id": f"follow_up_{action['id']}",
                        "label": f"{action['priority']} {template['label_suffix']}",
                        "prompt": template["prompt"].format(title=action["title"]),
                        "action_id": action["id"],
                    }
                )

            return {
                "title": "Top 3 actions to fix this roster",
                "validation_id": validation.get("validationId"),
                "actions": fallback_actions,
                "quick_follow_ups": quick_follow_ups,
            }

        grouped: Dict[str, Dict[str, Any]] = {}
        for issue in issues:
            if not isinstance(issue, dict):
                continue

            key = self._normalize_check_key(issue.get("checkType"))
            group = grouped.setdefault(
                key,
                {
                    "score": 0,
                    "count": 0,
                    "critical": 0,
                    "check_type": issue.get("checkType") or "General",
                    "affected": [],
                },
            )

            severity_weight = self._severity_weight(issue.get("severity"))
            group["score"] += (severity_weight * 2) + 1
            group["count"] += 1
            if severity_weight >= 3:
                group["critical"] += 1

            group["affected"].append(
                {
                    "employee": issue.get("employeeName") or issue.get("employeeId") or "Unknown employee",
                    "dates": issue.get("affectedDates") or "affected dates",
                    "description": issue.get("description") or "",
                }
            )

        sorted_groups = sorted(
            grouped.items(),
            key=lambda kv: (
                kv[1]["score"],
                kv[1]["critical"],
                kv[1]["count"],
            ),
            reverse=True,
        )

        top_groups = sorted_groups[:3]
        actions: list[Dict[str, Any]] = []
        for index, (group_key, group_data) in enumerate(top_groups, start=1):
            template = self._CHECK_TEMPLATES.get(group_key, self._CHECK_TEMPLATES["default"])
            affected_summary = self._summarize_affected(group_data.get("affected", []))

            actions.append(
                {
                    "id": f"action_{index}",
                    "priority": f"P{index}",
                    "title": template["title"],
                    "owner": "Roster manager",
                    "check_type": group_data.get("check_type"),
                    "issue_count": group_data.get("count", 0),
                    "critical_count": group_data.get("critical", 0),
                    "affected_shifts": group_data.get("affected", [])[:3],
                    "what_to_change": template["what_to_change"],
                    "why": template["why"],
                    "expected_outcome": template["expected_outcome"],
                    "risk_if_ignored": template["risk_if_ignored"],
                    "focus_examples": affected_summary,
                }
            )

        quick_follow_ups = []
        for index, action in enumerate(actions):
            template = self._FOLLOW_UP_TEMPLATES[index % len(self._FOLLOW_UP_TEMPLATES)]
            quick_follow_ups.append(
                {
                    "id": f"follow_up_{action['id']}",
                    "label": f"{action['priority']} {template['label_suffix']}",
                    "prompt": template["prompt"].format(title=action["title"]),
                    "action_id": action["id"],
                }
            )

        return {
            "title": "Top 3 actions to fix this roster",
            "validation_id": validation.get("validationId"),
            "actions": actions,
            "quick_follow_ups": quick_follow_ups,
        }

    def _should_include_action_plan(self, question: str, is_follow_up: bool) -> bool:
        # Keep action cards visible across explain turns for UI continuity.
        # Repetition control should be handled by rendering strategy, not by dropping payload.
        return True

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        message = (payload.get("message") or "").strip()
        if not message:
            return feature_response(
                type="roster_explain",
                message="Please enter a question about this roster validation result.",
                note="MESSAGE_REQUIRED",
            )

        context_payload = payload.get("context_payload")
        validation = self._extract_validation(context_payload)
        if validation is None:
            return feature_response(
                type="roster_explain",
                message=(
                    "I can explain roster results once you provide a roster validation context. "
                    "Please open FairBot from a roster results page."
                ),
                note="ROSTER_CONTEXT_REQUIRED",
            )

        provider_type = self._resolve_provider_type()
        history_messages = normalize_chat_history(payload.get("history_payload"))
        is_follow_up = self._is_follow_up_question(message, history_messages)
        context_for_prompt = (
            self._build_follow_up_context(validation)
            if is_follow_up
            else self._build_trimmed_context(validation)
        )
        context_text = json.dumps(context_for_prompt, ensure_ascii=True, default=str, indent=2)

        system_prompt = (
            "You are FairWorkly's compliance assistant with access to roster validation results. "
            "If the user's question is about the roster validation results, explain the outcomes "
            "in plain language for HR users. Never recompute compliance and never invent extra data. "
            "Use only the provided validation context, and when data is missing, say so explicitly. "
            "If the user's question is a general compliance question unrelated to these specific "
            "roster results (e.g., 'what are penalty rates?'), answer it based on your knowledge "
            "of Australian Fair Work regulations without forcing the roster context into the answer. "
            "For follow-up questions, continue from conversation history and avoid repeating the full summary unless asked. "
            "Answer in the same language as the user's question. "
            "Return plain text only and do not use Markdown headings."
        )
        if is_follow_up:
            response_contract = (
                "This is a follow-up question. Continue from prior turns.\n"
                "Focus only on what the user asked now.\n"
                "Do not repeat overall validation summary, total counts, or a full list of checks.\n"
                "If asked about next steps, provide practical step-by-step actions with:\n"
                "- what to change in the roster\n"
                "- why it fixes the compliance risk\n"
                "- who/which shifts are affected\n"
                "Answer directly without introductory recap."
            )
        else:
            response_contract = (
                "If the question is about these roster results, provide:\n"
                "1) A concise explanation (max 4 lines)\n"
                "2) A short bridge sentence that the detailed action cards are provided separately\n"
                "Do not output a long full recap."
            )

        user_prompt = (
            f"User question:\n{message}\n\n"
            f"Roster validation context (JSON, use only if relevant to the question):\n{context_text}\n\n"
            f"{response_contract}\n\n"
            "If the question is a general compliance question, answer it directly."
        )
        action_plan = (
            self._build_action_plan(validation)
            if self._should_include_action_plan(message, is_follow_up)
            else None
        )
        response_data = {"action_plan": action_plan} if action_plan else None

        try:
            llm = LLMProviderFactory.create(provider_type=provider_type)
            messages = [{"role": "system", "content": system_prompt}]
            messages.extend(history_messages)
            messages.append({"role": "user", "content": user_prompt})
            llm_response = await llm.generate(
                messages,
                temperature=0.2,
                max_tokens=800,
            )
            content = (llm_response.get("content") or "").strip()
            if not content:
                content = self._fallback_message(validation, message)
            else:
                content = self._normalize_response_format(content)

            return feature_response(
                type="roster_explain",
                message=content,
                model=llm_response.get("model"),
                data=response_data,
            )
        except Exception as exc:
            self.logger.warning("Roster explain LLM unavailable: %s", exc, exc_info=True)
            note, reason = self._classify_llm_error(exc)
            return feature_response(
                type="roster_explain",
                message=self._fallback_message(validation, message, reason),
                note=note,
                data=response_data,
            )

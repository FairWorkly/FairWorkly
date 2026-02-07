---
title: "Agent: Compliance Prompt Builder"
labels: ["agent-service", "python", "prompt", "medium-priority"]
---

## Goal
Create a specialized prompt builder for compliance scenarios. Inherits from `PromptBuilderBase` and provides optimized prompts for Fair Work compliance questions.

## Files
- `agent-service/agents/compliance/compliance_prompt_builder.py` (create)
- `agent-service/tests/test_compliance_prompt_builder.py` (create)

## Implementation

### 1. Create `CompliancePromptBuilder` class
```python
from agents.shared.prompt_builder_base import PromptBuilderBase

class CompliancePromptBuilder(PromptBuilderBase):

    def build_system_prompt(self, role: str = "default") -> str:
        base = """You are FairWorkly's compliance assistant specializing in Australian Fair Work regulations.

Rules:
- Only answer based on provided context documents
- If unsure, say "I don't have enough information"
- Cite specific Award clauses when applicable
- Use Australian terminology (e.g., "penalty rates" not "overtime premium")
"""

        role_additions = {
            "admin": "You are helping a business owner understand compliance obligations.",
            "manager": "You are helping a manager with day-to-day compliance questions.",
            "employee": "You are helping an employee understand their rights."
        }

        return base + role_additions.get(role, "")

    def build_user_prompt(self, question: str, context: list = None) -> str:
        if context:
            docs = "\n---\n".join(context)
            return f"""Based on these Fair Work documents:

{docs}

---
Question: {question}

Provide a clear, practical answer with specific references."""

        return question
```

### 2. 不同场景的 Prompt 模板

**Payroll 合规检查:**
```python
def build_payroll_check_prompt(self, payroll_data: dict) -> str:
    return f"""Review this payroll data for compliance:

{json.dumps(payroll_data, indent=2)}

Check for:
1. Correct penalty rates (weekend, public holiday)
2. Overtime calculations
3. Minimum wage compliance
4. Leave entitlements

Return issues in structured format."""
```

**Roster 合规检查:**
```python
def build_roster_check_prompt(self, roster_data: dict) -> str:
    return f"""Review this roster for compliance:

{json.dumps(roster_data, indent=2)}

Check for:
1. Minimum break between shifts (10-12 hours)
2. Maximum hours per week
3. Required rest days
4. Junior employee restrictions

Return issues and suggestions."""
```

## Usage Example
```python
from agents.compliance.compliance_prompt_builder import CompliancePromptBuilder

builder = CompliancePromptBuilder()

# 基础用法
messages = builder.build_messages(
    question="What is the Saturday penalty rate for retail?",
    context=["Retail Award clause 26.1..."],
    role="manager"
)

# Payroll 专用
payroll_prompt = builder.build_payroll_check_prompt(payroll_data)

# Roster 专用
roster_prompt = builder.build_roster_check_prompt(roster_data)
```

## Acceptance Criteria
- [ ] 继承 `PromptBuilderBase`
- [ ] 实现 `build_system_prompt()` 支持不同角色
- [ ] 实现 `build_user_prompt()` 整合 RAG context
- [ ] 提供 `build_payroll_check_prompt()` 方法
- [ ] 提供 `build_roster_check_prompt()` 方法
- [ ] 单元测试覆盖

## Notes
- 可被 ComplianceFeature、PayrollFeature、RosterFeature 复用
- Prompt 设计原则：准确性 > 创造性
- 明确要求 LLM 不要编造法规条款

---
title: "Agent: Roster Feature"
labels: ["agent-service", "python", "roster", "high-priority"]
---

## Goal
Create Roster Agent Feature for shift scheduling compliance checking. Uses OpenAI for flexible scheduling suggestions.

## Files
- `agent-service/agents/roster/__init__.py` (create)
- `agent-service/agents/roster/roster_feature.py` (create)
- `agent-service/master_agent/main.py` (update - 注册 feature)
- `agent-service/tests/test_roster_feature.py` (create)

## Implementation

### 1. Create `RosterFeature` class
```python
from master_agent.feature_registry import FeatureBase
from agents.shared.llm.factory import LLMProvider  # 现有 OpenAI provider

class RosterFeature(FeatureBase):
    def __init__(self):
        self.llm = LLMProvider()  # 用 OpenAI

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        message = payload.get("message")
        file = payload.get("file")

        # 1. 如果有文件，解析 roster 数据
        # 2. 检查排班合规性
        # 3. 返回结果和建议
```

### 2. Register in `main.py`
```python
from agents.roster.roster_feature import RosterFeature

registry.register("compliance_roster", RosterFeature())
```

### 3. 功能点
- 排班合规检查
- 休息时间验证（breaks between shifts）
- 最大工时验证
- 排班建议生成

## Example Input/Output

**Input:**
```json
{
  "message": "Check this roster for compliance issues",
  "file_name": "roster_week1.xlsx"
}
```

**Output:**
```json
{
  "type": "roster",
  "compliant": false,
  "issues": [
    {
      "employee": "Jane Doe",
      "issue": "Less than 10 hours between shifts",
      "date": "2024-01-15",
      "details": "Shift ended 11pm, next shift starts 7am (8 hours gap)"
    }
  ],
  "suggestions": [
    "Consider moving Jane's Monday shift to start at 9am"
  ],
  "model": "gpt-4o-mini"
}
```

## Acceptance Criteria
- [ ] `RosterFeature` 继承 `FeatureBase`
- [ ] 使用 OpenAI (现有 LLMProvider)
- [ ] 能处理 roster 文件上传
- [ ] 返回结构化的合规检查结果
- [ ] 提供排班优化建议
- [ ] 在 `main.py` 正确注册
- [ ] IntentRouter 能正确路由到此 feature

## Notes
- 使用 OpenAI 因为排班建议可以更灵活
- 可复用 `file_handler.py` 的 Excel 解析功能
- 可复用 RAG retriever 获取 Award 相关信息

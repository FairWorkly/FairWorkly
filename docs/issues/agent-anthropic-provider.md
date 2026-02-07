---
title: "Agent: Anthropic (Claude) Provider"
labels: ["agent-service", "python", "llm", "high-priority", "good-first-issue"]
---

## Goal
Implement Claude API integration for the agent-service. This provider will be used by Payroll Agent for compliance calculations where accuracy is critical.

**难度：低** - 复用现有 `langchain_provider.py` 结构，约 30 行代码。

## Files
- `agent-service/agents/shared/llm/anthropic_provider.py` (update - 已有 TODO 占位)
- `agent-service/agents/shared/llm/factory.py` (update - 添加 anthropic 分支)
- `agent-service/config.yaml` (update - 添加 Claude model 配置)
- `agent-service/pyproject.toml` (update - 添加依赖)

## Dependencies
Add to `pyproject.toml`:
```toml
langchain-anthropic = "^0.3.0"
```

## Implementation

### 1. 复制 `langchain_provider.py` 结构，改用 `ChatAnthropic`

```python
"""LangChain-backed provider using ChatAnthropic."""

import os
from typing import List, Dict, Any

from langchain_core.messages import SystemMessage, HumanMessage, AIMessage
from langchain_anthropic import ChatAnthropic

from .provider_base import LLMProviderBase


class LangChainAnthropicProvider(LLMProviderBase):
    """LLM provider that relies on LangChain ChatAnthropic."""

    def __init__(
        self,
        model: str | None = None,
        temperature: float | None = None,
    ):
        self.model = model or os.getenv("ANTHROPIC_MODEL", "claude-sonnet-4-20250514")
        api_key = os.getenv("ANTHROPIC_API_KEY")
        if not api_key:
            raise ValueError("ANTHROPIC_API_KEY must be set")

        self.chat = ChatAnthropic(
            model=self.model,
            temperature=temperature or 0.3,
            anthropic_api_key=api_key,
        )

    async def generate(
        self,
        messages: List[Dict[str, str]],
        temperature: float = 0.7,
        max_tokens: int = 4096,
    ) -> Dict[str, Any]:
        lc_messages = []
        for message in messages:
            role = message.get("role", "user")
            content = message.get("content", "")
            if role == "system":
                lc_messages.append(SystemMessage(content=content))
            elif role == "assistant":
                lc_messages.append(AIMessage(content=content))
            else:
                lc_messages.append(HumanMessage(content=content))

        response = await self.chat.ainvoke(
            lc_messages, temperature=temperature, max_tokens=max_tokens
        )
        return {
            "content": response.content,
            "model": self.model,
            "usage": {},
        }

    def count_tokens(self, text: str) -> int:
        return len(text.split())
```

### 2. Update `factory.py` - 添加 anthropic 分支

```python
from .anthropic_provider import LangChainAnthropicProvider

# 在 create() 方法里添加：
if provider_type == "anthropic":
    model = model_params.get("anthropic_model_name") or os.getenv("ANTHROPIC_MODEL")
    temperature = model_params.get("temperature")
    return LangChainAnthropicProvider(model=model, temperature=temperature)
```

### 3. Update `config.yaml`

```yaml
model_params:
  # ... existing
  anthropic_model_name: claude-sonnet-4-20250514
```

## Environment
`.env` 添加：
```
ANTHROPIC_API_KEY=your_key_here
```

## Acceptance Criteria
- [ ] `LangChainAnthropicProvider` 继承 `LLMProviderBase`
- [ ] `factory.py` 支持 `provider_type="anthropic"`
- [ ] 支持 async 调用
- [ ] 基本错误处理（key 无效）

## Notes
- 直接复用 `langchain_provider.py` 的结构
- LangChain 已有 `ChatAnthropic`，不需要直接调 Anthropic SDK
- Claude 在合规场景更可靠，Payroll Agent 将使用此 provider

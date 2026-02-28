"""Tests for the Anthropic (Claude) LLM provider."""

import asyncio
from unittest.mock import AsyncMock, MagicMock, patch

import pytest

from shared.llm.anthropic_provider import AnthropicProvider


class TestAnthropicProvider:
    """Unit tests for AnthropicProvider."""

    def test_init_raises_without_api_key(self, monkeypatch):
        monkeypatch.delenv("ANTHROPIC_API_KEY", raising=False)
        with pytest.raises(ValueError, match="ANTHROPIC_API_KEY must be set"):
            AnthropicProvider(api_key=None)

    @patch("shared.llm.anthropic_provider.ChatAnthropic")
    def test_init_uses_env_defaults(self, mock_chat_cls, monkeypatch):
        monkeypatch.setenv("ANTHROPIC_API_KEY", "test-key")
        monkeypatch.setenv("ANTHROPIC_MODEL", "claude-test-model")

        provider = AnthropicProvider()

        assert provider.model == "claude-test-model"
        assert provider.api_key == "test-key"
        mock_chat_cls.assert_called_once_with(
            model="claude-test-model",
            temperature=0.3,
            anthropic_api_key="test-key",
        )

    @patch("shared.llm.anthropic_provider.ChatAnthropic")
    def test_init_uses_explicit_params(self, mock_chat_cls, monkeypatch):
        monkeypatch.delenv("ANTHROPIC_API_KEY", raising=False)

        provider = AnthropicProvider(
            model="claude-custom",
            api_key="explicit-key",
            temperature=0.5,
        )

        assert provider.model == "claude-custom"
        assert provider.api_key == "explicit-key"
        mock_chat_cls.assert_called_once_with(
            model="claude-custom",
            temperature=0.5,
            anthropic_api_key="explicit-key",
        )

    @patch("shared.llm.anthropic_provider.ChatAnthropic")
    def test_generate_returns_expected_format(self, mock_chat_cls, monkeypatch):
        monkeypatch.setenv("ANTHROPIC_API_KEY", "test-key")

        mock_response = MagicMock()
        mock_response.content = "This is Claude's response."
        bound_mock = MagicMock()
        bound_mock.ainvoke = AsyncMock(return_value=mock_response)
        mock_chat = MagicMock()
        mock_chat.bind.return_value = bound_mock
        mock_chat_cls.return_value = mock_chat

        provider = AnthropicProvider()
        result = asyncio.run(
            provider.generate(
                [
                    {"role": "system", "content": "You are a helper."},
                    {"role": "user", "content": "Hello"},
                ]
            )
        )

        assert result["content"] == "This is Claude's response."
        assert result["model"] == "claude-sonnet-4-20250514"
        assert "usage" in result
        mock_chat.bind.assert_called_once()
        bound_mock.ainvoke.assert_called_once()

    @patch("shared.llm.anthropic_provider.ChatAnthropic")
    def test_count_tokens(self, mock_chat_cls, monkeypatch):
        monkeypatch.setenv("ANTHROPIC_API_KEY", "test-key")

        provider = AnthropicProvider()
        assert provider.count_tokens("one two three four") == 4
        assert provider.count_tokens("") == 0


class TestFactoryAnthropicBranch:
    """Test that the factory correctly creates an Anthropic provider."""

    @patch("shared.llm.anthropic_provider.ChatAnthropic")
    def test_factory_creates_anthropic_provider(self, mock_chat_cls, monkeypatch):
        monkeypatch.setenv("ANTHROPIC_API_KEY", "test-key")
        monkeypatch.setenv("LLM_PROVIDER", "anthropic")

        from shared.llm.factory import LLMProviderFactory

        provider = LLMProviderFactory.create()
        assert isinstance(provider, AnthropicProvider)

    @patch("shared.llm.anthropic_provider.ChatAnthropic")
    def test_factory_anthropic_reads_config_model(self, mock_chat_cls, monkeypatch):
        monkeypatch.setenv("ANTHROPIC_API_KEY", "test-key")

        from shared.llm.factory import LLMProviderFactory

        provider = LLMProviderFactory.create(provider_type="anthropic")
        assert isinstance(provider, AnthropicProvider)

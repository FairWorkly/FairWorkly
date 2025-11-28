using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Application.Compliance.Orchestrators;

public class ComplianceAiOrchestrator
{
    private readonly IAiClient _aiClient;

    public ComplianceAiOrchestrator(IAiClient aiClient)
    {
        _aiClient = aiClient;
    }

    /// <summary>
    /// 发送聊天请求给 AI (用于测试 Mock 机制是否生效)
    /// </summary>
    public async Task<string> ChatWithAiAsync(string message)
    {
        // 1. 构造请求 Payload (必须符合 Python 服务或 Mock 的预期)
        var requestPayload = new
        {
            message = message
        };

        // 2. 定义预期返回的结构 (对应 Python 的 ChatResponse)
        // 我们在 Application 层定义一个私有类来接数据，防止污染外部
        var response = await _aiClient.PostAsync<object, AiChatResponseDto>(
            "/chat", // 路由必须匹配 MockAiRouter 中的配置
            requestPayload
        );

        return response.Reply;
    }

    // 私有 DTO，仅用于反序列化 AI 的响应
    private class AiChatResponseDto
    {
        public string Reply { get; set; } = string.Empty;
    }
}
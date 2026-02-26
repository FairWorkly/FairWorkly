using System.Text.Json.Serialization;

namespace FairWorkly.Application.FairBot;

/// <summary>
/// Response from Agent Service for FairBot chat requests.
/// Maps snake_case JSON from Python Agent Service to C# PascalCase.
/// </summary>
public class FairBotChatResponse
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; init; }

    [JsonPropertyName("file_name")]
    public string? FileName { get; init; }

    [JsonPropertyName("routed_to")]
    public string? RoutedTo { get; init; }

    [JsonPropertyName("result")]
    public FairBotChatResult? Result { get; init; }
}

public class FairBotChatResult
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    [JsonPropertyName("file_name")]
    public string? FileName { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("sources")]
    public List<FairBotSource>? Sources { get; init; }

    [JsonPropertyName("note")]
    public string? Note { get; init; }
}

public class FairBotSource
{
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    [JsonPropertyName("page")]
    public int? Page { get; init; }

    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;
}

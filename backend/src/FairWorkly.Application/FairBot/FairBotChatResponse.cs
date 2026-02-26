using System.Text.Json.Serialization;

namespace FairWorkly.Application.FairBot;

/// <summary>
/// Response from Agent Service for FairBot chat requests.
/// Maps snake_case JSON from Python Agent Service to C# PascalCase.
/// </summary>
public class FairBotChatResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    [JsonPropertyName("routed_to")]
    public string? RoutedTo { get; set; }

    [JsonPropertyName("result")]
    public FairBotChatResult? Result { get; set; }
}

public class FairBotChatResult
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("sources")]
    public List<FairBotSource>? Sources { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }
}

public class FairBotSource
{
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

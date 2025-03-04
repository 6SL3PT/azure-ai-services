using Azure;

using System;
using System.Text.Json.Serialization;

namespace AzureAIServices.Application.Models;

public class AnalyzeResultResponseError {
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("innererror")]
    public ResponseError? InnerError { get; set; }
}

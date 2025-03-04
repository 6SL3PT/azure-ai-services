using System.Text.Json.Serialization;

namespace AzureAIServices.Application.Models;

public class AnalyzeResultResponse {
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = string.Empty;
    [JsonPropertyName("modelId")]
    public string ModelId { get; set; } = string.Empty;
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    [JsonPropertyName("fields")]
    public Dictionary<string, SlipFieldDto> Fields { get; set; } = [];
}

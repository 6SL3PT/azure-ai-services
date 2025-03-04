using System.Text.Json.Serialization;

namespace AzureAIServices.Application.Models;
public class SlipFieldDto {
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    [JsonPropertyName("confidence")]
    public float? Confidence { get; set; }
}
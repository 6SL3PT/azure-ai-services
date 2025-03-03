using System.Text.Json.Serialization;

namespace SlipIntelligence.Application.Models;

public class UriRequest {
    [JsonPropertyName("uriDocument")]
    public string UriDocument { get; set; } = string.Empty;
}

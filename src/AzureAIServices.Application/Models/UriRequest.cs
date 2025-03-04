using System.Text.Json.Serialization;

namespace AzureAIServices.Application.Models;

public class UriRequest {
    [JsonPropertyName("uriDocument")]
    public string UriDocument { get; set; } = string.Empty;
}

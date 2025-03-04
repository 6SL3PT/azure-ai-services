using System.Text.Json.Serialization;

namespace AzureAIServices.Application.Models;

public class Base64Request {
    [JsonPropertyName("base64Document")]
    public string Base64Document { get; set; } = string.Empty;
}

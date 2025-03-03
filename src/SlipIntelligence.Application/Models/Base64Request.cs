using System.Text.Json.Serialization;

namespace SlipIntelligence.Application.Models;

public class Base64Request {
    [JsonPropertyName("base64Document")]
    public string Base64Document { get; set; } = string.Empty;
}

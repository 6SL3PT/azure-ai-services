using System.Text.Json.Serialization;

namespace SlipIntelligence.Application.Models;

public class BytesRequest {
    [JsonPropertyName("bytesDocument")]
    public required byte[] BytesDocument { get; set; }
}

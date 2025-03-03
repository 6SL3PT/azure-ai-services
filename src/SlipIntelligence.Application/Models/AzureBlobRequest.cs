using System.Text.Json.Serialization;

namespace SlipIntelligence.Application.Models;

public class AzureBlobRequest {
    [JsonPropertyName("containerName")]
    public string ContainerName { get; set; } = string.Empty;
    [JsonPropertyName("blobName")]
    public string BlobName { get; set; } = string.Empty;
}

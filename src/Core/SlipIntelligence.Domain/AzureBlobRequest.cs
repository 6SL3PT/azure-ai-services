namespace SlipIntelligence.Domain;

public class AzureBlobRequest
{
    public string ContainerName { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
}

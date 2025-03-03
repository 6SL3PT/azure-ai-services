using Azure.Storage.Blobs;

using SlipIntelligence.Infrastructure.Interfaces;

namespace SlipIntelligence.Infrastructure.Clients;

public class AzureBlobClient: IAzureBlobClient {
    private readonly BlobServiceClient _client;

    public AzureBlobClient(BlobServiceClient client) {
        _client = client;
    }

    public async Task<Stream> GetBlobStreamAsync(string containerName, string blobName) {
        var containerClient = _client.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var downloadInfo = await blobClient.DownloadAsync();
        return downloadInfo.Value.Content;
    }
}

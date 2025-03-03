using Azure.Storage.Blobs;

using SlipIntelligence.Infrastructure.Interfaces;

namespace SlipIntelligence.Infrastructure.Clients;

public class AzureBlobClient: IAzureBlobClient {
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobClient(string connectionString) {
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<Stream> GetBlobStreamAsync(string containerName, string blobName) {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var downloadInfo = await blobClient.DownloadAsync();
        return downloadInfo.Value.Content;
    }
}

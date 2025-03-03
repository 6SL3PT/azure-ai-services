using Azure;
using Azure.Storage.Blobs;

using SlipIntelligence.Infrastructure.Interfaces;

using System;
using System.IO;
using System.Threading.Tasks;

namespace SlipIntelligence.Infrastructure.Clients;

public class AzureBlobClient: IAzureBlobClient {
    private readonly BlobServiceClient _client;

    public AzureBlobClient(string connectionString) {
        _client = new BlobServiceClient(connectionString);
    }

    public async Task<Stream> GetBlobStreamAsync(string containerName, string blobName) {
        var containerClient = _client.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var downloadInfo = await blobClient.DownloadAsync();
        return downloadInfo.Value.Content;
    }
}

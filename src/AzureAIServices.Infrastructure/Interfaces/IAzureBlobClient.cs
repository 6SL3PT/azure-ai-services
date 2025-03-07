﻿namespace AzureAIServices.Infrastructure.Interfaces;

public interface IAzureBlobClient {
    Task<Stream> GetBlobStreamAsync(string containerName, string blobName);
}

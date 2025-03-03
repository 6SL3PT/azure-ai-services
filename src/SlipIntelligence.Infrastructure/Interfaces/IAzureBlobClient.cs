using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace SlipIntelligence.Infrastructure.Interfaces;

public interface IAzureBlobClient
{
    Task<Stream> GetBlobStreamAsync(string containerName, string blobName);
}


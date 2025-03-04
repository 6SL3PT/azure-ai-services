using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Moq;

using AzureAIServices.Infrastructure.Clients;

using System.Text;

namespace AzureAIServices.Tests.AzureAIServices;
public class AzureBlobClientTest {
    private readonly Mock<BlobServiceClient> _mockServiceClient;

    public AzureBlobClientTest() {
        _mockServiceClient = new Mock<BlobServiceClient>();
    }

    [Fact]
    public async Task GetBlobStreamAsync_ReturnsExpectedResponse() {
        // Arrange
        string blobContent = "this is test data";
        BlobDownloadInfo downloadResult = BlobsModelFactory.BlobDownloadInfo(
            content: new MemoryStream(Encoding.UTF8.GetBytes(blobContent)));
        Response<BlobDownloadInfo> response = Response.FromValue(downloadResult, new Mock<Response>().Object);

        Mock<BlobClient> mockBlobClient = new();
        mockBlobClient.Setup(client => client.DownloadAsync())
            .Returns(Task.FromResult(response));

        _mockServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(new Mock<BlobContainerClient>().Object);
        _mockServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())
            .GetBlobClient(It.IsAny<string>()))
            .Returns(mockBlobClient.Object);

        AzureBlobClient azureBlobClient = new(_mockServiceClient.Object);

        // Act
        var result = await azureBlobClient.GetBlobStreamAsync(It.IsAny<string>(), It.IsAny<string>());

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Stream>(result);

        StreamReader reader = new(result);
        Assert.Equal(blobContent, reader.ReadToEnd());
    }
}

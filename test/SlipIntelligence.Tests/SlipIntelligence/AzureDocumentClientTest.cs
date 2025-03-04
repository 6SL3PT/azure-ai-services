using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

using Moq;

using SlipIntelligence.Infrastructure.Clients;

namespace SlipIntelligence.Tests.SlipIntelligence;

public class AzureDocumentClientTest {
    private readonly Mock<DocumentAnalysisClient> _mockAnalysisClient;

    public AzureDocumentClientTest() {
        _mockAnalysisClient = new Mock<DocumentAnalysisClient>();
    }

    [Fact]
    public async Task AnalyzeDocumentStreamAsync_ReturnsExpectedResponse() {
        // Arrange
        string fakeModelId = Guid.NewGuid().ToString();

        Mock<AnalyzeDocumentOperation> mockOperation = new();
        SetupOperation(mockOperation, fakeModelId);
        _mockAnalysisClient.Setup(client => client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                fakeModelId,
                It.IsAny<Stream>(),
                It.IsAny<AnalyzeDocumentOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(mockOperation.Object));

        AzureDocumentClient azureDocumentClient = new(_mockAnalysisClient.Object);

        // Act
        var response = await azureDocumentClient.AnalyzeDocumentStreamAsync(It.IsAny<Stream>(), fakeModelId);

        // Assert
        Assert.IsType<AnalyzeResult>(response);
        Assert.NotNull(response);
        Assert.NotNull(response.Documents);
        Assert.Equal(fakeModelId, response.ModelId);
    }

    [Fact]
    public async Task AnalyzeDocumentUriAsync_ReturnsExpectedResponse() {
        // Arrange
        string fakeModelId = Guid.NewGuid().ToString();

        Mock<AnalyzeDocumentOperation> mockOperation = new();
        SetupOperation(mockOperation, fakeModelId);
        _mockAnalysisClient.Setup(client => client.AnalyzeDocumentFromUriAsync(
                WaitUntil.Completed,
                fakeModelId,
                It.IsAny<Uri>(),
                It.IsAny<AnalyzeDocumentOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(mockOperation.Object));

        AzureDocumentClient azureDocumentClient = new(_mockAnalysisClient.Object);

        // Act
        var response = await azureDocumentClient.AnalyzeDocumentUriAsync(It.IsAny<Uri>(), fakeModelId);

        // Assert
        Assert.IsType<AnalyzeResult>(response);
        Assert.NotNull(response);
        Assert.NotNull(response.Documents);
        Assert.Equal(fakeModelId, response.ModelId);
    }

    private static void SetupOperation(Mock<AnalyzeDocumentOperation> mockOperation, string fakeModelId) {
        AnalyzedDocument document = DocumentAnalysisModelFactory.AnalyzedDocument($"custom:{fakeModelId}");
        List<AnalyzedDocument> documents = [document];
        AnalyzeResult result = DocumentAnalysisModelFactory.AnalyzeResult(fakeModelId, documents: documents);

        mockOperation.SetupGet(op => op.Value)
            .Returns(result);
    }
}

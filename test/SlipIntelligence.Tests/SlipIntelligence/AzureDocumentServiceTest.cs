using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Storage.Blobs;

using Moq;

using SlipIntelligence.Application.Extensions;
using SlipIntelligence.Application.Models;
using SlipIntelligence.Application.Services;
using SlipIntelligence.Infrastructure.Clients;

using System.Drawing;

using Xunit;

namespace SlipIntelligence.Tests.SlipIntelligence;
public class AzureDocumentServiceTest {
    private readonly Mock<DocumentAnalysisClient> _mockAnalysisClient;
    private readonly Mock<BlobServiceClient> _mockBlobServiceClient;
    public AzureDocumentServiceTest() {
        _mockAnalysisClient = new Mock<DocumentAnalysisClient>();
        _mockBlobServiceClient = new Mock<BlobServiceClient>();
    }

    [Fact]
    public async Task AnalyzeDocumentBase64Async_ReturnsExpectedResponse() {
        // Arrange
        string fakeModelId = Guid.NewGuid().ToString();
        Base64Request fakeRequest = new() { Base64Document = "test" };

        Mock<AnalyzeDocumentOperation> mockOperation = new();
        SetupDocumentOperation(mockOperation, fakeModelId);
        _mockAnalysisClient.Setup(client => client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                fakeModelId,
                It.IsAny<Stream>(),
                It.IsAny<AnalyzeDocumentOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(mockOperation.Object));

        AzureDocumentService azureDocumentService = new(
            new AzureDocumentClient(_mockAnalysisClient.Object),
            new AzureBlobClient(_mockBlobServiceClient.Object));

        // Act
        var response = await azureDocumentService.AnalyzeDocumentBase64Async(fakeRequest, fakeModelId);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<ResponseMessage<AnalyzeResultResponse>>(response);
        Console.WriteLine(response.Data.ToString());
    }

    private static void SetupDocumentOperation(Mock<AnalyzeDocumentOperation> mockOperation, string fakeModelId) {
        var fieldValue = DocumentAnalysisModelFactory.DocumentFieldValueWithDoubleFieldType(150.0);

        var fieldPolygon = new List<PointF>() {
            new(1f, 2f), new(2f, 2f), new(2f, 1f), new(1f, 1f)
        };
        var fieldRegion = DocumentAnalysisModelFactory.BoundingRegion(1, fieldPolygon);
        var fieldRegions = new List<BoundingRegion>() { fieldRegion };

        var fieldSpans = new List<DocumentSpan>() {
            DocumentAnalysisModelFactory.DocumentSpan(25, 32)
        };

        var field = DocumentAnalysisModelFactory.DocumentField(DocumentFieldType.Double, fieldValue, "150.00", fieldRegions, fieldSpans, confidence: 0.85f);
        var fields = new Dictionary<string, DocumentField> {
            { "amount", field }
        };

        var documentPolygon = new List<PointF>() {
            new(0f, 10f), new(10f, 10f), new(10f, 0f), new(0f, 0f)
        };
        var documentRegion = DocumentAnalysisModelFactory.BoundingRegion(1, documentPolygon);
        var documentRegions = new List<BoundingRegion>() { documentRegion };

        var documentSpans = new List<DocumentSpan>() {
            DocumentAnalysisModelFactory.DocumentSpan(0, 105)
        };

        var document = DocumentAnalysisModelFactory.AnalyzedDocument($"custom:{fakeModelId}", documentRegions, documentSpans, fields, 0.95f);
        var documents = new List<AnalyzedDocument>() { document };

        var result = DocumentAnalysisModelFactory.AnalyzeResult(fakeModelId, documents: documents);

        mockOperation.SetupGet(op => op.Value).Returns(result);
    }
}
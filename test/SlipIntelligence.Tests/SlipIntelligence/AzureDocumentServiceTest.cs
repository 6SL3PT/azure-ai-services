using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

using Microsoft.AspNetCore.Http;

using Moq;

using SlipIntelligence.Application.Extensions;
using SlipIntelligence.Application.Models;
using SlipIntelligence.Application.Services;
using SlipIntelligence.Infrastructure.Interfaces;

using System.Drawing;
using System.Text;

namespace SlipIntelligence.Tests.SlipIntelligence;
public class AzureDocumentServiceTest {
    private readonly Mock<IAzureDocumentClient> _mockDocumentClient;
    private readonly Mock<IAzureBlobClient> _mockBlobClient;
    private readonly AzureDocumentService _service;

    public AzureDocumentServiceTest() {
        _mockDocumentClient = new Mock<IAzureDocumentClient>();
        _mockBlobClient = new Mock<IAzureBlobClient>();
        _service = new AzureDocumentService(_mockDocumentClient.Object, _mockBlobClient.Object);
    }

    [Fact]
    public async Task AnalyzeDocumentBase64Async_ShouldReturnSuccessResponse_WhenAnalysisIsSuccessful() {
        // Arrange
        var base64Document = Convert.ToBase64String(new byte[] { 0, 1, 2 });
        var request = new Base64Request { Base64Document = base64Document };
        var modelId = "test-model-id";

        SetupDocumentClientMockForStream(_mockDocumentClient, modelId);

        // Act
        var result = await _service.AnalyzeDocumentBase64Async(request, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);

        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(modelId, response.ModelId);
        Assert.Single(response.Fields);
        Assert.Equal("150.00", response.Fields["amount"].Content);
        Assert.Equal(0.85f, response.Fields["amount"].Confidence);
    }

    [Fact]
    public async Task AnalyzeDocumentBase64Async_ShouldReturnErrorResponse_WhenRequestFailedExceptionIsThrown() {
        // Arrange
        var base64Document = Convert.ToBase64String(new byte[] { 0, 1, 2 });
        var request = new Base64Request { Base64Document = base64Document };
        var modelId = "test-model-id";

        _mockDocumentClient.Setup(client => client.AnalyzeDocumentStreamAsync(It.IsAny<Stream>(), modelId))
            .ThrowsAsync(new RequestFailedException("Error message"));

        // Act
        var result = await _service.AnalyzeDocumentBase64Async(request, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Error while sending request to Azure Document Intelligence", response.Content);
    }

    [Fact]
    public async Task AnalyzeDocumentBytesAsync_ShouldReturnSuccessResponse_WhenAnalysisIsSuccessful() {
        // Arrange
        var modelId = "test-model-id";
        var formFileMock = new Mock<IFormFile>();
        var memoryStream = new MemoryStream(new byte[] { 0, 1, 2 });
        formFileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);

        SetupDocumentClientMockForStream(_mockDocumentClient, modelId);

        // Act
        var result = await _service.AnalyzeDocumentBytesAsync(formFileMock.Object, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(modelId, response.ModelId);
        Assert.Single(response.Fields);
        Assert.Equal("150.00", response.Fields["amount"].Content);
        Assert.Equal(0.85f, response.Fields["amount"].Confidence);
    }

    [Fact]
    public async Task AnalyzeDocumentBytesAsync_ShouldReturnErrorResponse_WhenRequestFailedExceptionIsThrown() {
        // Arrange
        var modelId = "test-model-id";
        var formFileMock = new Mock<IFormFile>();
        var memoryStream = new MemoryStream(new byte[] { 0, 1, 2 });
        formFileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);

        _mockDocumentClient.Setup(client => client.AnalyzeDocumentStreamAsync(It.IsAny<Stream>(), modelId))
            .ThrowsAsync(new RequestFailedException("Error message"));

        // Act
        var result = await _service.AnalyzeDocumentBytesAsync(formFileMock.Object, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Error while sending request to Azure Document Intelligence", response.Content);
    }

    [Fact]
    public async Task AnalyzeDocumentUriAsync_ShouldReturnSuccessResponse_WhenAnalysisIsSuccessful() {
        // Arrange
        var modelId = "test-model-id";
        var request = new UriRequest { UriDocument = "https://example.com/document" };

        SetupDocumentClientMockForUri(_mockDocumentClient, modelId);

        // Act
        var result = await _service.AnalyzeDocumentUriAsync(request, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(modelId, response.ModelId);
        Assert.Single(response.Fields);
        Assert.Equal("150.00", response.Fields["amount"].Content);
        Assert.Equal(0.85f, response.Fields["amount"].Confidence);
    }

    [Fact]
    public async Task AnalyzeDocumentUriAsync_ShouldReturnErrorResponse_WhenRequestFailedExceptionIsThrown() {
        // Arrange
        var modelId = "test-model-id";
        var request = new UriRequest { UriDocument = "https://example.com/document" };

        _mockDocumentClient.Setup(client => client.AnalyzeDocumentUriAsync(It.IsAny<Uri>(), modelId))
            .ThrowsAsync(new RequestFailedException("Error message"));

        // Act
        var result = await _service.AnalyzeDocumentUriAsync(request, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Error while sending request to Azure Document Intelligence", response.Content);
    }

    [Fact]
    public async Task AnalyzeDocumentAzureBlobAsync_ShouldReturnErrorResponse_WhenGetBlobStreamThrowsRequestFailedException() {
        // Arrange
        var request = new AzureBlobRequest { ContainerName = "test-container", BlobName = "test-blob" };
        var modelId = "test-model-id";

        string expectedErrorMessage = "Blob fetch error";
        _mockBlobClient.Setup(client => client.GetBlobStreamAsync(request.ContainerName, request.BlobName))
            .ThrowsAsync(new RequestFailedException(expectedErrorMessage));

        // Act
        var result = await _service.AnalyzeDocumentAzureBlobAsync(request, modelId);

        // Assert
        Assert.IsType<ResponseMessage<AnalyzeResultResponse>>(result);
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains(expectedErrorMessage, response.Content);
    }

    [Fact]
    public async Task AnalyzeDocumentAzureBlobAsync_ShouldReturnErrorResponse_WhenGetBlobStreamThrowsException() {
        // Arrange
        var request = new AzureBlobRequest { ContainerName = "test-container", BlobName = "test-blob" };
        var modelId = "test-model-id";

        string expectedErrorMessage = "Blob fetch error";
        _mockBlobClient.Setup(client => client.GetBlobStreamAsync(request.ContainerName, request.BlobName))
            .ThrowsAsync(new Exception(expectedErrorMessage));

        // Act
        var result = await _service.AnalyzeDocumentAzureBlobAsync(request, modelId);

        // Assert
        Assert.IsType<ResponseMessage<AnalyzeResultResponse>>(result);
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains(expectedErrorMessage, response.Content);
    }

    [Fact]
    public async Task AnalyzeDocumentAzureBlobAsync_ShouldReturnSuccessResponse_WhenBlobStreamIsProcessedSuccessfully() {
        // Arrange
        var modelId = "test-model-id";
        var request = new AzureBlobRequest {
            ContainerName = "test-container",
            BlobName = "test-blob"
        };

        // Mock the blob stream to return a valid stream
        var mockBlobStream = new MemoryStream(Encoding.UTF8.GetBytes("sample document content"));
        _mockBlobClient.Setup(client => client.GetBlobStreamAsync(request.ContainerName, request.BlobName))
            .ReturnsAsync(mockBlobStream);

        // Mock the document client to return a successful AnalyzeResult
        var analyzeResult = CreateAnalyzeResult(modelId);
        _mockDocumentClient.Setup(client => client.AnalyzeDocumentStreamAsync(It.IsAny<Stream>(), modelId))
            .ReturnsAsync(analyzeResult);

        // Act
        var result = await _service.AnalyzeDocumentAzureBlobAsync(request, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(modelId, response.ModelId);
        Assert.Single(response.Fields);
        Assert.Equal("150.00", response.Fields["amount"].Content);
        Assert.Equal(0.85f, response.Fields["amount"].Confidence);
    }

    [Fact]
    public async Task AnalyzeDocumentAzureBlobAsync_ShouldReturnErrorResponse_WhenBlobStreamProcessedFailed() {
        // Arrange
        var modelId = "test-model-id";
        var request = new AzureBlobRequest {
            ContainerName = "test-container",
            BlobName = "test-blob"
        };

        // Mock the blob stream to return a valid stream
        var mockBlobStream = new MemoryStream(Encoding.UTF8.GetBytes("sample document content"));
        _mockBlobClient.Setup(client => client.GetBlobStreamAsync(request.ContainerName, request.BlobName))
            .ReturnsAsync(mockBlobStream);

        // Mock the document client to throw Exception
        var analyzeResult = CreateAnalyzeResult(modelId);
        _mockDocumentClient.Setup(client => client.AnalyzeDocumentStreamAsync(It.IsAny<Stream>(), modelId))
            .ThrowsAsync(new RequestFailedException("Error message"));

        // Act
        var result = await _service.AnalyzeDocumentAzureBlobAsync(request, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Error while sending request to Azure Document Intelligence", response.Content);
    }


    [Fact]
    public async Task ConvertAnalyzeResultToResponse_ShouldReturnErrorResponse_WhenNoDocumentsFound() {
        // Arrange
        var base64Document = Convert.ToBase64String(new byte[] { 0, 1, 2 });
        var request = new Base64Request { Base64Document = base64Document };
        var modelId = "test-model-id";
        var serviceVersion = "1.0";

        // Create a response with no documents
        var analyzeResult = DocumentAnalysisModelFactory.AnalyzeResult(modelId, documents: [], serviceVersion: serviceVersion);
        _mockDocumentClient.Setup(client => client.AnalyzeDocumentStreamAsync(It.IsAny<Stream>(), modelId))
            .ReturnsAsync(analyzeResult);

        // Act
        var result = await _service.AnalyzeDocumentBase64Async(request, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal(modelId, response.ModelId);
        Assert.Equal("Model did not found any document.", response.Content);
        Assert.Equal(serviceVersion, response.ApiVersion);
    }

    [Fact]
    public async Task ConvertAnalyzeResultToResponse_ShouldReturnErrorResponse_WhenNoDocumentsIsNull() {
        // Arrange
        var base64Document = Convert.ToBase64String(new byte[] { 0, 1, 2 });
        var request = new Base64Request { Base64Document = base64Document };
        var modelId = "test-model-id";
        var serviceVersion = "1.0";

        // Create a response with no documents
        var analyzeResult = DocumentAnalysisModelFactory.AnalyzeResult(modelId, documents: null, serviceVersion: serviceVersion);
        _mockDocumentClient.Setup(client => client.AnalyzeDocumentStreamAsync(It.IsAny<Stream>(), modelId))
            .ReturnsAsync(analyzeResult);

        // Act
        var result = await _service.AnalyzeDocumentBase64Async(request, modelId);

        // Assert
        Assert.Equal(1000, result.Status.Code);
        var response = result.Data as AnalyzeResultResponse;
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal(modelId, response.ModelId);
        Assert.Equal("Model did not found any document.", response.Content);
        Assert.Equal(serviceVersion, response.ApiVersion);
    }

    private static AnalyzeResult CreateAnalyzeResult(string fakeModelId) {
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

        return DocumentAnalysisModelFactory.AnalyzeResult(fakeModelId, documents: documents);
    }

    private static void SetupDocumentClientMockForUri(Mock<IAzureDocumentClient> mockDocumentClient, string modelId) {
        var analyzeResult = CreateAnalyzeResult(modelId);
        mockDocumentClient.Setup(client => client.AnalyzeDocumentUriAsync(It.IsAny<Uri>(), modelId))
            .ReturnsAsync(analyzeResult);
    }

    private static void SetupDocumentClientMockForStream(Mock<IAzureDocumentClient> mockDocumentClient, string modelId) {
        var analyzeResult = CreateAnalyzeResult(modelId);
        mockDocumentClient.Setup(client => client.AnalyzeDocumentStreamAsync(It.IsAny<Stream>(), modelId))
            .ReturnsAsync(analyzeResult);
    }
}
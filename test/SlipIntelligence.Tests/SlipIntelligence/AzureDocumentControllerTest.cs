using Xunit;
using SlipIntelligence.Api.Controllers;
using Moq;
using SlipIntelligence.Application.Contracts;
using SlipIntelligence.Application.Models;
using SlipIntelligence.Application.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SlipIntelligence.Tests.SlipIntelligence;

public class AzureDocumentControllerTest {
    private readonly Mock<IAzureDocumentService> _mockService;
    private readonly AzureDocumentController _azureDocumentController;

    public AzureDocumentControllerTest() {
        _mockService = new Mock<IAzureDocumentService>();
        _azureDocumentController = new AzureDocumentController(_mockService.Object);
    }

    private ResponseMessage<AnalyzeResultResponse> CreateAnalyzeResultResponse(bool success = true) {
        var analyzeResponse = new AnalyzeResultResponse {
            Success = success,
            ApiVersion = "1.0",
            ModelId = "test-model-id",
            Content = "Extracted content",
            Fields = new Dictionary<string, SlipFieldDto> {
                { "field1", new SlipFieldDto { Content = "Field 1", Confidence = 0.97f } },
                { "field2", new SlipFieldDto { Content = "Field 2", Confidence = 0.5f } }
            }
        };
        return new ResponseMessage<AnalyzeResultResponse>(analyzeResponse);
    }

    #region TextExtractFromBase64 Tests

    [Fact]
    public async Task TextExtractFromBase64_ValidRequest_ReturnsOkResult() {
        // Arrange
        var modelId = "test-model-id";
        var request = new Base64Request { Base64Document = "validBase64String" };
        var expectedResponse = CreateAnalyzeResultResponse();

        _mockService.Setup(s => s.AnalyzeDocumentBase64Async(request, modelId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _azureDocumentController.TextExtractFromBase64(modelId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task TextExtractFromBase64_NoDocument_ReturnsBadRequest() {
        // Arrange
        var modelId = "test-model-id";
        var request = new Base64Request { Base64Document = "" };

        // Act
        var result = await _azureDocumentController.TextExtractFromBase64(modelId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("No document provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task TextExtractFromBase64_InternalServerError_Returns500() {
        // Arrange
        var modelId = "test-model-id";
        var request = new Base64Request { Base64Document = "validBase64String" };
        _mockService.Setup(s => s.AnalyzeDocumentBase64Async(request, modelId))
            .ThrowsAsync(new Exception("Internal error"));

        // Act
        var result = await _azureDocumentController.TextExtractFromBase64(modelId, request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error: Internal error", statusCodeResult.Value);
    }

    #endregion

    #region TextExtractFromBytes Tests

    [Fact]
    public async Task TextExtractFromBytes_ValidRequest_ReturnsOkResult() {
        // Arrange
        var modelId = "test-model-id";
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(10);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[10]));
        var expectedResponse = CreateAnalyzeResultResponse();

        _mockService.Setup(s => s.AnalyzeDocumentBytesAsync(mockFile.Object, modelId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _azureDocumentController.TextExtractFromBytes(modelId, mockFile.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task TextExtractFromBytes_NoFile_ReturnsBadRequest() {
        // Arrange
        var modelId = "test-model-id";

        // Act
        var result = await _azureDocumentController.TextExtractFromBytes(modelId, null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("No document provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task TextExtractFromBytes_EmptyFile_ReturnsBadRequest() {
        // Arrange
        var modelId = "test-model-id";
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);

        // Act
        var result = await _azureDocumentController.TextExtractFromBytes(modelId, mockFile.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("No document provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task TextExtractFromBytes_InternalServerError_Returns500() {
        // Arrange
        var modelId = "test-model-id";
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(10);
        _mockService.Setup(s => s.AnalyzeDocumentBytesAsync(mockFile.Object, modelId))
            .ThrowsAsync(new Exception("Internal error"));

        // Act
        var result = await _azureDocumentController.TextExtractFromBytes(modelId, mockFile.Object);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error: Internal error", statusCodeResult.Value);
    }

    #endregion

    #region TextExtractFromUri Tests

    [Fact]
    public async Task TextExtractFromUri_ValidRequest_ReturnsOkResult() {
        // Arrange
        var modelId = "test-model-id";
        var request = new UriRequest { UriDocument = "http://example.com/document" };
        var expectedResponse = CreateAnalyzeResultResponse();

        _mockService.Setup(s => s.AnalyzeDocumentUriAsync(request, modelId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _azureDocumentController.TextExtractFromUri(modelId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task TextExtractFromUri_NoUri_ReturnsBadRequest() {
        // Arrange
        var modelId = "test-model-id";
        var request = new UriRequest { UriDocument = "" };

        // Act
        var result = await _azureDocumentController.TextExtractFromUri(modelId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("No URI provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task TextExtractFromUri_InternalServerError_Returns500() {
        // Arrange
        var modelId = "test-model-id";
        var request = new UriRequest { UriDocument = "http://example.com/document" };
        _mockService.Setup(s => s.AnalyzeDocumentUriAsync(request, modelId))
            .ThrowsAsync(new Exception("Internal error"));

        // Act
        var result = await _azureDocumentController.TextExtractFromUri(modelId, request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error: Internal error", statusCodeResult.Value);
    }

    #endregion

    #region TextExtractFromAzureBlob Tests

    [Fact]
    public async Task TextExtractFromAzureBlob_ValidRequest_ReturnsOkResult() {
        // Arrange
        var modelId = "test-model-id";
        var request = new AzureBlobRequest { ContainerName = "container", BlobName = "blob" };
        var expectedResponse = CreateAnalyzeResultResponse();

        _mockService.Setup(s => s.AnalyzeDocumentAzureBlobAsync(request, modelId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _azureDocumentController.TextExtractFromAzureBlob(modelId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task TextExtractFromAzureBlob_NoContainerName_ReturnsBadRequest() {
        // Arrange
        var modelId = "test-model-id";
        var request = new AzureBlobRequest { ContainerName = "", BlobName = "blob" };

        // Act
        var result = await _azureDocumentController.TextExtractFromAzureBlob(modelId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("No container name provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task TextExtractFromAzureBlob_NoBlobName_ReturnsBadRequest() {
        // Arrange
        var modelId = "test-model-id";
        var request = new AzureBlobRequest { ContainerName = "container", BlobName = "" };

        // Act
        var result = await _azureDocumentController.TextExtractFromAzureBlob(modelId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("No blob name provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task TextExtractFromAzureBlob_InternalServerError_ReturnsBadRequest() {
        // Arrange
        var modelId = "test-model-id";
        var request = new AzureBlobRequest { ContainerName = "container", BlobName = "blob" };
        _mockService.Setup(s => s.AnalyzeDocumentAzureBlobAsync(request, modelId))
            .ThrowsAsync(new Exception("Internal error"));

        // Act
        var result = await _azureDocumentController.TextExtractFromAzureBlob(modelId, request);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal("Failed to process invoice: Internal error", badRequestResult.Value);
    }

    #endregion
}

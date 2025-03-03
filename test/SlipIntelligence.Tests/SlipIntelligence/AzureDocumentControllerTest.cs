using Xunit;
using SlipIntelligence.Api.Controllers;
using Moq;
using SlipIntelligence.Application.Contracts;
using SlipIntelligence.Application.Models;
using SlipIntelligence.Application.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace SlipIntelligence.Tests.SlipIntelligence;

public class AzureDocumentControllerTest {
    private readonly Mock<IAzureDocumentService> _mockAzureDocumentService;
    private readonly AzureDocumentController _azureDocumentController;

    public AzureDocumentControllerTest() {
        _mockAzureDocumentService = new Mock<IAzureDocumentService>();
        _azureDocumentController = new AzureDocumentController(_mockAzureDocumentService.Object);
    }

    [Fact]
    public async Task TextExtractFromBase64_ValidRequest_ReturnsOkResult() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new Base64Request { Base64Document = "JVBERi0xLjMKJcfs..." };
        var responseMessage = new ResponseMessage<AnalyzeResultResponse>(new AnalyzeResultResponse { Success = true });
        _mockAzureDocumentService
            .Setup(service => service.AnalyzeDocumentBase64Async(request, modelId))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _azureDocumentController.TextExtractFromBase64(modelId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(responseMessage, okResult.Value);
    }

    [Fact]
    public async Task TextExtractFromBase64_InvalidRequest_ReturnsBadRequest() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new Base64Request { Base64Document = "" };

        // Act
        var result = await _azureDocumentController.TextExtractFromBase64(modelId, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task TextExtractFromBase64_Exception_ReturnsInternalServerError() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new Base64Request { Base64Document = "JVBERi0xLjMKJcfs..." };
        var exceptionMessage = "Something went wrong";
        _mockAzureDocumentService
            .Setup(service => service.AnalyzeDocumentBase64Async(request, modelId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _azureDocumentController.TextExtractFromBase64(modelId, request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal($"Internal server error: {exceptionMessage}", statusCodeResult.Value);
    }

    [Fact]
    public async Task TextExtractFromBytes_ValidRequest_ReturnsOkResult() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var fileMock = new Mock<IFormFile>();
        var documentStream = new MemoryStream(Encoding.UTF8.GetBytes("Sample document content"));
        fileMock.Setup(f => f.OpenReadStream()).Returns(documentStream);
        fileMock.Setup(f => f.Length).Returns(documentStream.Length);
        var document = fileMock.Object;
        var responseMessage = new ResponseMessage<AnalyzeResultResponse>(new AnalyzeResultResponse { Success = true });
        _mockAzureDocumentService
            .Setup(service => service.AnalyzeDocumentBytesAsync(document, modelId))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _azureDocumentController.TextExtractFromBytes(modelId, document);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(responseMessage, okResult.Value);
    }

    [Fact]
    public async Task TextExtractFromBytes_InvalidRequest_ReturnsBadRequest() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var document = null as IFormFile;

        // Act
        var result = await _azureDocumentController.TextExtractFromBytes(modelId, document);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task TextExtractFromBytes_Exception_ReturnsInternalServerError() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var fileMock = new Mock<IFormFile>();
        var documentStream = new MemoryStream(Encoding.UTF8.GetBytes("Sample document content"));
        fileMock.Setup(f => f.OpenReadStream()).Returns(documentStream);
        fileMock.Setup(f => f.Length).Returns(documentStream.Length);
        var document = fileMock.Object;
        var exceptionMessage = "Something went wrong";
        _mockAzureDocumentService
            .Setup(service => service.AnalyzeDocumentBytesAsync(document, modelId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _azureDocumentController.TextExtractFromBytes(modelId, document);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal($"Internal server error: {exceptionMessage}", statusCodeResult.Value);
    }

    [Fact]
    public async Task TextExtractFromUri_ValidRequest_ReturnsOkResult() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new UriRequest { UriDocument = "https://example.com/sample-document.pdf" };
        var responseMessage = new ResponseMessage<AnalyzeResultResponse>(new AnalyzeResultResponse { Success = true });
        _mockAzureDocumentService
            .Setup(service => service.AnalyzeDocumentUriAsync(request, modelId))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _azureDocumentController.TextExtractFromUri(modelId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(responseMessage, okResult.Value);
    }

    [Fact]
    public async Task TextExtractFromUri_InvalidRequest_ReturnsBadRequest() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new UriRequest { UriDocument = "" };

        // Act
        var result = await _azureDocumentController.TextExtractFromUri(modelId, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task TextExtractFromUri_Exception_ReturnsInternalServerError() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new UriRequest { UriDocument = "https://example.com/sample-document.pdf" };
        var exceptionMessage = "Something went wrong";
        _mockAzureDocumentService
            .Setup(service => service.AnalyzeDocumentUriAsync(request, modelId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _azureDocumentController.TextExtractFromUri(modelId, request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal($"Internal server error: {exceptionMessage}", statusCodeResult.Value);
    }

    [Fact]
    public async Task TextExtractFromAzureBlob_ValidRequest_ReturnsOkResult() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new AzureBlobRequest { ContainerName = "sample-container", BlobName = "sample-document.pdf" };
        var responseMessage = new ResponseMessage<AnalyzeResultResponse>(new AnalyzeResultResponse { Success = true });
        _mockAzureDocumentService
            .Setup(service => service.AnalyzeDocumentAzureBlobAsync(request, modelId))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _azureDocumentController.TextExtractFromAzureBlob(modelId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(responseMessage, okResult.Value);
    }

    [Fact]
    public async Task TextExtractFromAzureBlob_NoContainerName_ReturnsBadRequest() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new AzureBlobRequest { ContainerName = "", BlobName = "sample-document.pdf" };

        // Act
        var result = await _azureDocumentController.TextExtractFromAzureBlob(modelId, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task TextExtractFromAzureBlob_NoBlobName_ReturnsBadRequest() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new AzureBlobRequest { ContainerName = "sample-container", BlobName = "" };

        // Act
        var result = await _azureDocumentController.TextExtractFromAzureBlob(modelId, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task TextExtractFromAzureBlob_Exception_ReturnsBadRequest() {
        // Arrange
        var modelId = "prebuilt-invoice";
        var request = new AzureBlobRequest { ContainerName = "sample-container", BlobName = "sample-document.pdf" };
        var exceptionMessage = "Something went wrong";
        _mockAzureDocumentService
            .Setup(service => service.AnalyzeDocumentAzureBlobAsync(request, modelId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _azureDocumentController.TextExtractFromAzureBlob(modelId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal($"Failed to process invoice: {exceptionMessage}", badRequestResult.Value);
    }
}
using Microsoft.AspNetCore.Mvc;

using SlipIntelligence.Application.Contracts;
using SlipIntelligence.Application.Extensions;
using SlipIntelligence.Application.Models;

namespace SlipIntelligence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AzureDocumentController: ControllerBase {
    private readonly IAzureDocumentService azureDocumentService;

    public AzureDocumentController(IAzureDocumentService azureDocumentService) {
        this.azureDocumentService = azureDocumentService;
    }

    [HttpPost("base64/{modelId}")]
    public async Task<ActionResult<ResponseMessage<AnalyzeResultResponse>>> TextExtractFromBase64(string modelId, [FromBody] Base64Request request) {
        if(string.IsNullOrEmpty(request.Base64Document))
            return BadRequest("No document provided.");

        try {
            var result = await azureDocumentService.AnalyzeDocumentBase64Async(request, modelId);
            return Ok(result);
        } catch(Exception ex) {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("bytes/{modelId}")]
    public async Task<ActionResult<ResponseMessage<AnalyzeResultResponse>>> TextExtractFromBytes(string modelId, IFormFile document) {
        if(document == null || document.Length == 0)
            return BadRequest("No document provided.");

        try {
            var result = await azureDocumentService.AnalyzeDocumentBytesAsync(document, modelId);
            return Ok(result);
        } catch(Exception ex) {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("uri/{modelId}")]
    public async Task<ActionResult<ResponseMessage<AnalyzeResultResponse>>> TextExtractFromUri(string modelId, [FromBody] UriRequest request) {
        if(string.IsNullOrEmpty(request.UriDocument))
            return BadRequest("No URI provided.");

        try {
            var result = await azureDocumentService.AnalyzeDocumentUriAsync(request, modelId);
            return Ok(result);
        } catch(Exception ex) {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("azure-blob/{modelId}")]
    public async Task<ActionResult<ResponseMessage<AnalyzeResultResponse>>> TextExtractFromAzureBlob(string modelId, [FromBody] AzureBlobRequest request) {
        if(string.IsNullOrEmpty(request.ContainerName))
            return BadRequest("No container name provided.");

        if(string.IsNullOrEmpty(request.BlobName))
            return BadRequest("No blob name provided.");

        try {
            var result = await azureDocumentService.AnalyzeDocumentAzureBlobAsync(request, modelId);
            return Ok(result);
        } catch(Exception ex) {
            return BadRequest($"Failed to process invoice: {ex.Message}");
        }
    }
}

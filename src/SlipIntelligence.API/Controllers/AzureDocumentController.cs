﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

using SlipIntelligence.Application.Contracts.SlipIntelligence;
using SlipIntelligence.Application.Extensions;
using SlipIntelligence.Application.Models.SlipIntelligence;
// using SystemContract = SlipIntelligence.Application.Contracts.System;

namespace SlipIntelligence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AzureDocumentController: ControllerBase {
    private readonly IAzureDocumentService azureDocumentService;

    public AzureDocumentController(IAzureDocumentService azureDocumentService) {
        this.azureDocumentService = azureDocumentService;
    }

    [HttpPost("base64")]
    public async Task<ActionResult<ResponseMessage<AnalyzeResultResponse>>> TextExtractFromBase64([FromBody] Base64Request request) {
        if(string.IsNullOrEmpty(request.Base64Document))
            return BadRequest("No image provided.");

        try {
            var result = await azureDocumentService.AnalyzeDocumentBase64Async(request);
            return Ok(result);
        } catch(Exception ex) {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("uri")]
    public async Task<ActionResult<ResponseMessage<AnalyzeResultResponse>>> TextExtractFromUri([FromBody] UriRequest request) {
        if(string.IsNullOrEmpty(request.UriDocument))
            return BadRequest("No URI provided.");

        try {
            var result = await azureDocumentService.AnalyzeDocumentUriAsync(request);
            return Ok(result);
        } catch(Exception ex) {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("azure-blob")]
    public async Task<ActionResult<ResponseMessage<AnalyzeResultResponse>>> TextExtractFromAzureBlob([FromBody] AzureBlobRequest request) {
        if(string.IsNullOrEmpty(request.ContainerName))
            return BadRequest("No container name provided.");

        if(string.IsNullOrEmpty(request.BlobName))
            return BadRequest("No blob name provided.");

        try {
            var result = await azureDocumentService.AnalyzeDocumentAzureBlobAsync(request);
            return Ok(result);
        } catch(Exception ex) {
            return BadRequest($"Failed to process invoice: {ex.Message}");
        }
    }
}

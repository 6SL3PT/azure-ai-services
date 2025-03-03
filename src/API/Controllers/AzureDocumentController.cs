using Microsoft.AspNetCore.Mvc;
using SlipIntelligence.Application.Interfaces;
using SlipIntelligence.Domain;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SlipIntelligence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AzureDocumentController : ControllerBase
{
    private readonly IAzureDocumentService _azureDocumentService;

    public AzureDocumentController(IAzureDocumentService azureDocumentService)
    {
        _azureDocumentService = azureDocumentService;
    }

    [HttpPost("base64")]
    public async Task<IActionResult> TextExtractFromBase64([FromBody] Base64Request request)
    {
        if (string.IsNullOrEmpty(request.Base64Document))
        {
            return BadRequest("No image provided.");
        }

        try
        {
            var result = await _azureDocumentService.AnalyzeDocumentBase64Async(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("uri")]
    public async Task<IActionResult> TextExtractFromUri([FromBody] UriRequest request)
    {
        if (string.IsNullOrEmpty(request.UriDocument))
        {
            return BadRequest("No URI provided.");
        }

        try
        {
            var result = await _azureDocumentService.AnalyzeDocumentUriAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

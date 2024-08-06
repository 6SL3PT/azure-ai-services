using Microsoft.AspNetCore.Mvc;
using SlipIntelligence.Application.Interfaces;
using SlipIntelligence.Api.Models;
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
    public async Task<IActionResult> TextExtractFromBase64([FromBody] Base64ImageRequestModel request)
    {
        if (string.IsNullOrEmpty(request.Base64Image))
        {
            return BadRequest("No image provided.");
        }

        try
        {
            // Convert Base64 string to a byte array
            var imageBytes = Convert.FromBase64String(request.Base64Image);

            using var stream = new MemoryStream(imageBytes);
            var result = await _azureDocumentService.AnalyzeDocumentStreamAsync(stream);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

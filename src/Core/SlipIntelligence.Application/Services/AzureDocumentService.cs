using System.Runtime.CompilerServices;
using SlipIntelligence.Application.Interfaces;
using SlipIntelligence.Domain;
using SlipIntelligence.Infrastructure.Interfaces;

namespace SlipIntelligence.Application.Services;

public class AzureDocumentService : IAzureDocumentService
{
    private readonly IAzureDocumentClient _azureDocumentClient;

    public AzureDocumentService(IAzureDocumentClient azureDocumentClient)
    {
        _azureDocumentClient = azureDocumentClient;
    }

    public async Task<AnalyzeResultDto> AnalyzeDocumentBase64Async(Base64Request request)
    {
        var documentBytes = Convert.FromBase64String(request.Base64Document);

        // Convert Base64 string to a byte array
        using var stream = new MemoryStream(documentBytes);
        var response = await _azureDocumentClient.AnalyzeDocumentStreamAsync(stream);

        return new AnalyzeResultDto
        {
            Status = "success",
            AnalyzedText = response.Content
        };
    }

    public async Task<AnalyzeResultDto> AnalyzeDocumentUriAsync(UriRequest request)
    {
        var documentUri = new Uri(request.UriDocument);
        var result = await _azureDocumentClient.AnalyzeDocumentUriAsync(documentUri);

        return new AnalyzeResultDto
        {
            Status = "success",
            AnalyzedText = result.Content
        };
    }
}

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

    public async Task<AnalyzeResultDto> AnalyzeDocumentStreamAsync(Stream documentStream)
    {
        var response = await _azureDocumentClient.AnalyzeDocumentStreamAsync(documentStream);
        return new AnalyzeResultDto
        {
            Status = "success",
            AnalyzedText = response.Content
        };
    }

    public async Task<AnalyzeResultDto> AnalyzeDocumentUriAsync(Uri documentUri)
    {
        var result = await _azureDocumentClient.AnalyzeDocumentUriAsync(documentUri);
        return new AnalyzeResultDto
        {
            Status = "success",
            AnalyzedText = result.Content
        };
    }
}

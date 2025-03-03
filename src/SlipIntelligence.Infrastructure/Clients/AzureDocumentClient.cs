using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

using SlipIntelligence.Infrastructure.Interfaces;

namespace SlipIntelligence.Infrastructure.Clients;

public class AzureDocumentClient: IAzureDocumentClient {
    private readonly DocumentAnalysisClient _client;

    public AzureDocumentClient(DocumentAnalysisClient client) {
        _client = client;
    }

    public async Task<AnalyzeResult> AnalyzeDocumentStreamAsync(Stream documentStream, string modelId) {
        var response = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, documentStream);
        return response.Value;
    }

    public async Task<AnalyzeResult> AnalyzeDocumentUriAsync(Uri documentUri, string modelId) {
        var response = await _client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelId, documentUri);
        return response.Value;
    }
}

using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

using SlipIntelligence.Infrastructure.Interfaces;

namespace SlipIntelligence.Infrastructure.Clients;

public class AzureDocumentClient: IAzureDocumentClient {
    private readonly DocumentAnalysisClient _client;
    private readonly string _modelId;

    public AzureDocumentClient(DocumentAnalysisClient client, string modelId) {
        _client = client;
        _modelId = modelId;
    }

    public async Task<AnalyzeResult> AnalyzeDocumentStreamAsync(Stream documentStream) {
        var response = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, _modelId, documentStream);
        return response.Value;
    }

    public async Task<AnalyzeResult> AnalyzeDocumentUriAsync(Uri documentUri) {
        var response = await _client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, _modelId, documentUri);
        return response.Value;
    }
}

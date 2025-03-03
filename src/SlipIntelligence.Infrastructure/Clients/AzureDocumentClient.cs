using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

using SlipIntelligence.Infrastructure.Interfaces;

using System;
using System.IO;
using System.Threading.Tasks;

namespace SlipIntelligence.Infrastructure.Clients;

public class AzureDocumentClient: IAzureDocumentClient {
    private readonly DocumentAnalysisClient _client;
    private readonly string _modelId;

    public AzureDocumentClient(string endpoint, string apiKey, string modelId) {
        var credential = new AzureKeyCredential(apiKey);
        _client = new DocumentAnalysisClient(new Uri(endpoint), credential);
        _modelId = modelId;
    }

    public async Task<AnalyzeResult> AnalyzeDocumentStreamAsync(Stream documentStream) {
        var result =
            await _client.AnalyzeDocumentAsync(WaitUntil.Completed, _modelId, documentStream);
        return result.Value;
    }

    public async Task<AnalyzeResult> AnalyzeDocumentUriAsync(Uri documentUri) {
        var result =
            await _client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, _modelId, documentUri);
        return result.Value;
    }
}

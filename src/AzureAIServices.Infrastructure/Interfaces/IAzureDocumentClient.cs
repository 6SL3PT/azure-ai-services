using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace AzureAIServices.Infrastructure.Interfaces;

public interface IAzureDocumentClient {
    Task<AnalyzeResult> AnalyzeDocumentStreamAsync(Stream documentStream, string modelId);
    Task<AnalyzeResult> AnalyzeDocumentUriAsync(Uri documentUri, string modelId);
}

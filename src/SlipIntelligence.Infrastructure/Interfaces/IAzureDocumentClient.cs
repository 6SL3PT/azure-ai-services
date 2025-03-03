using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace SlipIntelligence.Infrastructure.Interfaces;

public interface IAzureDocumentClient {
    Task<AnalyzeResult> AnalyzeDocumentStreamAsync(Stream documentStream);
    Task<AnalyzeResult> AnalyzeDocumentUriAsync(Uri documentUri);
}

using Microsoft.AspNetCore.Http;

using SlipIntelligence.Application.Extensions;
using SlipIntelligence.Application.Models;

namespace SlipIntelligence.Application.Contracts;
public interface IAzureDocumentService {
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentBase64Async(Base64Request request, string modelId);
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentBytesAsync(IFormFile document, string modelId);
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentUriAsync(UriRequest request, string modelId);
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentAzureBlobAsync(AzureBlobRequest request, string modelId);
}

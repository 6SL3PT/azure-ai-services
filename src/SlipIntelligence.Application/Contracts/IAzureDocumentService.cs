using Microsoft.AspNetCore.Http;

using SlipIntelligence.Application.Extensions;
using SlipIntelligence.Application.Models;

namespace SlipIntelligence.Application.Contracts;
public interface IAzureDocumentService {
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentBase64Async(Base64Request request);
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentBytesAsync(IFormFile document);
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentUriAsync(UriRequest request);
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentAzureBlobAsync(AzureBlobRequest request);
}

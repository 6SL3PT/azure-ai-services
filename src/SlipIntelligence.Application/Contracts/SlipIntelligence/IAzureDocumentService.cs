using System.IO;
using System.Threading.Tasks;

using SlipIntelligence.Application.Extensions;
using SlipIntelligence.Application.Models.SlipIntelligence;

namespace SlipIntelligence.Application.Contracts.SlipIntelligence;
public interface IAzureDocumentService {
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentBase64Async(Base64Request request);
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentUriAsync(UriRequest request);
    Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentAzureBlobAsync(AzureBlobRequest request);
}

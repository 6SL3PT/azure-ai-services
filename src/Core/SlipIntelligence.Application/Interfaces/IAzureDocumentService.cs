using System.IO;
using System.Threading.Tasks;
using SlipIntelligence.Domain;

namespace SlipIntelligence.Application.Interfaces;

public interface IAzureDocumentService
{
    Task<AnalyzeResultDto> AnalyzeDocumentBase64Async(Base64Request request);
    Task<AnalyzeResultDto> AnalyzeDocumentUriAsync(UriRequest request);
}

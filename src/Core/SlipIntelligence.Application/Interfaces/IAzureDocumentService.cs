using System.IO;
using System.Threading.Tasks;
using SlipIntelligence.Domain;

namespace SlipIntelligence.Application.Interfaces;

public interface IAzureDocumentService
{
    Task<AnalyzeResultDto> AnalyzeDocumentStreamAsync(Stream documentStream);
    Task<AnalyzeResultDto> AnalyzeDocumentUriAsync(Uri documentUri);
}

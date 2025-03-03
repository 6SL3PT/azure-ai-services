using SlipIntelligence.Application.Interfaces;
using SlipIntelligence.Domain;
using SlipIntelligence.Infrastructure.Interfaces;

namespace SlipIntelligence.Application.Services;

public class AzureDocumentService : IAzureDocumentService
{
    private readonly IAzureBlobClient _azureBlobClient;
    private readonly IAzureDocumentClient _azureDocumentClient;

    public AzureDocumentService(IAzureDocumentClient azureDocumentClient, IAzureBlobClient azureBlobClient)
    {
        _azureBlobClient = azureBlobClient;
        _azureDocumentClient = azureDocumentClient;
    }

    public async Task<AnalyzeResultDto> AnalyzeDocumentBase64Async(Base64Request request)
    {
        // Convert Base64 string to a byte array
        var documentBytes = Convert.FromBase64String(request.Base64Document);

        using var stream = new MemoryStream(documentBytes);
        var response = await _azureDocumentClient.AnalyzeDocumentStreamAsync(stream);

        var fieldDict = response.Documents
            .SelectMany(doc => doc.Fields)
            .ToDictionary(
                fieldKvp => fieldKvp.Key,
                fieldKvp => new SlipField
                {
                    Content = fieldKvp.Value.Content,
                    Confidence = fieldKvp.Value.Confidence
                }
            );

        return new AnalyzeResultDto
        {
            ApiVersion = response.ServiceVersion,
            ModelId = response.ModelId,
            Content = response.Content,
            Fields = fieldDict
        };
    }

    public async Task<AnalyzeResultDto> AnalyzeDocumentUriAsync(UriRequest request)
    {
        var documentUri = new Uri(request.UriDocument);
        var response = await _azureDocumentClient.AnalyzeDocumentUriAsync(documentUri);

        var fieldDict = response.Documents
            .SelectMany(doc => doc.Fields)
            .ToDictionary(
                fieldKvp => fieldKvp.Key,
                fieldKvp => new SlipField
                {
                    Content = fieldKvp.Value.Content,
                    Confidence = fieldKvp.Value.Confidence
                }
            );

        return new AnalyzeResultDto
        {
            ApiVersion = response.ServiceVersion,
            ModelId = response.ModelId,
            Content = response.Content,
            Fields = fieldDict
        };
    }

    public async Task<AnalyzeResultDto> AnalyzeDocumentAzureBlobAsync(AzureBlobRequest request)
    {
        var blobStream = await _azureBlobClient.GetBlobStreamAsync(request.ContainerName, request.BlobName);

        // Azure Document Intelligence requires the stream to be seekable
        // These command will convert non-seekable stream into seekable stream
        // by copy the non-seekable stream to a MemoryStream which is seekable
        using var memoryStream = new MemoryStream();
        await blobStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var response = await _azureDocumentClient.AnalyzeDocumentStreamAsync(memoryStream);

        var fieldDict = response.Documents
            .SelectMany(doc => doc.Fields)
            .ToDictionary(
                fieldKvp => fieldKvp.Key,
                fieldKvp => new SlipField
                {
                    Content = fieldKvp.Value.Content,
                    Confidence = fieldKvp.Value.Confidence
                }
            );

        return new AnalyzeResultDto
        {
            ApiVersion = response.ServiceVersion,
            ModelId = response.ModelId,
            Content = response.Content,
            Fields = fieldDict
        };
    }
}

﻿using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

using Microsoft.AspNetCore.Http;

using AzureAIServices.Application.Contracts;
using AzureAIServices.Application.Extensions;
using AzureAIServices.Application.Models;
using AzureAIServices.Infrastructure.Interfaces;

using System.Text.Json;

namespace AzureAIServices.Application.Services;

public class AzureDocumentService: IAzureDocumentService {
    private readonly IAzureBlobClient _azureBlobClient;
    private readonly IAzureDocumentClient _azureDocumentClient;

    public AzureDocumentService(IAzureDocumentClient azureDocumentClient, IAzureBlobClient azureBlobClient) {
        _azureBlobClient = azureBlobClient;
        _azureDocumentClient = azureDocumentClient;
    }

    public async Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentBase64Async(Base64Request request, string modelId) {
        // Convert Base64 string to a byte array
        var documentBytes = Convert.FromBase64String(request.Base64Document);
        using var stream = new MemoryStream(documentBytes);
        try {
            var response = await _azureDocumentClient.AnalyzeDocumentStreamAsync(stream, modelId);
            return ConvertAnalyzeResultToResponse(response);
        } catch(RequestFailedException ex) {
            return ConvertAnalyzeResultErrorToResponse(ex, modelId);
        }
    }

    public async Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentBytesAsync(IFormFile document, string modelId) {
        using var documentStream = document.OpenReadStream();
        try {
            var response = await _azureDocumentClient.AnalyzeDocumentStreamAsync(documentStream, modelId);
            return ConvertAnalyzeResultToResponse(response);
        } catch(RequestFailedException ex) {
            return ConvertAnalyzeResultErrorToResponse(ex, modelId);
        }
    }

    public async Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentUriAsync(UriRequest request, string modelId) {
        var documentUri = new Uri(request.UriDocument);
        try {
            var response = await _azureDocumentClient.AnalyzeDocumentUriAsync(documentUri, modelId);
            return ConvertAnalyzeResultToResponse(response);
        } catch(RequestFailedException ex) {
            return ConvertAnalyzeResultErrorToResponse(ex, modelId);
        }
    }

    public async Task<ResponseMessage<AnalyzeResultResponse>> AnalyzeDocumentAzureBlobAsync(AzureBlobRequest request, string modelId) {
        Stream? blobStream;
        try {
            blobStream = await _azureBlobClient.GetBlobStreamAsync(request.ContainerName, request.BlobName);
        } catch(RequestFailedException ex) {
            return new ResponseMessage<AnalyzeResultResponse>(
                new AnalyzeResultResponse() {
                    Success = false,
                    ModelId = modelId,
                    Content = ex.Message ?? "RequestFailedException: Error while fetching blob from Azure Blob Storage"
                });
        } catch(Exception ex) {
            return new ResponseMessage<AnalyzeResultResponse>(
                new AnalyzeResultResponse() {
                    Success = false,
                    ModelId = modelId,
                    Content = ex.Message ?? "Exception: Error while fetching blob from Azure Blob Storage"
                });
        }

        // Azure Document Intelligence requires the stream to be seekable
        // These command will convert non-seekable stream into seekable stream
        // by copy the non-seekable stream to a MemoryStream which is seekable
        using var memoryStream = new MemoryStream();
        await blobStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        try {
            var response = await _azureDocumentClient.AnalyzeDocumentStreamAsync(memoryStream, modelId);
            return ConvertAnalyzeResultToResponse(response);
        } catch(RequestFailedException ex) {
            return ConvertAnalyzeResultErrorToResponse(ex, modelId);
        }
    }

    private static ResponseMessage<AnalyzeResultResponse> ConvertAnalyzeResultToResponse(AnalyzeResult response) {
        if(response.Documents == null || response.Documents.Count == 0) {
            return new ResponseMessage<AnalyzeResultResponse>(
                new AnalyzeResultResponse {
                    Success = false,
                    ApiVersion = response.ServiceVersion,
                    ModelId = response.ModelId,
                    Content = "Model did not found any document."
                });
        }

        var fieldDict = response.Documents
            .SelectMany(doc => doc.Fields)
            .ToDictionary(
                fieldKvp => JsonNamingPolicy.CamelCase.ConvertName(fieldKvp.Key),
                fieldKvp => new SlipFieldDto {
                    Content = fieldKvp.Value.Content,
                    Confidence = fieldKvp.Value.Confidence
                }
            );

        return new ResponseMessage<AnalyzeResultResponse>(
            new AnalyzeResultResponse {
                Success = true,
                ApiVersion = response.ServiceVersion,
                ModelId = response.ModelId,
                Content = response.Content,
                Fields = fieldDict
            });
    }

    private static ResponseMessage<AnalyzeResultResponse> ConvertAnalyzeResultErrorToResponse(RequestFailedException exception, string modelId) {
        var innerError = exception.GetRawResponse()?.Content.ToObjectFromJson<Dictionary<string, AnalyzeResultResponseError>>()["error"].InnerError;
        return new ResponseMessage<AnalyzeResultResponse>(
            new AnalyzeResultResponse() {
                Success = false,
                ModelId = modelId,
                Content = innerError == null
                    ? "Error while sending request to Azure Document Intelligence"
                    : $"{innerError.Code}: {innerError.Message}"
            });
    }
}

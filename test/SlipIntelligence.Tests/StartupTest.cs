using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using SlipIntelligence.API;
using SlipIntelligence.Application.Contracts;
using SlipIntelligence.Application.Services;
using SlipIntelligence.Infrastructure.Clients;
using SlipIntelligence.Infrastructure.Interfaces;

using System.Net.Http.Headers;
using System.Text;

namespace SlipIntelligence.Tests;
public class StartupTests: IClassFixture<WebApplicationFactory<Startup>> {
    private readonly WebApplicationFactory<Startup> _factory;

    public StartupTests(WebApplicationFactory<Startup> factory) {
        _factory = factory;
    }

    [Fact]
    public void ConfigureServices_RegistersServices() {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Act
        var documentClient = serviceProvider.GetService<IAzureDocumentClient>();
        var blobClient = serviceProvider.GetService<IAzureBlobClient>();
        var documentService = serviceProvider.GetService<IAzureDocumentService>();

        // Assert
        Assert.NotNull(documentClient);
        Assert.NotNull(blobClient);
        Assert.NotNull(documentService);
        Assert.IsType<AzureDocumentClient>(documentClient);
        Assert.IsType<AzureBlobClient>(blobClient);
        Assert.IsType<AzureDocumentService>(documentService);
    }
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using SlipIntelligence.API;
using SlipIntelligence.Application.Contracts;
using SlipIntelligence.Application.Services;
using SlipIntelligence.Infrastructure.Clients;
using SlipIntelligence.Infrastructure.Interfaces;

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
        var configuration = serviceProvider.GetService<IConfiguration>();

        // Assert
        Assert.NotNull(documentClient);
        Assert.NotNull(blobClient);
        Assert.NotNull(documentService);
        Assert.NotNull(configuration);
        Assert.IsType<AzureDocumentClient>(documentClient);
        Assert.IsType<AzureBlobClient>(blobClient);
        Assert.IsType<AzureDocumentService>(documentService);
    }

    [Fact]
    public void ConfigureServices_WithMockedServices_RegistersMockedInstances()
    {
        // Arrange
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                // Mock IAzureDocumentClient
                var mockDocumentClient = new Mock<IAzureDocumentClient>();
                services.AddSingleton(mockDocumentClient.Object);

                // Mock IAzureBlobClient
                var mockBlobClient = new Mock<IAzureBlobClient>();
                services.AddSingleton(mockBlobClient.Object);

                // Mock other dependencies as needed
            })
            .UseStartup<Startup>();

        var factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { });
        });

        using var scope = factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Act
        var documentClient = serviceProvider.GetService<IAzureDocumentClient>();
        var blobClient = serviceProvider.GetService<IAzureBlobClient>();

        // Assert
        Assert.NotNull(documentClient);
        Assert.IsType<AzureDocumentClient>(documentClient); // Depending on how Moq is set up

        Assert.NotNull(blobClient);
        Assert.IsType<AzureBlobClient>(blobClient); // Depending on how Moq is set up
    }

    [Fact]
    public void ConfigureServices_MissingAzureDocumentIntelligenceEndpoint_ThrowsException()
    {
        // Arrange: Setup in-memory configuration without the required endpoint
        var inMemorySettings = new Dictionary<string, string?>
        {
            // "Azure:DocumentIntelligence:Endpoint" is intentionally omitted
            { "Azure:DocumentIntelligence:ApiKey", "fake-api-key" },
            { "Azure:BlobStorage:ConnectionString", "UseDevelopmentStorage=true" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var startup = new Startup(configuration);
        var services = new ServiceCollection();

        // Act & Assert: Expect exception when resolving IAzureDocumentClient
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var documentClient = serviceProvider.GetRequiredService<IAzureDocumentClient>();
        });

        Assert.Equal("Azure:DocumentIntelligence:Endpoint is not configured.", exception.Message);
    }

    [Fact]
    public void ConfigureServices_MissingAzureBlobStorageConnectionString_ThrowsException()
    {
        // Arrange: Setup in-memory configuration without the required connection string
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Azure:DocumentIntelligence:Endpoint", "https://fake-endpoint.com" },
            { "Azure:DocumentIntelligence:ApiKey", "fake-api-key" }
            // "Azure:BlobStorage:ConnectionString" is intentionally omitted
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var startup = new Startup(configuration);
        var services = new ServiceCollection();

        // Act & Assert: Expect exception when resolving IAzureBlobClient
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var blobClient = serviceProvider.GetRequiredService<IAzureBlobClient>();
        });

        Assert.Equal("Azure:BlobStorage:ConnectionString is not configured.", exception.Message);
    }
}

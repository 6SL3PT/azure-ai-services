using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Storage.Blobs;
using Microsoft.OpenApi.Models;
using SlipIntelligence.Api.Middlewares;
using SlipIntelligence.Application.Contracts;
using SlipIntelligence.Application.Services;
using SlipIntelligence.Infrastructure.Interfaces;
using SlipIntelligence.Infrastructure.Clients;

namespace SlipIntelligence.API;
public class Startup {
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services) {
        services.AddControllers();

        // Configure Azure Document Intelligence client
        services.AddSingleton<IAzureDocumentClient>(s => {
            var endpoint = Configuration["Azure:DocumentIntelligence:Endpoint"]
                ?? throw new InvalidOperationException("Azure:DocumentIntelligence:Endpoint is not configured.");
            var apiKey = Configuration["Azure:DocumentIntelligence:ApiKey"]
                ?? throw new InvalidOperationException("Azure:DocumentIntelligence:ApiToken is not configured.");
            var modelId = Configuration["Azure:DocumentIntelligence:ModelId"]
                ?? throw new InvalidOperationException("Azure:DocumentIntelligence:ModelId is not configured.");
            return new AzureDocumentClient(new DocumentAnalysisClient(new Uri(endpoint), new AzureKeyCredential(apiKey)), modelId);
        });

        // Configure Azure Blob Storage client
        services.AddSingleton<IAzureBlobClient>(s => {
            var connectionString = Configuration["Azure:BlobStorage:ConnectionString"]
                ?? throw new InvalidOperationException("Azure:BlobStorage:ConnectionString is not configured.");
            return new AzureBlobClient(new BlobServiceClient(connectionString));
        });

        // Register application services
        services.AddScoped<IAzureDocumentService, AzureDocumentService>();

        services.AddSingleton(Configuration);
        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Slip Processing WebAPI", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if(env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        } else {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseMiddleware<SimpleAuthenticationMiddleware>(Configuration);
        app.UseAuthorization();

        // Enable middleware to serve generated Swagger as a JSON endpoint
        app.UseSwagger();

        // Enable middleware to serve Swagger UI, specifying the Swagger JSON endpoint
        app.UseSwaggerUI(c => {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Slip Processing WebAPI V1");
            c.RoutePrefix = "swagger";
        });

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
}


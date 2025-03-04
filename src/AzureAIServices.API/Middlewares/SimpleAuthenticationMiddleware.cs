using System.Text.Json;

using AzureAIServices.Application.Extensions;

namespace AzureAIServices.Api.Middlewares;
public class SimpleAuthenticationMiddleware {
    private readonly RequestDelegate _next;
    private readonly string _token;

    public SimpleAuthenticationMiddleware(
            RequestDelegate next,
            IConfiguration configuration) {
        _next = next;
        _token = configuration["ApiSettings:ApiToken"]
            ?? throw new InvalidOperationException("ApiSettings:ApiToken is not configured.");
    }

    public async Task InvokeAsync(HttpContext context) {
        if(context.Request.Headers.TryGetValue("Authorization", out var authHeader)) {
            var token = authHeader.ToString().Replace("Bearer ", "");

            if(token == _token) {
                await _next(context);
                return;
            }
        }

        var responseMessage = new ResponseMessage<ErrorMessage>(new ErrorMessage(9102, "Invalid or missing token"));

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var jsonResponse = JsonSerializer.Serialize(responseMessage);
        await context.Response.WriteAsync(jsonResponse);
    }
}

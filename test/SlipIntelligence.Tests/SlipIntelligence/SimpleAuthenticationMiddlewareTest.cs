using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using Moq;

using SlipIntelligence.Api.Middlewares;


namespace SlipIntelligence.Tests.SlipIntelligence;

public class SimpleAuthenticationMiddlewareTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly HttpContext _httpContext;

    public SimpleAuthenticationMiddlewareTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockNext = new Mock<RequestDelegate>();
        _httpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task InvokeAsync_WithValidToken_CallsNextMiddleware()
    {
        // Arrange
        string validToken = "valid_token";
        _mockConfiguration.Setup(c => c["ApiSettings:ApiToken"]).Returns(validToken);

        _httpContext.Request.Headers["Authorization"] = "Bearer valid_token";

        var _middleware = new SimpleAuthenticationMiddleware(_mockNext.Object, _mockConfiguration.Object);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        Assert.Equal(200, _httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        string validToken = "valid_token";
        _mockConfiguration.Setup(c => c["ApiSettings:ApiToken"]).Returns(validToken);

        _httpContext.Request.Headers["Authorization"] = "Bearer invalid_token";

        var _middleware = new SimpleAuthenticationMiddleware(_mockNext.Object, _mockConfiguration.Object);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(401, _httpContext.Response.StatusCode);
        Assert.Equal("application/json", _httpContext.Response.ContentType);
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WhenAuthorizationHeaderIsMissing_ReturnsUnauthorized()
    {
        // Arrange
        string validToken = "valid_token";
        _mockConfiguration.Setup(c => c["ApiSettings:ApiToken"]).Returns(validToken);

        var _middleware = new SimpleAuthenticationMiddleware(_mockNext.Object, _mockConfiguration.Object);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(401, _httpContext.Response.StatusCode);
        Assert.Equal("application/json", _httpContext.Response.ContentType);
        _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public void Constructor_WhenApiTokenIsNotConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["ApiSettings:ApiToken"]).Returns<string>((key) => null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            new SimpleAuthenticationMiddleware(_mockNext.Object, _mockConfiguration.Object));
    }
}

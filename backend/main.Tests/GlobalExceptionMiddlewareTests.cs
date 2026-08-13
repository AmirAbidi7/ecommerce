using System.Text.Json;
using main.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace main.Tests;

public class GlobalExceptionMiddlewareTests
{
    private static async Task<(int Status, JsonElement Body)> Invoke(Exception ex)
    {
        var http = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
        var middleware = new GlobalExceptionMiddleware(
            _ => throw ex,
            NullLogger<GlobalExceptionMiddleware>.Instance);

        await middleware.InvokeAsync(http);

        http.Response.Body.Seek(0, SeekOrigin.Begin);
        using var doc = JsonDocument.Parse(http.Response.Body);
        return (http.Response.StatusCode, doc.RootElement.Clone());
    }

    [Theory]
    [InlineData(typeof(ArgumentException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(UnauthorizedAccessException), StatusCodes.Status401Unauthorized)]
    [InlineData(typeof(KeyNotFoundException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(InvalidOperationException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task ShouldMapExceptionToStatus(Type exceptionType, int expectedStatus)
    {
        var ex = (Exception)Activator.CreateInstance(exceptionType, "boom")!;
        var (status, _) = await Invoke(ex);

        Assert.Equal(expectedStatus, status);
    }

    [Fact]
    public async Task ShouldWriteErrorBody()
    {
        var (_, body) = await Invoke(new KeyNotFoundException("missing"));

        Assert.Equal("missing", body.GetProperty("error").GetProperty("message").GetString());
    }
}
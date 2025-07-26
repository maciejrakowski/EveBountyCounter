using EveBountyCounter.EwbApiClient.JsonSerializerContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EveBountyCounter.EwbApiClient;

/// <summary>
/// Represents a base class for API client implementations.
/// </summary>
internal abstract class ApiClient(IHttpClientFactory httpClientFactory, ILogger<ApiClient>? logger)
{
    /// <summary>
    /// Sends an asynchronous HTTP request to a specified URL with the given parameters.
    /// </summary>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="method">The HTTP method to use, such as GET, POST, etc.</param>
    /// <param name="headers">A dictionary of headers to include in the request.</param>
    /// <param name="body">The body of the request, if any. Can be null for requests without a body.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the response content as a string if the request is successful; otherwise, null.</returns>
    protected async Task<string?> SendAsync(string url, HttpMethod method, Dictionary<string, string> headers, string? body = null)
    {
        using var client = httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(method, url);
        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        request.Content = body is not null ? new StringContent(body) : null;

        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            logger?.LogInformation("{}; Response from {}: {}", nameof(SendAsync), url, content);
            
            return content;
            
        }
        
        return null;
    }
}
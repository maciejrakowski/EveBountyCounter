using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EveBountyCounter.EwbApiClient;

/// <summary>
/// Represents a base class for API client implementations.
/// </summary>
internal abstract class ApiClient(IHttpClientFactory httpClientFactory, ILogger<ApiClient>? logger)
{
    /// <summary>
    /// Sends an asynchronous HTTP request to the given URL with the specified method, headers, and optional body,
    /// and deserializes the response into the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the HTTP response content is deserialized.</typeparam>
    /// <param name="url">The URL to send the HTTP request to.</param>
    /// <param name="method">The HTTP method to use for the request (e.g., GET, POST).</param>
    /// <param name="headers">A dictionary of headers to include in the HTTP request.</param>
    /// <param name="body">The optional body content to include in the HTTP request, serialized as a string.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the specified type, or null if the deserialization fails.</returns>
    protected async Task<T?> SendAsync<T>(string url, HttpMethod method, Dictionary<string, string> headers, string? body = null)
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
            
            return JsonConvert.DeserializeObject<T>(content);
        }
        
        return default;
    }
}
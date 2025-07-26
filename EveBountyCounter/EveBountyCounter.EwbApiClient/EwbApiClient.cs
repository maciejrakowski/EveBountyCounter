using System.Globalization;
using System.Text.Json;
using EveBountyCounter.EwbApiClient.Contracts;
using EveBountyCounter.EwbApiClient.JsonSerializerContext;
using Microsoft.Extensions.Logging;

namespace EveBountyCounter.EwbApiClient;

/// <inheritdoc cref="IEwbApiClient"/>
internal class EwbApiClient(IHttpClientFactory httpClientFactory, ILogger<EwbApiClient> logger) : ApiClient(httpClientFactory, logger), IEwbApiClient
{
    public async Task<RealtimeBountyUpdateResponse?> SubmitBountyAsync(string apiKey, decimal bountyAmount)
    {
        var headers = GetHeaders(apiKey);
        string url = EwbUrl.Bounty.Replace("{{version}}", "1");

        var response = await SendAsync(url, HttpMethod.Post, headers, bountyAmount.ToString(new CultureInfo("en-US")));

        if (response is null)
        {
            return null;
        }
        
        var result = JsonSerializer.Deserialize(response, DeserializationModeOptionsContext.Default.RealtimeBountyUpdateResponse);
        
        return result;
    }

    public async Task<RealtimeBountyUpdateResponse?> SubmitBountyAsync(string apiKey, string characterId, decimal bountyAmount)
    {
        var headers = GetHeaders(apiKey);
        string url = EwbUrl.BountyByCharacter.Replace("{{version}}", "1").Replace("{{characterId}}", characterId);

        var response = await SendAsync(url, HttpMethod.Post, headers, bountyAmount.ToString(new CultureInfo("en-US")));

        if (response is null)
        {
            return null;
        }
        
        var result = JsonSerializer.Deserialize(response, DeserializationModeOptionsContext.Default.RealtimeBountyUpdateResponse);
        
        return result;
    }

    public async Task<IEnumerable<CharacterResponse>> GetCharactersAsync(string apiKey)
    {
        var headers = GetHeaders(apiKey);
        string url = EwbUrl.Characters.Replace("{{version}}", "1");

        var response = await SendAsync(url, HttpMethod.Get, headers);

        if (response is null)
        {
            return [];
        }
        
        var result =  JsonSerializer.Deserialize(response, DeserializationModeOptionsContext.Default.IEnumerableCharacterResponse);
        
        return result ?? [];
    }

    /// <summary>
    /// Constructs a dictionary containing the headers required for API requests.
    /// </summary>
    /// <param name="apiKey">The API key used for authentication.</param>
    /// <returns>A dictionary with the necessary headers for the API request.</returns>
    private Dictionary<string, string> GetHeaders(string apiKey)
    {
        return new Dictionary<string, string>
        {
            ["x-api-key"] = apiKey
        };
    }
}
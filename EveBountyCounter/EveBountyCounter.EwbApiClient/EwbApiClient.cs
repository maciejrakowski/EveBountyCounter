using EveBountyCounter.EwbApiClient.Contracts;

namespace EveBountyCounter.EwbApiClient;

/// <inheritdoc cref="IEwbApiClient"/>
internal class EwbApiClient(IHttpClientFactory httpClientFactory) : ApiClient(httpClientFactory), IEwbApiClient
{
    public async Task<bool> SubmitBountyAsync(string apiKey, decimal bountyAmount)
    {
        var headers = GetHeaders(apiKey);
        string url = EwbUrl.Bounty.Replace("{{version}}", "1");

        await SendAsync<string>(url, HttpMethod.Post, headers, $"{{\"bounty\": {bountyAmount}}}");

        return true;
    }
    
    public async Task<bool> SubmitBountyAsync(string apiKey, string characterId, decimal bountyAmount)
    {
        var headers = GetHeaders(apiKey);
        string url = EwbUrl.BountyByCharacter.Replace("{{version}}", "1").Replace("{{characterId}}", characterId);

        await SendAsync<string>(url, HttpMethod.Post, headers, $"{{\"bounty\": {bountyAmount}}}");

        return true;
    }

    public async Task<IEnumerable<CharacterResponse>> GetCharactersAsync(string apiKey)
    {
        var headers = GetHeaders(apiKey);
        string url = EwbUrl.Characters.Replace("{{version}}", "1");

        var response = await SendAsync<IEnumerable<CharacterResponse>>(url, HttpMethod.Get, headers);

        return response ?? [];
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
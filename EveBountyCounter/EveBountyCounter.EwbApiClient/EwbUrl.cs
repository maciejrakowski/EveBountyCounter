namespace EveBountyCounter.EwbApiClient;

public static class EwbUrl
{
    private const string _baseUrl = "https://api.eveworkbench.com/";

    public static string Characters => _baseUrl +  "v{{version}}/characters";
    public static string Bounty => _baseUrl + "v{{version}}/eve-journal/realtime-bounty-update";
    public static string BountyByCharacter => _baseUrl + "v{{version}}/eve-journal/realtime-bounty-update/{{characterId}}";
}
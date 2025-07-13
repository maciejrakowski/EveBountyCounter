namespace EveBountyHunter.Configuration.Models;

/// <summary>
/// Represents an API key configuration for EVE Workbench.
/// </summary>
/// <remarks>
/// This configuration stores the character name and associated API key for accessing EVE Workbench services.
/// </remarks>
public class EveWorkbenchApiKey
{
    /// <summary>
    /// Gets or sets the name of the character associated with the Eve Workbench API key.
    /// </summary>
    public string CharacterName { get; set; } = "";

    /// <summary>
    /// Gets or sets the API key used for authentication with EveWorkbench services.
    /// </summary>
    public string ApiKey { get; set; } = "";
}
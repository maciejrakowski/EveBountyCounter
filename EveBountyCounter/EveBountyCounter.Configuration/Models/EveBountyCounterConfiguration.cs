namespace EveBountyHunter.Configuration.Models;

/// <summary>
/// Represents the configuration settings for the EVE Bounty Counter application.
/// </summary>
/// <remarks>
/// The configuration primarily includes the directory for storing log files.
/// </remarks>
public class EveBountyCounterConfiguration
{
    /// <summary>
    /// Gets or sets the directory path where log files for the application are stored.
    /// </summary>
    public required string LogsDirectory { get; set; }

    /// <summary>
    /// Gets or sets the list of API keys for accessing EveWorkbench services.
    /// </summary>
    public List<EveWorkbenchApiKey> EveWorkbenchApiKeys { get; set; } = [];
}
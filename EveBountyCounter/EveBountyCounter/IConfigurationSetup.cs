namespace EveBountyCounter;

/// <summary>
/// Provides setup and configuration management for the EVE Bounty Counter application.
/// </summary>
public interface IConfigurationSetup
{
    /// <summary>
    /// Retrieves the directory path for game logs used by the application.
    /// </summary>
    /// <returns>
    /// The directory path for game logs as a string. If a valid configuration is found,
    /// it returns the configured logs directory. Otherwise, it prompts the user to input a valid logs directory path.
    /// </returns>
    string GetLogsDirectory();

    /// <summary>
    /// Prompts the user to enter a character name and API key, and adds the API key to the application configuration.
    /// </summary>
    /// <remarks>
    /// If the provided character name or API key is empty or null, the operation is aborted and a message is displayed to the user.
    /// The API key is saved using the configuration management methods provided by the EbhConfiguration class.
    /// </remarks>
    Task AddApiKey();
}
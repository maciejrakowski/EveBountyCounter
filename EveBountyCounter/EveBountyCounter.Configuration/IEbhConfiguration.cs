using EveBountyHunter.Configuration.Models;

namespace EveBountyHunter.Configuration;

/// <summary>
/// Provides methods for handling configuration operations related to
/// the EveBountyCounter application, including loading and saving configuration settings.
/// </summary>
public interface IEbhConfiguration
{
    /// <summary>
    /// Retrieves the configuration settings for the EveBountyCounter application from a predefined configuration file.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="EveBountyCounterConfiguration"/> if the configuration file was successfully read and deserialized;
    /// otherwise, null if the file is not found or an error occurs.
    /// </returns>
    EveBountyCounterConfiguration? GetConfiguration();

    /// <summary>
    /// Saves the provided configuration settings to a predefined configuration file.
    /// </summary>
    /// <param name="config">The configuration settings to save.</param>
    /// <remarks>
    /// This method serializes the provided configuration settings to JSON and writes them to the configuration file.
    /// </remarks>
    void SaveConfiguration(EveBountyCounterConfiguration config);

    /// <summary>
    /// Adds an API key for accessing EveWorkbench services to the configuration settings.
    /// </summary>
    /// <param name="characterName">The name of the character associated with the API key.</param>
    /// <param name="characterId">The unique identifier of the character associated with the API key.</param>
    /// <param name="apiKey">The API key to add or update in the configuration settings.</param>
    void AddApiKey(string characterName, long characterId, string apiKey);

    /// <summary>
    /// Retrieves the API key details for a specific character based on the provided character name.
    /// </summary>
    /// <param name="characterName">The name of the character whose API key details are to be retrieved.</param>
    /// <returns>
    /// An instance of <see cref="EveWorkbenchCharacterApi"/> containing the API key details for the specified character
    /// if a match is found; otherwise, null if the character is not found in the configuration.
    /// </returns>
    EveWorkbenchCharacterApi? GetCharacter(string characterName);
}
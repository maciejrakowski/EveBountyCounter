using System.Text.Json;
using EveBountyHunter.Configuration.JsonSerializerContext;
using EveBountyHunter.Configuration.Models;

namespace EveBountyHunter.Configuration;

/// <summary>
/// Provides methods for handling configuration operations related to
/// the EveBountyCounter application, including loading and saving configuration settings.
/// </summary>
public static class EbhConfiguration
{
    private static readonly string ConfigurationFilePath = "ebhConfig.json";

    /// <summary>
    /// Retrieves the configuration settings for the EveBountyCounter application from a predefined configuration file.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="EveBountyCounterConfiguration"/> if the configuration file was successfully read and deserialized;
    /// otherwise, null if the file is not found or an error occurs.
    /// </returns>
    public static EveBountyCounterConfiguration? GetConfiguration()
    {
        string fileContent = String.Empty;
        try
        {
            fileContent = File.ReadAllText(ConfigurationFilePath);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Cannot read file {ConfigurationFilePath}, exception: {ex.Message}");
        }

        var config = JsonSerializer.Deserialize(fileContent, DeserializationModeOptionsContext.Default.EveBountyCounterConfiguration);;

        return config;
    }

    /// <summary>
    /// Saves the provided configuration settings to a predefined configuration file.
    /// </summary>
    /// <param name="config">The configuration settings to save.</param>
    /// <remarks>
    /// This method serializes the provided configuration settings to JSON and writes them to the configuration file.
    /// </remarks>
    public static void SaveConfiguration(EveBountyCounterConfiguration config)
    {
        var json = JsonSerializer.Serialize(config, SerializationModeOptionsContext.Default.EveBountyCounterConfiguration);
        File.WriteAllText(ConfigurationFilePath, json);
    }

    /// <summary>
    /// Adds an API key for accessing EveWorkbench services to the configuration settings.
    /// </summary>
    /// <param name="characterName">The name of the character associated with the API key.</param>
    /// <param name="apiKey">The API key to add to the configuration settings.</param>
    /// <remarks>
    /// This method adds the provided API key to the configuration settings for the specified character.
    /// If the character already has an API key, the existing key is updated with the provided one.
    /// </remarks>
    public static void AddApiKey(string characterName, string apiKey)
    {
        var config = GetConfiguration();
        if (config is null)
        {
            return;
        }
        
        var apiKeys = config.EveWorkbenchApiKeys;
        var existingApiKey = apiKeys.FirstOrDefault(x => x.CharacterName == characterName);
        if (existingApiKey is not null)
        {
            existingApiKey.ApiKey = apiKey;
        }
        else
        {
            apiKeys.Add(new EveWorkbenchApiKey
            {
                CharacterName = characterName,
                ApiKey = apiKey
            });
        }
        
        SaveConfiguration(config);
    }
}
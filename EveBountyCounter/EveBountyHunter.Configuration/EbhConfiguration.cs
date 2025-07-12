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
}
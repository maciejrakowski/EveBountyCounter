using System.Text.Json;
using EveBountyHunter.Configuration.JsonSerializerContext;
using EveBountyHunter.Configuration.Models;

namespace EveBountyHunter.Configuration;

/// <inheritdoc cref="IEbhConfiguration"/>
internal class EbhConfiguration : IEbhConfiguration
{
    private readonly string _configurationFilePath = "ebhConfig.json";

    public EveBountyCounterConfiguration? GetConfiguration()
    {
        string fileContent = String.Empty;
        try
        {
            fileContent = File.ReadAllText(_configurationFilePath);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Cannot read file {_configurationFilePath}, exception: {ex.Message}");
        }

        var config = JsonSerializer.Deserialize(fileContent, DeserializationModeOptionsContext.Default.EveBountyCounterConfiguration);
        ;

        return config;
    }
    
    public void SaveConfiguration(EveBountyCounterConfiguration config)
    {
        var json = JsonSerializer.Serialize(config, SerializationModeOptionsContext.Default.EveBountyCounterConfiguration);
        File.WriteAllText(_configurationFilePath, json);
    }

    public void AddApiKey(string characterName, long characterId, string apiKey)
    {
        var config = GetConfiguration();
        if (config is null)
        {
            return;
        }

        var apiKeys = config.EveWorkbenchCharacterApis;
        var existingApiKey = apiKeys.FirstOrDefault(x => x.CharacterName == characterName);
        if (existingApiKey is not null)
        {
            existingApiKey.ApiKey = apiKey;
            existingApiKey.CharacterId = characterId;
        }
        else
        {
            apiKeys.Add(new EveWorkbenchCharacterApi
            {
                CharacterName = characterName,
                CharacterId = characterId,
                ApiKey = apiKey
            });
        }

        SaveConfiguration(config);
    }

    public EveWorkbenchCharacterApi? GetCharacter(string characterName)
    {
        var config = GetConfiguration();
        if (config is null)
        {
            return null;
        }
        
        return config.EveWorkbenchCharacterApis.FirstOrDefault(x => x.CharacterName == characterName);
    }
}
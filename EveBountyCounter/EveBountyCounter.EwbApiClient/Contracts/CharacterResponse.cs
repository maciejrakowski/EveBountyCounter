namespace EveBountyCounter.EwbApiClient.Contracts;

/// <summary>
/// Represents a response containing character information retrieved from the EVE Workbench API.
/// </summary>
public class CharacterResponse
{
    /// <summary>
    /// Gets or sets the name of the character.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the unique identifier of the character.
    /// </summary>
    public required long Id { get; set; }
}
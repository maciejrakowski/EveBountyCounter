using EveBountyCounter.EwbApiClient.Contracts;

namespace EveBountyCounter.EwbApiClient;

/// <summary>
/// Defines the interface for an API client for EVE Workbench.
/// </summary>
public interface IEwbApiClient
{
    /// <summary>
    /// Submits a bounty to the EVE Workbench API.
    /// </summary>
    /// <param name="apiKey">The API key used for authentication.</param>
    /// <param name="bountyAmount">The amount of the bounty to be submitted.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
    Task<bool> SubmitBountyAsync(string apiKey, decimal bountyAmount);

    /// <summary>
    /// Submits a bounty to the EVE Workbench service for a specific character.
    /// </summary>
    /// <param name="apiKey">The API key for authenticating with the EVE Workbench API.</param>
    /// <param name="characterId">The ID of the character for whom the bounty is being submitted.</param>
    /// <param name="bountyAmount">The amount of the bounty to be submitted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the bounty submission was successful.</returns>
    Task<bool> SubmitBountyAsync(string apiKey, string characterId, decimal bountyAmount);

    /// <summary>
    /// Asynchronously retrieves a list of characters associated with the provided API key from the EVE Workbench API.
    /// </summary>
    /// <param name="apiKey">The API key used to authenticate and retrieve character data.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains an enumerable collection of <see cref="CharacterResponse"/> objects representing the characters retrieved.</returns>
    Task<IEnumerable<CharacterResponse>> GetCharactersAsync(string apiKey);
}
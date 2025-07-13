using EveBountyCounter.Counter.Models;

namespace EveBountyCounter.Counter.Events;

/// <summary>
/// Provides data for events related to character bounty updates in the EVE Bounty Counter application.
/// </summary>
public class CharacterBountyUpdatedEventArgs(CharacterBounty characterBounty, long bountyIncrease) : EventArgs
{
    /// <summary>
    /// Gets the bounty tracking information associated with a specific character.
    /// </summary>
    public CharacterBounty CharacterBounty { get; } = characterBounty;

    /// <summary>
    /// Gets or sets the amount by which the bounty has increased.
    /// </summary>
    /// <remarks>
    /// This property represents the incremental change in the bounty value for a character since the last bounty update.
    /// </remarks>
    public long BountyIncrease { get; set; } = bountyIncrease;
}

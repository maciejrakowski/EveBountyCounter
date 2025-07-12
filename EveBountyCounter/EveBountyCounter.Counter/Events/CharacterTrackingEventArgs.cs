using EveBountyCounter.Counter.Models;

namespace EveBountyCounter.Counter.Events;

/// <summary>
/// Provides data for events related to character tracking in the EVE Bounty Counter application.
/// </summary>
/// <remarks>
/// This event argument is associated with character tracking events, such as when tracking starts,
/// undocking occurs, or bounty updates are registered. It encapsulates information regarding
/// the tracked character's bounty details, including their name, log file path, and bounty data.
/// </remarks>
public class CharacterTrackingEventArgs(CharacterBounty characterBounty) : EventArgs
{
    /// <summary>
    /// Represents the information and state related to a tracked character's bounty in the EVE Bounty Counter application.
    /// </summary>
    /// <remarks>
    /// This property provides access to the `CharacterBounty` instance, which stores details about the character being tracked, such as:
    /// - Character's name
    /// - Path to the log file associated with the character
    /// - Last known position in the log file
    /// - Total accumulated bounty
    /// - Session-specific bounty total
    /// </remarks>
    public CharacterBounty CharacterBounty { get; } = characterBounty;
}

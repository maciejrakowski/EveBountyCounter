namespace EveBountyCounter.Counter.Models;

/// <summary>
/// Represents a bounty tracking model for an EVE Online character.
/// </summary>
/// <remarks>
/// This class is used to store information about an EVE Online character, including
/// the character's name, associated log file path, and bounty-related data.
/// It provides properties for tracking the character's total bounties and session-specific bounties,
/// as well as the last recorded position in the character's log file.
/// </remarks>
public class CharacterBounty
{
    /// <summary>
    /// Gets or sets the name of the character being tracked for bounties.
    /// </summary>
    /// <remarks>
    /// This property is used to store the name of an EVE Online character whose
    /// bounty-related data is being monitored. It serves as a key identifier
    /// for a character within the bounty tracking system.
    /// </remarks>
    public string CharacterName { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the path to the log file associated with the character being tracked.
    /// </summary>
    public string LogFilePath { get; set; } = "";

    /// <summary>
    /// Gets or sets the last recorded position within the character's log file.
    /// </summary>
    /// <remarks>
    /// This property holds the byte offset into the log file where the last read operation ended.
    /// It is used to track the progress of log file reading and ensures continuity
    /// when monitoring log files for bounty-related events in EVE Online.
    /// </remarks>
    public long LastPosition { get; set; }
    
    /// <summary>
    /// Gets or sets the total bounty accumulated for the character.
    /// </summary>
    public decimal TotalBounty { get; set; }
    
    /// <summary>
    /// Gets or sets the session-specific bounty accumulated for the character.
    /// </summary>
    public decimal SessionTotalBounty { get; set; }
}
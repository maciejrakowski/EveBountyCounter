using System.ComponentModel;
using EveBountyCounter.Counter;

namespace EveBountyCounter;

/// <summary>
/// Represents a worker component for tracking and logging bounty-related events
/// for characters in the EVE Online game. Inherits from <see cref="BackgroundWorker" />.
/// </summary>
public class CounterWorker : BackgroundWorker
{
    private readonly BountyWatcher _bountyWatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="CounterWorker" /> class.
    /// </summary>
    /// <param name="logsDirectory">The directory path for game logs.</param>
    public CounterWorker(string logsDirectory)
    {
        _bountyWatcher = new BountyWatcher(logsDirectory);

        _bountyWatcher.CharacterTrackingStarted += (sender, args) =>
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {args.CharacterBounty.CharacterName}: tracking");
        };

        _bountyWatcher.CharacterUndocking += (sender, args) =>
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {args.CharacterBounty.CharacterName}: undocking");
        };

        _bountyWatcher.CharacterBountyUpdated += (sender, args) =>
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {args.CharacterBounty.CharacterName}: bounty {args.CharacterBounty.TotalBounty:N0} ISK; {args.CharacterBounty.TotalBounty}; session total bounty {args.CharacterBounty.SessionTotalBounty:N0} ISK");
        };
    }

    /// <summary>
    /// Starts the monitoring process for tracking bounty-related events in game logs.
    /// </summary>
    public void Start()
    {
        _bountyWatcher.Start();
    }

    /// <summary>
    /// Stops the monitoring process for tracking bounty-related events in game logs.
    /// </summary>   
    public void Stop()
    {
        _bountyWatcher.Stop();
    }
}
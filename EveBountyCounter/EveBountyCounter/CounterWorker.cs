using System.ComponentModel;
using EveBountyCounter.Counter;
using EveBountyCounter.Counter.Events;

namespace EveBountyCounter;

/// <summary>
/// Represents a worker component for tracking and logging bounty-related events
/// for characters in the EVE Online game. Inherits from <see cref="BackgroundWorker" />.
/// </summary>
public class CounterWorker : BackgroundWorker
{
    private readonly BountyWatcher _bountyWatcher;
    private bool _userInput = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="CounterWorker" /> class.
    /// </summary>
    /// <param name="logsDirectory">The directory path for game logs.</param>
    public CounterWorker(string logsDirectory)
    {
        _bountyWatcher = new BountyWatcher(logsDirectory);

        _bountyWatcher.CharacterTrackingStarted += OnBountyWatcherOnCharacterTrackingStarted;

        _bountyWatcher.CharacterUndocking += OnBountyWatcherOnCharacterUndocking;

        _bountyWatcher.CharacterBountyUpdated += OnBountyWatcherOnCharacterBountyUpdated;
    }

    private void OnBountyWatcherOnCharacterTrackingStarted(object? sender, CharacterTrackingEventArgs args)
    {
        if (!_userInput)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {args.CharacterBounty.CharacterName}: tracking");
        }
    }

    private void OnBountyWatcherOnCharacterUndocking(object? sender, CharacterTrackingEventArgs args)
    {
        if (!_userInput)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {args.CharacterBounty.CharacterName}: undocking");
        }
    }

    private void OnBountyWatcherOnCharacterBountyUpdated(object? sender, CharacterBountyUpdatedEventArgs args)
    {
        if (!_userInput)
        {
            Console.WriteLine(
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {args.CharacterBounty.CharacterName}: Bounty added {args.BountyIncrease:N0} ISK: Bounty {args.CharacterBounty.TotalBounty:N0} ISK; {args.CharacterBounty.TotalBounty} ; Session total bounty {args.CharacterBounty.SessionTotalBounty:N0} ISK");
        }
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

    /// <summary>
    /// Resets the bounty of a specified character or allows the user to select and reset
    /// bounties for characters with outstanding bounties.
    /// </summary>
    /// <remarks>
    /// This method retrieves all characters with bounties, provides the user with the option
    /// to select a character if there are multiple, and resets the bounty for the chosen
    /// character via the <see cref="BountyWatcher"/> instance.
    /// </remarks>
    public void ResetCharacterBounty()
    {
        Console.WriteLine();
        _userInput = true;

        var charactersWithBounties = _bountyWatcher.GetCharacterBounties()
            .Where(x => x.Value.TotalBounty > 0)
            .ToDictionary();

        if (charactersWithBounties.Count == 0)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: No characters to reset bounty");
        }

        int characterIndex;
        if (charactersWithBounties.Count == 1)
        {
            characterIndex = 0;
        }
        else
        {
            for (int i = 0; i < charactersWithBounties.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {charactersWithBounties.ElementAt(i).Key}: Bounty: {charactersWithBounties.ElementAt(i).Value.TotalBounty:N0} ISK");
            }
        
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Enter the number of the character to reset bounty:");
            var userInput = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(userInput, out characterIndex))
            {
                characterIndex -= 1;
            }
            else
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Invalid input");
            }
        }

        var characterName = charactersWithBounties.ElementAt(characterIndex).Key;

        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Resetting bounty");
        _bountyWatcher.ResetCharacterBounty(characterName);
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Bounty reset");

        _userInput = false;
    }
}
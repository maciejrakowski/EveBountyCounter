using System.ComponentModel;
using System.Globalization;
using EveBountyCounter.Counter;
using EveBountyCounter.Counter.Events;
using EveBountyCounter.EwbApiClient;
using EveBountyHunter.Configuration;

namespace EveBountyCounter;

/// <summary>
/// Represents a worker component for tracking and logging bounty-related events
/// for characters in the EVE Online game. Inherits from <see cref="BackgroundWorker" />.
/// </summary>
public class CounterWorker : BackgroundWorker
{
    private readonly IEwbApiClient _ewbApiClient;
    private readonly BountyWatcher _bountyWatcher;
    private readonly IEbhConfiguration _ebhConfiguration;
    private bool _outputToConsole = true;

    /// <summary>
    /// A worker class that performs asynchronous operations to monitor and log bounty-related events
    /// for characters in the EVE Online game. Inherits functionality from <see cref="BackgroundWorker" />.
    /// </summary>
    public CounterWorker(IEbhConfiguration ebhConfiguration, IEwbApiClient ewbApiClient)
    {
        _ewbApiClient = ewbApiClient;
        _ebhConfiguration = ebhConfiguration;

        _bountyWatcher = new BountyWatcher(_ebhConfiguration.GetConfiguration()!.LogsDirectory);

        _bountyWatcher.CharacterTrackingStarted += OnBountyWatcherOnCharacterTrackingStarted;

        _bountyWatcher.CharacterUndocking += OnBountyWatcherOnCharacterUndocking;

        _bountyWatcher.CharacterBountyUpdated += OnBountyWatcherOnCharacterBountyUpdated;
    }

    private void OnBountyWatcherOnCharacterTrackingStarted(object? sender, CharacterTrackingEventArgs args)
    {
        if (_outputToConsole)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {args.CharacterBounty.CharacterName}: tracking");
        }
    }

    private void OnBountyWatcherOnCharacterUndocking(object? sender, CharacterTrackingEventArgs args)
    {
        if (_outputToConsole)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {args.CharacterBounty.CharacterName}: undocking");
        }
    }

    private void OnBountyWatcherOnCharacterBountyUpdated(object? sender, CharacterBountyUpdatedEventArgs args)
    {
        if (_outputToConsole)
        {
            Console.WriteLine(
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {args.CharacterBounty.CharacterName}: Bounty added {args.BountyIncrease:N2} ISK: Bounty {args.CharacterBounty.TotalBounty:N2} ISK; {args.CharacterBounty.TotalBounty.ToString(new CultureInfo("en-US"))} ; Session total bounty {args.CharacterBounty.SessionTotalBounty:N2} ISK");
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
    /// Disables console output by setting the internal flag to false.
    /// This can be used to temporarily suppress logging or output to the console.
    /// </summary>
    public void PauseConsoleOutput()
    {
        _outputToConsole = false;
    }

    /// <summary>
    /// Enables console output by setting the internal flag to true.
    /// This can be used to resume logging or output to the console.
    /// </summary>   
    public void ResumeConsoleOutput()
    {
        _outputToConsole = true;
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
        try
        {
            PauseConsoleOutput();
            Console.WriteLine();

            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Resetting bounty:");

            var characterName = GetCharacterForUserInput();
            if (characterName is null)
            {
                return;
            }

            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Resetting bounty");
            _bountyWatcher.ResetCharacterBounty(characterName);
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Bounty reset");
        }
        catch
        {
            // ignored
        }
        finally
        {
            ResumeConsoleOutput();
        }
    }

    /// <summary>
    /// Submits the bounty for a character to the EVE Workbench API.
    /// </summary>
    /// <remarks>
    /// This method retrieves the bounty information for a specified character, validates the presence of an API key,
    /// and posts the bounty data to the EVEWorkbench real-time bounty update endpoint. 
    /// </remarks>
    public async Task SubmitBountyAsync()
    {
        try
        {
            PauseConsoleOutput();

            Console.WriteLine();

            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Submitting bounty:");
            var characterName = GetCharacterForUserInput();
            if (characterName is null)
            {
                return;
            }

            var bounty = _bountyWatcher.GetCharacterBounty(characterName);
            if (bounty is null)
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: No bounty found");
                return;
            }

            var character = _ebhConfiguration.GetCharacter(characterName);
            if (character is null)
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: No API key found");
                return;
            }

            if (character.CharacterId is null)
            {
                var characters = await _ewbApiClient.GetCharactersAsync(character.ApiKey);

                foreach (var c in characters)
                {
                    _ebhConfiguration.AddApiKey(c.Name.Trim(), c.Id, character.ApiKey);
                }
                character = _ebhConfiguration.GetCharacter(characterName);
                
                if (character is null)
                {
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: character info update failed. Please try again.");
                    return;
                }
            }
            
            var submitted = await _ewbApiClient.SubmitBountyAsync(character.ApiKey, character.CharacterId.ToString()!, bounty.TotalBounty);

            if (!submitted)
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Error submitting bounties");
                return;
            }

            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Bounty submitted");
        }
        finally
        {
            ResumeConsoleOutput();
        }
    }

    /// <summary>
    /// Retrieves the name of the character selected by the user based on the input.
    /// </summary>
    /// <returns>
    /// The name of the selected character as a string, or null if no selectable characters are available.
    /// </returns>
    private string? GetCharacterForUserInput()
    {
        var charactersWithBounties = _bountyWatcher.GetCharactersBounties()
            .Where(x => x.Value.TotalBounty > 0)
            .ToDictionary();

        if (charactersWithBounties.Count == 0)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: No characters to select");
            return null;
        }

        int characterIndex;
        if (charactersWithBounties.Count == 1)
        {
            characterIndex = 0;
        }
        else
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Select a character:");
            for (int i = 0; i < charactersWithBounties.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {charactersWithBounties.ElementAt(i).Key}: Bounty: {charactersWithBounties.ElementAt(i).Value.TotalBounty:N2} ISK");
            }

            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Enter the number of the character:");
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

        return characterName;
    }
}
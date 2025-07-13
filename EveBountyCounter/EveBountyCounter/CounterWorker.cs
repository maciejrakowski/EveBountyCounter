using System.ComponentModel;
using System.Globalization;
using EveBountyCounter.Counter;
using EveBountyCounter.Counter.Events;
using EveBountyHunter.Configuration;

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

        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Resetting bounty:");

        var characterName = GetCharacterForUserInput();
        if (characterName is null)
        {
            _userInput = false;
            return;
        }

        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Resetting bounty");
        _bountyWatcher.ResetCharacterBounty(characterName);
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Bounty reset");

        _userInput = false;
    }

    /// <summary>
    /// Submits the bounty for a character to the EVE Workbench API.
    /// </summary>
    /// <remarks>
    /// This method retrieves the bounty information for a specified character, validates the presence of an API key,
    /// and posts the bounty data to the EVEWorkbench real-time bounty update endpoint. 
    /// </remarks>
    public void SubmitBounty()
    {
        Console.WriteLine();
        _userInput = true;

        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Submitting bounty:");
        var characterName = GetCharacterForUserInput();
        if (characterName is null)
        {
            _userInput = false;
            return;
        }

        var bounty = _bountyWatcher.GetCharacterBounty(characterName);
        if (bounty is null)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: No bounty found");
            return;
        }

        var configuration = EbhConfiguration.GetConfiguration();
        var apiKey = configuration?.EveWorkbenchApiKeys.FirstOrDefault(x => x.CharacterName == characterName)?.ApiKey;
        if (apiKey is null)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: No API key found");
            _userInput = false;
            return;
        }

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("Accept", "text/plain");

        var content = new StringContent(bounty.TotalBounty.ToString(new CultureInfo("en-US")));

        var response = client.PostAsync("https://api.eveworkbench.com/v1/eve-journal/realtime-bounty-update", content).Result;
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Error retrieving bounties: {response.StatusCode}");
            _userInput = false;
            return;
        }

        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {characterName}: Bounty submitted");
        _userInput = false;
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
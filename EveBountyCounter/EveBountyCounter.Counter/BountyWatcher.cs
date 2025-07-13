using System.Text.RegularExpressions;
using EveBountyCounter.Counter.Events;
using EveBountyCounter.Counter.Models;

namespace EveBountyCounter.Counter;

/// <summary>
/// Represents a bounty watcher component for monitoring bounty-related events in game logs.
/// </summary>
/// <remarks>
/// This class is used to monitor bounty-related events in game logs, such as when a character
/// starts tracking, undocks, or receives a bounty. It uses a <see cref="FileSystemWatcher" />
/// to monitor for new log files and updates the bounty data for each character.
/// </remarks>
public class BountyWatcher
{
    private readonly string _logsDirectory;

    private readonly FileSystemWatcher _watcher;
    private readonly Dictionary<string, CharacterBounty> _characterBounties = new();

    private static readonly Regex BountyRegex = new(@"<b><color=0xff00aa00>(\d{1,3}(?:[,.\s]\d{3})*(?:[,.]\d{1,2})?|\d+(?:[,.]\d{1,2})?)\s*ISK", RegexOptions.Compiled);
    private static readonly Regex UndockingRegex = new("Undocking from", RegexOptions.Compiled);
    private static readonly Regex ListenerRegex = new(@"^\s*Listener:\s*(.+)", RegexOptions.Compiled);
    private static readonly Regex SessionRegex = new(@"^\s*Session Started:\s*(.+)", RegexOptions.Compiled);

    /// <summary>
    /// Event triggered when character tracking is started.
    /// </summary>
    public event EventHandler<CharacterTrackingEventArgs>? CharacterTrackingStarted;

    /// <summary>
    /// Event triggered when a character undocks.
    /// </summary>
    public event EventHandler<CharacterTrackingEventArgs>? CharacterUndocking;

    /// <summary>
    /// Event triggered when a character receives a bounty.
    /// </summary>
    public event EventHandler<CharacterBountyUpdatedEventArgs>? CharacterBountyUpdated;

    /// <summary>
    /// Invokes the CharacterTrackingStarted event when a character's bounty tracking begins.
    /// </summary>
    /// <param name="characterBounty">The <see cref="CharacterBounty"/> object containing information about the character whose bounty tracking has started.</param>
    private void OnCharacterTrackingStarted(CharacterBounty characterBounty)
    {
        CharacterTrackingStarted?.Invoke(null, new CharacterTrackingEventArgs(characterBounty));
    }

    /// <summary>
    /// Invokes the CharacterUndocking event when a character undocks.
    /// </summary>
    /// <param name="characterBounty">The <see cref="CharacterBounty"/> object containing information about the character whose bounty tracking has started.</param>
    private void OnCharacterUndocking(CharacterBounty characterBounty)
    {
        CharacterUndocking?.Invoke(null, new CharacterTrackingEventArgs(characterBounty));
    }

    /// <summary>
    /// Invokes the CharacterBountyUpdated event when a character's bounty is updated.
    /// </summary>
    /// <param name="characterBounty">The <see cref="CharacterBounty"/> object containing information about the character whose bounty was updated.</param>
    /// <param name="bountyIncrease">The amount by which the character's bounty has increased.</param>
    private void OnCharacterBountyUpdated(CharacterBounty characterBounty, decimal bountyIncrease)
    {
        CharacterBountyUpdated?.Invoke(null, new CharacterBountyUpdatedEventArgs(characterBounty, bountyIncrease));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BountyWatcher"/> class.
    /// </summary>
    /// <param name="logsDirectory">The path to the directory containing game logs.</param>
    public BountyWatcher(string logsDirectory)
    {
        _logsDirectory = logsDirectory;
        _watcher = new FileSystemWatcher(_logsDirectory)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.txt",
            EnableRaisingEvents = true
        };

        _watcher.Created += (s, e) => { ScanCharacterLogs(_logsDirectory); };

        _watcher.Changed += (s, e) => { CheckLogFileForBounties(e); };
    }

    /// <summary>
    /// Starts the monitoring process for detecting changes in the logs directory
    /// and triggers the scanning of character logs for tracking bounty-related events.
    /// </summary>
    public void Start()
    {
        _watcher.EnableRaisingEvents = true;
        ScanCharacterLogs(_logsDirectory);
    }

    /// <summary>
    /// Stops the monitoring process for detecting changes in the logs directory.
    /// </summary>  
    public void Stop()
    {
        _watcher.EnableRaisingEvents = false;
    }

    /// <summary>
    /// Scans the logs directory for new log files and updates the bounty data for each character.
    /// </summary>
    /// <remarks>
    /// This method scans the logs directory for new log files and updates the bounty data for each character.
    /// It uses a regular expression to extract the name of the character from the log file path,
    /// and uses the extracted name to determine whether the character is already being tracked.
    /// If the character is not being tracked, it creates a new <see cref="CharacterBounty"/> instance
    /// </remarks>
    private void ScanCharacterLogs(string logsDirectory)
    {
        var files = Directory.GetFiles(logsDirectory, "*.txt");
        var latestFiles = new Dictionary<string, (string file, DateTime sessionTime)>();

        foreach (var file in files)
        {
            try
            {
                using var reader =
                    new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                string? listener = null;
                DateTime? sessionTime = null;

                while (reader.ReadLine() is { } line)
                {
                    var listenerMatch = ListenerRegex.Match(line);
                    if (listenerMatch.Success)
                    {
                        listener = listenerMatch.Groups[1].Value.Trim();
                    }

                    var sessionMatch = SessionRegex.Match(line);
                    if (sessionMatch.Success && DateTime.TryParse(sessionMatch.Groups[1].Value.Trim(), out var time))
                    {
                        sessionTime = time;
                    }

                    if (listener is not null && sessionTime is not null)
                    {
                        break;
                    }
                }

                if (listener == null || sessionTime == null)
                {
                    continue;
                }

                if (!latestFiles.ContainsKey(listener) || sessionTime > latestFiles[listener].sessionTime)
                {
                    latestFiles[listener] = (file, sessionTime.Value);
                }
            }
            catch
            {
                // ignored
            }
        }

        foreach (var (character, (filePath, _)) in latestFiles)
        {
            if (_characterBounties.ContainsKey(character) && _characterBounties[character].LogFilePath == filePath)
            {
                continue;
            }

            var characterBounty = new CharacterBounty()
            {
                CharacterName = character,
                LogFilePath = filePath,
                LastPosition = new FileInfo(filePath).Length,
                TotalBounty = 0,
                SessionTotalBounty = 0
            };
            _characterBounties[character] = characterBounty;

            OnCharacterTrackingStarted(characterBounty);
        }
    }

    /// <summary>
    /// Checks a log file for bounty-related events and updates the bounty data for the character.
    /// </summary>
    /// <param name="fileSystemEventArgs">The <see cref="FileSystemEventArgs"/> object containing information about the log file that was changed.</param>
    private void CheckLogFileForBounties(FileSystemEventArgs fileSystemEventArgs)
    {
        var characterBounty = _characterBounties.Values.FirstOrDefault(x => x.LogFilePath == fileSystemEventArgs.FullPath);
        if (characterBounty is null)
        {
            return;
        }

        try
        {
            using var stream = new FileStream(characterBounty.LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            stream.Seek(characterBounty.LastPosition, SeekOrigin.Begin);

            using var reader = new StreamReader(stream);
            while (reader.ReadLine() is { } line)
            {
                if (UndockingRegex.IsMatch(line))
                {
                    characterBounty.TotalBounty = 0;
                    OnCharacterUndocking(characterBounty);
                    continue;
                }

                var match = BountyRegex.Match(line);
                if (!match.Success)
                {
                    continue;
                }


                var bounty = GetBountyFromRegexMatch(match);
                if (bounty > 0)
                {
                    characterBounty.TotalBounty += bounty;
                    characterBounty.SessionTotalBounty += bounty;
                    OnCharacterBountyUpdated(characterBounty, bounty);
                }
            }

            characterBounty.LastPosition = stream.Position;
        }
        catch (IOException)
        {
            // File might be temporarily locked; skip
        }
    }

    private decimal GetBountyFromRegexMatch(Match match)
    {
        var valueStr = match.Groups[1].Value;

        // 1.00 must be of length at least = 4
        if (valueStr.Length <= 3)
        {
            return decimal.TryParse(valueStr, out decimal bounty) ? bounty : 0;
        }

        bool isDecimal = valueStr.LastIndexOfAny(['.', ','], valueStr.Length - 3, 1) > 0;

        if (long.TryParse(valueStr.Replace(".", "").Replace(",", ""), out long decimalBounty))
        {
            if (isDecimal)
            {
                return (decimal)decimalBounty / 100;
            }

            return decimalBounty;
        }

        return 0;
    }

    /// <summary>
    /// Retrieves a collection of character names currently being tracked for bounty-related events.
    /// </summary>
    /// <returns>
    /// A collection of strings representing the names of characters currently being tracked.
    /// </returns>
    public IEnumerable<string> GetTrackedCharacters()
    {
        return _characterBounties.Keys;
    }

    /// <summary>
    /// Retrieves the dictionary of character bounties currently being tracked.
    /// </summary>
    /// <returns>
    /// A dictionary where the keys represent character names and the values are <see cref="CharacterBounty"/> objects
    /// containing information about the respective character's bounty tracking.
    /// </returns>
    public IDictionary<string, CharacterBounty> GetCharactersBounties()
    {
        return _characterBounties;
    }

    /// <summary>
    /// Retrieves the bounty data for a specified character.
    /// </summary>
    /// <param name="characterName">The name of the character for which to retrieve bounty data.</param>
    /// <returns>
    /// A <see cref="CharacterBounty"/> object containing information about the specified character's bounty tracking.
    /// </returns>
    public CharacterBounty? GetCharacterBounty(string characterName)
    {
        return _characterBounties.GetValueOrDefault(characterName);
    }

    /// <summary>
    /// Resets the bounty of a specified character by setting their total bounty to zero.
    /// </summary>
    /// <param name="characterName">The name of the character whose bounty is being reset.</param>
    public void ResetCharacterBounty(string characterName)
    {
        if (_characterBounties.TryGetValue(characterName, out var bounty))
        {
            bounty.TotalBounty = 0;
        }
    }
}
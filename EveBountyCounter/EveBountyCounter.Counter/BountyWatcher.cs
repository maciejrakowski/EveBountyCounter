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

    private static readonly Regex BountyRegex = new(@"<b><color=0xff00aa00>([\d,]+) ISK", RegexOptions.Compiled);
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
    public event EventHandler<CharacterTrackingEventArgs>? CharacterBountyUpdated;

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
    /// Invokes the CharacterBountyUpdated event when a character receives a bounty.
    /// </summary>
    /// <param name="characterBounty">The <see cref="CharacterBounty"/> object containing information about the character whose bounty tracking has started.</param>
    private void OnCharacterBountyUpdated(CharacterBounty characterBounty)
    {
        CharacterBountyUpdated?.Invoke(null, new CharacterTrackingEventArgs(characterBounty));
    }

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

        _watcher.Changed += (s, e) => { MonitorAllLogs(); };
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
    /// Monitors all logs in the logs directory for changes and updates the bounty data for each character.
    /// </summary>
    /// <remarks>
    /// This method monitors all logs in the logs directory for changes and updates the bounty data for each character.
    /// It uses a regular expression to extract the bounty value from the log line, and uses the extracted value to update the bounty data.
    /// If the log line does not contain a bounty value, it skips the line.
    /// </remarks>   
    private void MonitorAllLogs()
    {
        foreach (var characterBounty in _characterBounties.Values)
        {
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

                    var valueStr = match.Groups[1].Value.Replace(",", "");
                    if (long.TryParse(valueStr, out long bounty))
                    {
                        characterBounty.TotalBounty += bounty;
                        characterBounty.SessionTotalBounty += bounty;
                        OnCharacterBountyUpdated(characterBounty);
                    }
                }

                characterBounty.LastPosition = stream.Position;
            }
            catch (IOException)
            {
                // File might be temporarily locked; skip
            }
        }
    }
}
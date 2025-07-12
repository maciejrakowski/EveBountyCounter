using EveBountyHunter.Configuration;
using EveBountyHunter.Configuration.Models;

namespace EveBountyCounter;

/// <summary>
/// Provides setup and configuration management for the EVE Bounty Counter application.
/// </summary>
public static class ConfigurationSetup
{
    /// <summary>
    /// Retrieves the directory path for game logs used by the application.
    /// </summary>
    /// <returns>
    /// The directory path for game logs as a string. If a valid configuration is found,
    /// it returns the configured logs directory. Otherwise, prompts the user to input a valid logs directory path.
    /// </returns>
    public static string GetLogsDirectory()
    {
        var config = EbhConfiguration.GetConfiguration();

        if (config is not null && !string.IsNullOrEmpty(config.LogsDirectory))
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Current logs directory: {config.LogsDirectory}");
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Press Enter to use current path, or type a new path (most likely: {Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\EVE\\logs\\Gamelogs).");
            Console.Write($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: ");

            var userInput = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(userInput) && Directory.Exists(config.LogsDirectory))
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Using existing path: {config.LogsDirectory}");
                return config.LogsDirectory;
            }

            // User entered a new path
            return ValidateAndGetPath(userInput);
        }

        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: No configuration found. Please enter the logs directory path  (most likely: {Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\EVE\\logs\\Gamelogs):");

        return GetPathFromUser();
    }

    /// <summary>
    /// Prompts the user to input a directory path for game logs.
    /// </summary>
    /// <returns>
    /// The directory path entered by the user as a string. Ensures the input is not empty and processes the path for validation.
    /// </returns>
    private static string GetPathFromUser()
    {
        while (true)
        {
            Console.Write($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Enter logs directory path: ");
            string userInput = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Path cannot be empty. Please try again.");
                continue;
            }

            return ValidateAndGetPath(userInput);
        }
    }

    /// <summary>
    /// Validates the provided directory path and ensures it exists. If the path is valid, updates the application's configuration with the new logs directory.
    /// </summary>
    /// <param name="path">The directory path to validate and update.</param>
    /// <returns>
    /// The validated directory path as a string. If the provided path is valid, it updates the configuration and returns the path; otherwise, prompts for a new valid path.
    /// </returns>
    private static string ValidateAndGetPath(string path)
    {
        while (true)
        {
            if (Directory.Exists(path))
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Path validated: {path}");

                var config = new EveBountyCounterConfiguration
                {
                    LogsDirectory = path
                };
                EbhConfiguration.SaveConfiguration(config);
                
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Configuration saved.");

                return path;
            }

            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Directory '{path}' does not exist. Please enter a valid path:");
            Console.Write($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Enter logs directory path: ");
            path = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Path cannot be empty. Please try again.");
            }
        }
    }
}
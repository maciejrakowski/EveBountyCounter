namespace EveBountyCounter;

/// <summary>
/// Represents the main application class.
/// </summary>
public class ConsoleApplication(
    IConfigurationSetup configurationSetup,
    CounterWorker worker)
{
    /// <summary>
    /// Executes the main workflow of the console application.
    /// </summary>
    public async Task RunAsync()
    {
        configurationSetup.GetLogsDirectory();

        worker.Start();

        Console.WriteLine("EVE Bounty Counter started. Press H for help, Q or ESC to quit.");

        ConsoleKey key = ConsoleKey.None;
        while (key != ConsoleKey.Q && key != ConsoleKey.Escape)
        {
            key = Console.ReadKey().Key;

            switch (key)
            {
                case ConsoleKey.H:
                    ShowHelp();
                    break;
                case ConsoleKey.R:
                    worker.ResetCharacterBounty();
                    break;
                case ConsoleKey.C:
                    await HandleAddApiKeyAsync();
                    break;
                case ConsoleKey.S:
                    await worker.SubmitBountyAsync();
                    break;
                // case ConsoleKey.L:
                //     HandleUpdateLogsDirectory();
                //     break;
            }
        }

        worker.Stop();
    }

    /// <summary>
    /// Displays the help information for available commands in the console application.
    /// </summary>
    private void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("EVE Bounty Counter");
        Console.WriteLine("H - Help (this screen)");
        Console.WriteLine("R - Reset character bounty");
        Console.WriteLine("C - Add API key");
        Console.WriteLine("S - Submit bounty");
        Console.WriteLine("L - Update Logs Directory");
        Console.WriteLine("Q or ESC - Quit");
    }

    /// <summary>
    /// Handles the process for adding an API key to the configuration setup.
    /// </summary>
    /// <returns>A task representing the asynchronous operation of adding the API key.</returns>
    private async Task HandleAddApiKeyAsync()
    {
        try
        {
            worker.PauseConsoleOutput();
            await configurationSetup.AddApiKey();
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            worker.ResumeConsoleOutput();
        }
    }
}
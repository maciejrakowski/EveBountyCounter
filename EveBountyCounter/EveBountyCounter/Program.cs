using EveBountyCounter;

internal class Program
{
    static void Main()
    {
        string logsDirectory = ConfigurationSetup.GetLogsDirectory();

        CounterWorker worker = new(logsDirectory);
        worker.Start();

        ConsoleKey key = ConsoleKey.None;
        while (key != ConsoleKey.Q && key != ConsoleKey.Escape)
        {
            key = Console.ReadKey().Key;

            switch (key)
            {
                case ConsoleKey.H:
                    Console.WriteLine();
                    Console.WriteLine("EVE Bounty Counter");
                    Console.WriteLine("H - Help (this screen)");
                    Console.WriteLine("R - Reset character bounty");
                    Console.WriteLine("C - Add API key");
                    Console.WriteLine("S - Submit bounty");
                    Console.WriteLine("L - Update Logs Directory");
                    Console.WriteLine("Q or ESC - Quit");
                    break;
                case ConsoleKey.R:
                    worker.ResetCharacterBounty();
                    break;
                case ConsoleKey.C:
                    try
                    {
                        worker.PauseConsoleOutput();
                        ConfigurationSetup.AddApiKey();
                    }
                    finally
                    {
                        worker.ResumeConsoleOutput();
                    }

                    break;
                case ConsoleKey.S:
                    worker.SubmitBounty();
                    break;
                case ConsoleKey.L:
                    try
                    {
                        worker.PauseConsoleOutput();
                        var newLogsDirectory = ConfigurationSetup.GetLogsDirectoryFromUser();
                        worker.Stop();
                        worker = new CounterWorker(newLogsDirectory);
                        worker.Start();
                    }
                    finally
                    {
                        worker.ResumeConsoleOutput();
                    }

                    break;
            }
        }

        worker.Stop();
    }
}
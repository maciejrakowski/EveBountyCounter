using EveBountyCounter;

internal class Program
{
    static void Main()
    {
        string logsDirectory = ConfigurationSetup.GetLogsDirectory();
        
        CounterWorker worker = new(logsDirectory);
        worker.Start();

        while (Console.ReadKey().Key != ConsoleKey.Q && Console.ReadKey().Key != ConsoleKey.Escape)
        {
        }
        
        worker.Stop();
    }
}



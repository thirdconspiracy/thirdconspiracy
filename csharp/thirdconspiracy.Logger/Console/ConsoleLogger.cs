using System;

namespace thirdconspiracy.Logger.Console
{
    public class ConsoleLogger : ILogger
    {
        public void Log(LogLevel level, string message, Exception exception)
        {
            System.Console.WriteLine($"{level:G} {DateTimeOffset.UtcNow}");
            System.Console.WriteLine(message);
            if (exception != null)
            {
                System.Console.WriteLine(exception.ToString());
            }
        }
    }
}
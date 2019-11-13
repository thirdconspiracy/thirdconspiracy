using System;

namespace thirdconspiracy.Logger.Null
{
    public class NullLogger : ILogger
    {
        public void Log(LogLevel level, string message, Exception exception)
        {
            //Do Nothing
        }
    }
}
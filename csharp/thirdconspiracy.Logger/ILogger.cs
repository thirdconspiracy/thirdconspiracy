using System;

namespace thirdconspiracy.Logger
{
    public interface ILogger
    {
        void Log(LogLevel level, string message, Exception exception);
    }
}
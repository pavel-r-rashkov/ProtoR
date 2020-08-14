namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Apache.Ignite.Core.Log;
    using Serilog.Events;

    public class IgniteSerilogLogger : ILogger
    {
        private static readonly Dictionary<LogLevel, LogEventLevel> LogLevelMap = new Dictionary<LogLevel, LogEventLevel>
        {
            { LogLevel.Trace, LogEventLevel.Verbose },
            { LogLevel.Debug, LogEventLevel.Debug },
            { LogLevel.Info, LogEventLevel.Information },
            { LogLevel.Warn, LogEventLevel.Warning },
            { LogLevel.Error, LogEventLevel.Error },
        };

        private readonly Serilog.ILogger serilog;

        public IgniteSerilogLogger(Serilog.ILogger serilog)
        {
            this.serilog = serilog.ForContext(
                Serilog.Core.Constants.SourceContextPropertyName,
                CacheConstants.IgniteLogSource);
        }

        public bool IsEnabled(LogLevel level)
        {
            return this.serilog.IsEnabled(LogLevelMap[level]);
        }

        public void Log(LogLevel level, string message, object[] args, IFormatProvider formatProvider, string category, string nativeErrorInfo, Exception exception)
        {
            this.serilog.Write(LogLevelMap[level], exception, message, args);
        }
    }
}

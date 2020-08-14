namespace ProtoR.Web.Infrastructure
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Events;

    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration CustomConfiguration(this LoggerConfiguration logConfiguration)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables(prefix: "PROTOR_")
                .Build();

            var logLevelConfiguration = configuration.GetValue<string>("Logging:MinLevel");
            var isValidLevel = Enum.TryParse(logLevelConfiguration, true, out LogEventLevel logLevel);
            var logLocation = configuration.GetValue<string>("Logging:LogFileLocation");
            var template = "[{Timestamp:HH:mm:ss}][{Level:u3}][{SourceContext}] {Message}{NewLine}{Exception}";

            logConfiguration
                .MinimumLevel.Is(isValidLevel ? logLevel : LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: template);

            if (!string.IsNullOrWhiteSpace(logLocation))
            {
                logConfiguration.WriteTo.File(
                    logLocation,
                    outputTemplate: template,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 3);
            }

            return logConfiguration;
        }
    }
}

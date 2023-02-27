using System;
using Serilog;
using Serilog.Events;
using System.Reflection;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace Travel.WebApi.Helpers
{
	public static class SerilogConfiguration
	{
		public static void Configure(ILoggingBuilder loggingBuilder)
		{
            var name = Assembly.GetExecutingAssembly().GetName();
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Assembly", $"{name.Name}")
                .Enrich.WithProperty("Assembly", $"{name.Version}")
                .WriteTo.SQLite(
                        Environment.CurrentDirectory + @"/Logs/log.db",
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        storeTimestampInUtc: true)
                .WriteTo.File(
                        new CompactJsonFormatter(),
                        Environment.CurrentDirectory + @"/Logs/log.json",
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Console()
                .CreateLogger();

            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(logger);

        }
	}
}


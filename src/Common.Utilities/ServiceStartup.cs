using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Formatting.Json;
using Serilog.Sinks.SumoLogic;

namespace Common.Utilities
{
    public class ServiceStartup
    {
        public static void UseSerilogOnHost(WebApplicationBuilder builder)
        {
            // this approach doesn't work in Blazor for an unknown reason.
            // Seems to be a problem with adding serilog to the host - builder.Host.UseSerilog(Log.Logger);

            // https://blog.datalust.co/using-serilog-in-net-6/
            builder.Host.UseSerilog((context, lc) =>
            {
                ConfigureLogger(lc, builder.Configuration);
            });
        }

        public static void ConfigureLogger(LoggerConfiguration lc, ConfigurationManager config)
        {
            // in PROD console should be set to Error (can hurt perf, runs on a blocking thread)
            Enum.TryParse(config["Serilog:SinkLevel:Console"], out LogEventLevel consoleLevel);
            Enum.TryParse(config["Serilog:SinkLevel:SumoLogic"], out LogEventLevel sumoLogicLevel);

            lc.Enrich.WithMachineName()
                // https://github.com/ekmsystems/serilog-enrichers-correlation-id
                // when searching, the second section of the traceparent is the trace-id, use just this section to see all related logs
                .Enrich.WithCorrelationIdHeader(config["Serilog:CorrelationIdField"] ?? "x-correlation-id") // https://www.w3.org/TR/trace-context/#traceparent-header
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder() // https://github.com/RehanSaeed/Serilog.Exceptions
                    .WithDefaultDestructurers()
                    .WithDestructuringDepth(8))
                .WriteTo.Console(restrictedToMinimumLevel: consoleLevel)
                .WriteTo.SumoLogic(restrictedToMinimumLevel: sumoLogicLevel,
                    endpointUrl: config["Serilog:write-to:SumoLogic.Url"],
                    sourceName: config["Serilog:SourceName"] ?? "InvalidSource",
                    sourceCategory: config["Serilog:Category"],
                    textFormatter: new JsonFormatter())
                .Destructure.ToMaximumDepth(8);
        }
    }
}
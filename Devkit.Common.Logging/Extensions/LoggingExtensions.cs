using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Devkit.Common.Logging.Options;
using Microsoft.Extensions.Logging;

namespace Devkit.Common.Logging.Extensions;

public static class LoggingExtensions
{
    public static IServiceCollection AddCustomLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection("LoggingOptions").Get<LoggingOptions>();

        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", options!.ApplicationName)
            .Enrich.WithExceptionDetails() 
            .Enrich.WithClientIp() 
            .Enrich.WithRequestHeader("User-Agent", "UserAgent") 
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console();

        if (options.Seq is { IsEnabled: true })
        {
            loggerConfiguration.WriteTo.Seq(
                serverUrl: options.Seq.ServerUrl,
                apiKey: options.Seq.ApiKey);
        }

        if (options.Elastic is { IsEnabled: true })
        {
            loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(options.Elastic.NodeUrl))
            {
                AutoRegisterTemplate = true,
                IndexFormat = options.Elastic.IndexFormat ?? "devkit-logs-{0:yyyy.MM.dd}"
            });
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger);
        });

        return services;
    }
}
using Devkit.Common.Jobs.Core;
using Devkit.Common.Jobs.Options;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Jobs.Providers.Hangfire;

public class HangfireJobProvider : IJobProvider
{
    public string ProviderName => "Hangfire";

    public void Configure(IServiceCollection services, JobOptions options)
    {
        services.AddHangfire(config =>
        {
            switch (options.Storage.Type.ToLowerInvariant())
            {
                case "postgresql":
                    config.UsePostgreSqlStorage(options.Storage.ConnectionString);
                    break;

                case "mssql":
                    config.UseSqlServerStorage(options.Storage.ConnectionString);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported Hangfire storage type: {options.Storage.Type}");
            }
        });

        services.AddHangfireServer();
    }

    public void Schedule(Type jobType, IServiceProvider serviceProvider, string cronExpression, bool useQueue)
    {
        using var scope = serviceProvider.CreateScope();
        var jobInstance = scope.ServiceProvider.GetService(jobType);

        switch (jobInstance)
        {
            case ILocalJob localJob:
                RecurringJob.AddOrUpdate(jobType.FullName!, () => localJob.RunAsync(CancellationToken.None), cronExpression);
                break;
            case IQueueJob queueJob when useQueue:
                RecurringJob.AddOrUpdate(jobType.FullName!, () => queueJob.ExecuteAsync(null, CancellationToken.None), cronExpression);
                break;
        }
    }
}
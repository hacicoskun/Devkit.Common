using Devkit.Common.Jobs.Core;
using Devkit.Common.Jobs.Options;
using Devkit.Common.Jobs.Providers.Hangfire;
using Devkit.Common.Jobs.Providers.Quartz;
using Devkit.Common.Jobs.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;

namespace Devkit.Common.Jobs.Extensions;

public static class JobServiceCollectionExtensions
{
    public static IServiceCollection AddJobScheduler(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection("Jobs").Get<JobOptions>()
                      ?? throw new InvalidOperationException("Jobs configuration missing.");

        IJobProvider provider = options.Provider.ToLowerInvariant() switch
        {
            "hangfire" => new HangfireJobProvider(),
            "quartz" => new QuartzJobProvider(),
            _ => throw new NotSupportedException($"Unsupported job provider: {options.Provider}")
        };

        provider.Configure(services, options);

        services.AddSingleton(options);
        services.AddSingleton(provider);
        services.AddSingleton<JobSchedulerService>();

        if (options.Provider.Equals("Hangfire", StringComparison.OrdinalIgnoreCase))
        {
            services.AddHangfireServer();
        }
        services.AddHostedService<JobSchedulerHostedService>();

        return services;
    }
}
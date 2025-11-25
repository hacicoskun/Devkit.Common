using Microsoft.Extensions.Hosting;

namespace Devkit.Common.Jobs.Services;

public class JobSchedulerHostedService(IServiceProvider serviceProvider, JobSchedulerService scheduler)
    : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        scheduler.ScheduleConfiguredJobs();
        Console.WriteLine("Jobs scheduled automatically on startup.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
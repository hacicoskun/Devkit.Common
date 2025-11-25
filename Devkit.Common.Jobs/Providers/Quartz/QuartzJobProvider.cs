using Devkit.Common.Jobs.Core;
using Devkit.Common.Jobs.Options;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Devkit.Common.Jobs.Providers.Quartz;

public class QuartzJobProvider : IJobProvider
{
    public string ProviderName => "Quartz";

    public void Configure(IServiceCollection services, JobOptions options)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
        });

        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
    }

    public void Schedule(Type jobType, IServiceProvider serviceProvider, string cronExpression, bool useQueue)
    {
        var schedulerFactory = serviceProvider.GetRequiredService<ISchedulerFactory>();
        var scheduler = schedulerFactory.GetScheduler().Result;

        var job = JobBuilder.Create(jobType)
            .WithIdentity(jobType.FullName)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithCronSchedule(cronExpression)
            .Build();

        scheduler.ScheduleJob(job, trigger).Wait();
        scheduler.Start().Wait();
    }
}
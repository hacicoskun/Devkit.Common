using Devkit.Common.Jobs.Core;
using Devkit.Common.Jobs.Options;

namespace Devkit.Common.Jobs.Services;

public class JobSchedulerService(IServiceProvider serviceProvider, IJobProvider provider, JobOptions options)
{
    public void ScheduleConfiguredJobs()
    {
        foreach (var job in options.Schedules)
        {
            var jobType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name.Equals(job.JobName, StringComparison.OrdinalIgnoreCase));

            if (jobType == null)
                continue;

            var useQueue = job.UseQueue ?? options.UseQueue;
            provider.Schedule(jobType, serviceProvider, job.Cron, useQueue);
        }
    }
}
using Devkit.Common.Jobs.Core;

namespace Devkit.Sample.Api.Jobs;

public class DateTimeLoggerJob(ILogger<DateTimeLoggerJob> logger) : IQueueJob
{
    public async Task ExecuteAsync(object? payload = null, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("🕒 Şu anki zaman: {Time}", DateTime.Now);
        await Task.CompletedTask;
    }
}
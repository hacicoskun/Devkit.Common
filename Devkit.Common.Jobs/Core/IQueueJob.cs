namespace Devkit.Common.Jobs.Core;

/// <summary>
/// Kuyruk (background) üzerinden çalışan job türü.
/// </summary>
public interface IQueueJob
{
    Task ExecuteAsync(object? payload = null, CancellationToken cancellationToken = default);
}
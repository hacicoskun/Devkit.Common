namespace Devkit.Common.Jobs.Core;

/// <summary>
/// Basit (tek instance çalışan) job türü.
/// </summary>
public interface ILocalJob
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
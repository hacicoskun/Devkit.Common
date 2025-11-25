using Devkit.Common.Jobs.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Jobs.Core;

/// <summary>
/// Job altyapısı (Hangfire, Quartz, vb.) için temel sözleşme.
/// </summary>
public interface IJobProvider
{
    string ProviderName { get; }

    void Configure(IServiceCollection services, JobOptions options);

    void Schedule(Type jobType, IServiceProvider serviceProvider, string cronExpression, bool useQueue);
}
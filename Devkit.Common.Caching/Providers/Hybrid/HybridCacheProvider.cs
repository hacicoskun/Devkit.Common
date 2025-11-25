using Devkit.Common.Caching.Core;
using Microsoft.Extensions.Caching.Hybrid;

namespace Devkit.Common.Caching.Providers.Hybrid;

public class HybridCacheProvider(HybridCache cache) : BaseCacheProvider, ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        key = NormalizeKey(key);

        return await cache.GetOrCreateAsync(key, static ct => ValueTask.FromResult<T?>(null), options: null, tags: null, cancellationToken: cancellationToken
        );
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        key = NormalizeKey(key);

        var options = new HybridCacheEntryOptions
        {
            Expiration = expiration,
            LocalCacheExpiration = expiration
        };

        await cache.SetAsync(key, value, options, [], cancellationToken);
    }

    public  async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        key = NormalizeKey(key);
        await cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        key = NormalizeKey(key);

        var options = new HybridCacheEntryOptions
        {
            Expiration = expiration,
            LocalCacheExpiration = expiration
        };

        return await cache.GetOrCreateAsync(
            key,
            async ct => await factory(),
            options,
            tags: null,
            cancellationToken: cancellationToken
        );
    }
}

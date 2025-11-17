using Devkit.Common.Caching.Providers.Core;
using Devkit.Common.Caching.Providers.InMemory;
using Devkit.Common.Caching.Providers.Redis;

namespace Devkit.Common.Caching.Providers.Hybrid;

public class HybridCacheProvider(InMemoryCacheProvider memory, RedisCacheProvider redis) : BaseCacheProvider, ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class
    {
        key = NormalizeKey(key);
       
        var l1 = await memory.GetAsync<T>(key, cancellationToken);
        if (l1 is not null)
            return l1;

        var l2 = await redis.GetAsync<T>(key, cancellationToken);
        if (l2 is not null)
        {
            await memory.SetAsync(key, l2, cancellationToken: cancellationToken);
        }

        return l2;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        key = NormalizeKey(key);

        await memory.SetAsync(key, value, expiration, cancellationToken);
        await redis.SetAsync(key, value, expiration, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        key = NormalizeKey(key);

        await memory.RemoveAsync(key, cancellationToken);
        await redis.RemoveAsync(key, cancellationToken);
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var newValue = await factory();
        await SetAsync(key, newValue, expiration, cancellationToken);
        return newValue;
    }
}

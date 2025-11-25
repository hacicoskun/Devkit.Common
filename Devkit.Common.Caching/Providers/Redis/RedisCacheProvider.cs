using Devkit.Common.Caching.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace Devkit.Common.Caching.Providers.Redis;

public class RedisCacheProvider(IDistributedCache cache) : BaseCacheProvider, ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class
    {
        key = NormalizeKey(key);

        var data = await cache.GetAsync(key, cancellationToken);
        return Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        key = NormalizeKey(key);

        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);

        var data = Serialize(value);
        await cache.SetAsync(key, data, options, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        key = NormalizeKey(key);
        return cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory,
        TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var value = await factory();
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }
}
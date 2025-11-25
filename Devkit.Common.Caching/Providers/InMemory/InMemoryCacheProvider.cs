using Devkit.Common.Caching.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Devkit.Common.Caching.Providers.InMemory;

public class InMemoryCacheProvider(IMemoryCache cache) : BaseCacheProvider, ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class
    {
        key = NormalizeKey(key);

        if (cache.TryGetValue(key, out T? value))
            return Task.FromResult(value);

        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        key = NormalizeKey(key);

        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);

        cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        key = NormalizeKey(key);
        cache.Remove(key);
        return Task.CompletedTask;
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory,
        TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var existing = await GetAsync<T>(key, cancellationToken);
        if (existing is not null)
            return existing;

        var value = await factory();
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }
}
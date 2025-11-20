using Devkit.Common.Caching.Enums;
using Devkit.Common.Caching.Options;
using Devkit.Common.Caching.Providers.Hybrid;
using Devkit.Common.Caching.Providers.InMemory;
using Devkit.Common.Caching.Providers.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Caching.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCacheProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new CachingOptions();
        configuration.GetSection("Cache").Bind(options);

        services.AddSingleton(options);

        switch (options.Provider)
        {
            case CacheProviderType.Memory:
                services.AddMemoryCache();
                services.AddSingleton<InMemoryCacheProvider>();
                services.AddSingleton<ICacheService>(sp => sp.GetRequiredService<InMemoryCacheProvider>());
                break;

            case CacheProviderType.Redis:
                services.AddStackExchangeRedisCache(redisOptions =>
                {
                    redisOptions.Configuration = options.Redis.ConnectionString;
                });

                services.AddSingleton<RedisCacheProvider>();
                services.AddSingleton<ICacheService>(sp => sp.GetRequiredService<RedisCacheProvider>());
                break;

            case CacheProviderType.Hybrid:
                services.AddHybridCache();
                services.AddSingleton<InMemoryCacheProvider>();

                services.AddStackExchangeRedisCache(redisOptions =>
                {
                    redisOptions.Configuration = options.Redis.ConnectionString;
                });
                services.AddSingleton<RedisCacheProvider>();

                services.AddSingleton<ICacheService, HybridCacheProvider>();
                break;

            default:
                throw new NotSupportedException($"Unsupported cache provider: {options.Provider}");
        } 

        return services;
    }
}
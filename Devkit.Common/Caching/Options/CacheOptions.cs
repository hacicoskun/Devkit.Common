using Devkit.Common.Caching.Enums;

namespace Devkit.Common.Caching.Options;

public class CachingOptions
{
    public CacheProviderType Provider { get; set; } = CacheProviderType.Memory;

    public RedisOptions Redis { get; set; } = new();
    public MemoryOptions Memory { get; set; } = new();
}

public class RedisOptions
{
    public string? ConnectionString { get; set; }
    public int DefaultExpirationSeconds { get; set; } = 3600;
}

public class MemoryOptions
{
    public int DefaultExpirationSeconds { get; set; } = 600;
}
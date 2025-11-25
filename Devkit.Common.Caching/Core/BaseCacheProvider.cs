using System.Text.Json;

namespace Devkit.Common.Caching.Core;

public abstract class BaseCacheProvider
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    protected virtual byte[] Serialize<T>(T value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, _jsonOptions);
    }

    protected virtual T? Deserialize<T>(byte[]? data)
    {
        if (data is null || data.Length == 0)
            return default;

        return JsonSerializer.Deserialize<T>(data, _jsonOptions);
    }

    /// <summary>
    /// Normalize cache key (prefix, trimming, lower-case etc.)
    /// </summary>
    protected virtual string NormalizeKey(string key)
    {
        return key.Trim().ToLowerInvariant();
    }
}
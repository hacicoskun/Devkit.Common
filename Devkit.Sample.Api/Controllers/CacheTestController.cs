using Devkit.Common.Caching;
using Microsoft.AspNetCore.Mvc;

namespace Devkit.Sample.Api.Controllers;

[ApiController]
[Route("cache")]
public class CacheTestController(ICacheService cache) : ControllerBase
{
   [HttpGet("get")]
    public async Task<IActionResult> Get(string key)
    {
        var value = await cache.GetAsync<string>(key);
        return Ok(new
        {
            key,
            value,
            exists = value != null
        });
    }

    [HttpPost("set")]
    public async Task<IActionResult> Set(string key, string value)
    {
        await cache.SetAsync(key, value, TimeSpan.FromMinutes(5));
        return Ok(new
        {
            message = "Value cached",
            key,
            value
        });
    }
    
    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string key)
    {
        await cache.RemoveAsync(key);
        return Ok(new
        {
            message = "Key removed",
            key
        });
    }

    [HttpGet("get-or-create")]
    public async Task<IActionResult> GetOrCreate(string key)
    {
        var value = await cache.GetOrCreateAsync(
            key,
            () => Task.FromResult($"created-at-{DateTime.UtcNow:HH:mm:ss}"),
            TimeSpan.FromMinutes(2)
        );

        return Ok(new
        {
            message = "Value returned",
            key,
            value
        });
    }
}
using Microsoft.Extensions.Caching.Memory;
using ProjectManagement.Application.Abstractions;

namespace ProjectManagement.Infrastructure.Caching;

public sealed class MemoryAppCache : IAppCache
{
    private readonly IMemoryCache _cache;

    public MemoryAppCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan absoluteExpirationRelativeToNow,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue is not null)
            return cachedValue;

        var value = await factory(cancellationToken);

        _cache.Set(key, value, absoluteExpirationRelativeToNow);

        return value;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}

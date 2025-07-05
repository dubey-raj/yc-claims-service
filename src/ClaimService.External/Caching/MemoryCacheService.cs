using Microsoft.Extensions.Caching.Memory;

namespace ClaimService.External.Caching
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(60);

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        // Add item to cache
        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
            };
            _cache.Set(key, value, cacheOptions);
        }

        // Retrieve item from cache
        public bool TryGet<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value!);
        }

        // Remove item from cache
        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }

}

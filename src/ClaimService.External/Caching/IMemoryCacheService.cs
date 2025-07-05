namespace ClaimService.External.Caching
{
    public interface IMemoryCacheService
    {
        /// <summary>
        /// Set the data into in-memory cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key to store the value</param>
        /// <param name="value">value</param>
        /// <param name="expiration">TTL for the cache, default is 60 minutes</param>
        void Set<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Get the data from in-memory cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet<T>(string key, out T value);

        /// <summary>
        /// Remove the data from cache with specified key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}

using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace OlimpoCache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public Task<string> GetCacheValueAsync(string key)
        {
            return Task.FromResult(this._cache.Get<string>(key));
        }

        public Task SetCacheValue(string key, string value)
        {
            this._cache.Set(key, value);
            return Task.CompletedTask;
        }
    }
}

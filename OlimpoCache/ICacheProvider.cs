using System.Threading.Tasks;

namespace OlimpoCache
{
    public interface ICacheProvider
    {
         Task<string> GetCacheValueAsync(string key);

         Task SetCacheValue(string key, string value);
    }
}
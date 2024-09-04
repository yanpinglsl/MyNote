using CacheManager.Core;
using Ocelot.Cache;
using System.Collections.Generic;

namespace YY.Ocelot.HttpApi
{
    public class CustomCacheModel
    {

        public string Region { get; set; }
        public TimeSpan Ttl { get; set; }

        public CachedResponse Response { get; set; }
    }
    public class CustomCache : IOcelotCache<CachedResponse>
    {
        /// <summary>
        /// 存储缓存的地方，可根据需求自定义
        /// </summary>

        private static Dictionary<string, CustomCacheModel> customCacheDic = new Dictionary<string, CustomCacheModel>();
        public void Add(string key, CachedResponse value, TimeSpan ttl, string region)
        {            
            customCacheDic[key] = new CustomCacheModel() { Region = region, Ttl = ttl, Response = value };
            Console.WriteLine("自定义缓存+Add");
        }

        public void AddAndDelete(string key, CachedResponse value, TimeSpan ttl, string region)
        {
            customCacheDic[key] = new CustomCacheModel() { Region = region, Ttl = ttl, Response = value };
            Console.WriteLine("自定义缓存+AddAndDelete");
        }

        public void ClearRegion(string region)
        {
            foreach(var cache in customCacheDic)
            {
                if (cache.Value.Region == region)
                    customCacheDic.Remove(cache.Key);
            }
            Console.WriteLine("自定义缓存+ClearRegion");
        }

        public CachedResponse Get(string key, string region)
        {
            Console.WriteLine("自定义缓存+Get");
            if (customCacheDic.ContainsKey(key) && customCacheDic[key].Region == region)
                return customCacheDic[key].Response;
            else
                return null;
        }
    }
}

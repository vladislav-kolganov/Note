using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Note.Domain.Extensions
{
    public static class DestributedCacheExtensions
    {
        public static T GetObject<T>(this IDistributedCache cache, string key)
        { 
            var data = cache.Get(key);
            return data?.Length > 0 ? JsonSerializer.Deserialize<T>(data) : default(T);
        }
        public static void SetObject<T>(this IDistributedCache cache, string key, T obj, DistributedCacheEntryOptions? options = null)
        { 
            var data = JsonSerializer.SerializeToUtf8Bytes(obj);
            if (data?.Length > 0)
            cache.Set(key, data, options ?? new DistributedCacheEntryOptions());
        }      public static void RefreshObject<T>(this IDistributedCache cache, string key, DistributedCacheEntryOptions? options = null)
        {
            var isObj = cache.GetObject<T>(key);
            if (isObj == null) 
            {
             return;
            }
            cache.Refresh(key);
        }

    }
}

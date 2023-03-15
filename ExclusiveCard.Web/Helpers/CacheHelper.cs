using Microsoft.Extensions.Caching.Memory;
using System;

namespace ExclusiveCard.WebAdmin.Helpers
{
    public class CacheHelper
    {
        public static T Get<T>(IMemoryCache cache, string name1, string name2 = "", string name3 = "")
        {
            T obj = cache.Get<T>($"{name1.Replace(" ", "")}{name2.Replace(" ", "")}{ name3.Replace(" ", "")}");
            return obj;
        }

        public static void Set<T>(IMemoryCache cache, T val, DateTime timeout, string name1, string name2 = "", string name3 = "")
        {
            cache.Set($"{name1.Replace(" ", "")}{name2.Replace(" ", "")}{ name3.Replace(" ", "")}", val, timeout);
        }

        public static void Set<T>(IMemoryCache cache, T val, string name1, string name2 = "", string name3 = "")
        {
            cache.Set($"{name1.Replace(" ", "")}{name2.Replace(" ", "")}{ name3.Replace(" ", "")}", val, DateTime.UtcNow.AddMinutes(10));
        }
    }
}

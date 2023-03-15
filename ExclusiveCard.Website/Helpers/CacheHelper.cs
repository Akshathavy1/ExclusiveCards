using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ExclusiveCard.Website.Helpers
{
    public class CacheHelper
    {
        public static IConfiguration AppSettings { get; set; }

        public static T Get<T>(IMemoryCache cache, string name1, string name2 = "", string name3 = "")
        {
            T obj = default(T);
            if (!string.IsNullOrEmpty(name1))
            {
                obj = cache.Get<T>($"{name1?.Replace(" ", "")}");
            }
            else if(!string.IsNullOrEmpty(name1) && !string.IsNullOrEmpty(name2))
            {
                obj = cache.Get<T>($"{name1?.Replace(" ", "")}{name2?.Replace(" ", "")}");
            }
            else if (!string.IsNullOrEmpty(name1) && !string.IsNullOrEmpty(name2) && !string.IsNullOrEmpty(name3))
            {
                obj = cache.Get<T>($"{name1?.Replace(" ", "")}{name2?.Replace(" ", "")}{name3?.Replace(" ", "")}");
            }

            return obj;
        }

        public static void Set<T>(IMemoryCache cache, T val, DateTime timeout, string name1, string name2 = "", string name3 = "")
        {
            cache.Set($"{name1?.Replace(" ", "")}{name2?.Replace(" ", "")}{ name3?.Replace(" ", "")}", val, timeout);
        }

        public static void Set<T>(IMemoryCache cache, T val, string name1, string name2 = "", string name3 = "")
        {
            AppSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            int timeout = Convert.ToInt32(AppSettings["CacheTimeoutMinutes"]);
            cache.Set($"{name1?.Replace(" ", "")}{name2?.Replace(" ", "")}{ name3?.Replace(" ", "")}", val, DateTime.UtcNow.AddMinutes(timeout));
        }
    }
}

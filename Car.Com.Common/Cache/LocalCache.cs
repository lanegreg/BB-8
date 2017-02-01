using System;
using System.Runtime.Caching;

namespace Car.Com.Common.Cache
{
  public class LocalCache : ILocalCache
  {
    private static readonly MemoryCache Cache = MemoryCache.Default;
    private const double DefaultSecondsToLive = 30;

    public T Get<T>(string key) where T : class
    {
      return Cache.Get(key) as T;
    }

    public T Get<T>(string key, Func<T> whenNotFoundInCache) where T : class
    {
      return Cache.Get(key) as T ?? whenNotFoundInCache.Invoke();
    }


    public void Put(string key, object value)
    {
      Put(key, value, DefaultSecondsToLive);
    }

    public void Put(string key, object value, double secondsToLive)
    {
      Cache.Add(key, value, DateTimeOffset.Now.AddSeconds(secondsToLive));
    }
  }
}

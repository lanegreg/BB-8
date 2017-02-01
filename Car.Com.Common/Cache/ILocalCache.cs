using System;

namespace Car.Com.Common.Cache
{
  public interface ILocalCache
  {
    T Get<T>(string key) where T : class;
    T Get<T>(string key, Func<T> whenNotFoundInCache) where T : class;

    void Put(string key, object value);
    void Put(string key, object value, double secondsToLive);
  }
}

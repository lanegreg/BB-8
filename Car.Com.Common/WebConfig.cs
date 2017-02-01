using System;
using System.Configuration;

namespace Car.Com.Common
{
  public static class WebConfig
  {
    public static T Get<T>(string keyName)
    {
      if (typeof(T) == typeof(string))
        return (T)(object)(ConfigurationManager.AppSettings[keyName] ?? String.Empty);

      if (typeof(T) == typeof(int))
        return (T)(object)Int32.Parse(ConfigurationManager.AppSettings[keyName] ?? "0");

      if (typeof(T) == typeof(long))
        return (T)(object)Int64.Parse(ConfigurationManager.AppSettings[keyName] ?? "0");

      if (typeof(T) == typeof(double))
        return (T)(object)Double.Parse(ConfigurationManager.AppSettings[keyName] ?? "0");

      if (typeof(T) == typeof(bool))
        return (T)(object)Boolean.Parse(ConfigurationManager.AppSettings[keyName] ?? "false");

      return (T)(object)null;
    }
  }
}

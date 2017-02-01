using System;
using System.Collections.Generic;

namespace Car.Com.Common.Cacheability
{
  public static class CacheConfig
  {
    private const int SecsPerMin = 60;
    private const int SecsPerHour = 3600;
    private const int SecsPerDay = 86400;


    public static void ParseTimeToLives(IDictionary<string, TimeToLive> cacheContolTtls, string configString)
    {
      cacheContolTtls.Clear();
      var levels = configString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

      #region - Add non-config-string levels

      cacheContolTtls.Add("none", new TimeToLive
      {
        ShortUnitName = "s",
        LongUnitName = "secs",
        Value = 0
      });
      cacheContolTtls.Add("zero_revalidate", new TimeToLive
      {
        ShortUnitName = "s",
        LongUnitName = "secs",
        Value = 0
      });

      #endregion

      #region - Parse Levels

      foreach (var level in levels)
      {
        var parts = level.Split(new[] { '=' });
        var levelName = parts[0];
        var ttlParts = parts[1].Split(new[] { '-' });

        switch (ttlParts[1])
        {
          case "secs":
            {
              cacheContolTtls.Add(levelName, new TimeToLive
              {
                ShortUnitName = "s",
                LongUnitName = "secs",
                ValueInSecs = Int32.Parse(ttlParts[0]),
                Value = Int32.Parse(ttlParts[0])
              });
              break;
            }

          case "mins":
            {
              cacheContolTtls.Add(levelName, new TimeToLive
              {
                ShortUnitName = "m",
                LongUnitName = "mins",
                ValueInSecs = Int32.Parse(ttlParts[0]) * SecsPerMin,
                Value = Int32.Parse(ttlParts[0])
              });
              break;
            }

          case "hrs":
            {
              cacheContolTtls.Add(levelName, new TimeToLive
              {
                ShortUnitName = "h",
                LongUnitName = "hrs",
                ValueInSecs = Int32.Parse(ttlParts[0]) * SecsPerHour,
                Value = Int32.Parse(ttlParts[0])
              });
              break;
            }

          case "days":
            {
              cacheContolTtls.Add(levelName, new TimeToLive
              {
                ShortUnitName = "d",
                LongUnitName = "days",
                ValueInSecs = Int32.Parse(ttlParts[0]) * SecsPerDay,
                Value = Int32.Parse(ttlParts[0])
              });
              break;
            }
        }
      }

      #endregion
    }

    public static TimeToLive ResolveTimeToLiveLevel(IDictionary<string, TimeToLive> cacheControlTtls, TimeToLiveLevel timeToLiveLevel)
    {
      switch (timeToLiveLevel)
      {
        case TimeToLiveLevel.None:
          return cacheControlTtls["none"];

        case TimeToLiveLevel.ZeroRevalidate:
          return cacheControlTtls["zero_revalidate"];

        case TimeToLiveLevel.Shortest:
          return cacheControlTtls["shortest"];

        case TimeToLiveLevel.UltraShort:
          return cacheControlTtls["ultra_short"];

        case TimeToLiveLevel.ExtraShort:
          return cacheControlTtls["extra_short"];

        case TimeToLiveLevel.Short:
          return cacheControlTtls["short"];

        case TimeToLiveLevel.Medium:
          return cacheControlTtls["medium"];

        case TimeToLiveLevel.Long:
          return cacheControlTtls["long"];

        case TimeToLiveLevel.Longer:
          return cacheControlTtls["longer"];

        case TimeToLiveLevel.ExtraLong:
          return cacheControlTtls["extra_long"];

        case TimeToLiveLevel.SuperLong:
          return cacheControlTtls["super_long"];

        case TimeToLiveLevel.UltraLong:
          return cacheControlTtls["ultra_long"];

        case TimeToLiveLevel.Longest:
          return cacheControlTtls["longest"];

        default:
          return null;
      }
    }

    public static void Validate(IDictionary<string, TimeToLive> downstreamTtls, IDictionary<string, TimeToLive> edgeTtls)
    {
      foreach (TimeToLiveLevel level in Enum.GetValues(typeof(TimeToLiveLevel)))
      {
        // Filter out non-config-string enums
        if (level == TimeToLiveLevel.None || level == TimeToLiveLevel.ZeroRevalidate)
          continue;

        var downstreamTtl = ResolveTimeToLiveLevel(downstreamTtls, level);
        var edgeTtl = ResolveTimeToLiveLevel(edgeTtls, level);

        if (downstreamTtl.ValueInSecs > edgeTtl.ValueInSecs)
          throw new Exception(String.Format("Cacheability:Downstream value is greater than Cacheability:Edge value for [CacheHeaders.{0}]", level));
      }
    }
  }
}
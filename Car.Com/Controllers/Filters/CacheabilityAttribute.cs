using Car.Com.Common;
using Car.Com.Common.Cacheability;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;


namespace Car.Com.Controllers.Filters
{
  // Ref: http://stackoverflow.com/questions/5826811/asp-net-mvc-3-0-configure-outputcache-attribute-to-produce-cache-control-publ
  public class CacheabilityAttribute : ActionFilterAttribute
  {
    #region Declarations

    private static readonly IDictionary<string, TimeToLive> TimeToLivesForDownstream = new Dictionary<string, TimeToLive>();
    private static readonly IDictionary<string, TimeToLive> TimeToLivesForEdge = new Dictionary<string, TimeToLive>();

    #endregion


    static CacheabilityAttribute()
    {
      var configStringForDownstream = WebConfig.Get<string>("Cacheability:Timeouts:Downstream");
      var configStringForEdge = WebConfig.Get<string>("Cacheability:Timeouts:Edge");

      CacheConfig.ParseTimeToLives(TimeToLivesForDownstream, configStringForDownstream);
      CacheConfig.ParseTimeToLives(TimeToLivesForEdge, configStringForEdge);
      CacheConfig.Validate(TimeToLivesForDownstream, TimeToLivesForEdge);
    }


    public TimeToLiveLevel TimeToLiveLevel { get; set; }

    public CacheabilityAttribute()
    {
      TimeToLiveLevel = TimeToLiveLevel.Shortest;
    }


    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      if (TimeToLiveLevel == TimeToLiveLevel.None)
      {
        var response = filterContext.HttpContext.Response;
        var cache = response.Cache;
        var downstreamTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForDownstream, TimeToLiveLevel);
        var edgeTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForEdge, TimeToLiveLevel);


        #region - Downstream Cacheability

        cache.SetCacheability(HttpCacheability.Private);
        cache.SetNoTransforms();

        cache.SetMaxAge(TimeSpan.FromSeconds(downstreamTtl.ValueInSecs));

        #endregion


        #region - Edge Cacheability

        response.AddHeader("Edge-Control",
          String.Format("downstream-ttl={0}{1}, cache-maxage={2}{3}",
            downstreamTtl.Value, downstreamTtl.ShortUnitName, edgeTtl.Value, edgeTtl.ShortUnitName));

        #endregion
      }
      else if (TimeToLiveLevel == TimeToLiveLevel.ZeroRevalidate)
      {
        var response = filterContext.HttpContext.Response;
        var cache = response.Cache;
        var downstreamTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForDownstream, TimeToLiveLevel);
        var edgeTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForEdge, TimeToLiveLevel);
        var now = DateTime.Now;


        #region - Downstream Cacheability

        cache.SetCacheability(HttpCacheability.Public);
        cache.SetNoTransforms();
        cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

        cache.SetMaxAge(TimeSpan.FromSeconds(downstreamTtl.ValueInSecs));
        cache.AppendCacheExtension("s-maxage=" + TimeSpan.FromSeconds(downstreamTtl.ValueInSecs).TotalSeconds);
        cache.SetExpires(now.AddSeconds(downstreamTtl.ValueInSecs));
        cache.SetLastModified(now);

        #endregion


        #region - Edge Cacheability

        response.AddHeader("Edge-Control",
          String.Format("downstream-ttl={0}{1}, cache-maxage={2}{3}",
            downstreamTtl.Value, downstreamTtl.ShortUnitName, edgeTtl.Value, edgeTtl.ShortUnitName));

        #endregion
      }
      else // this handles all the level-config-string related settings
      {
        var response = filterContext.HttpContext.Response;
        var cache = response.Cache;
        var downstreamTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForDownstream, TimeToLiveLevel);
        var edgeTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForEdge, TimeToLiveLevel);
        var now = DateTime.Now;


        #region - Downstream Cacheability

        cache.SetCacheability(HttpCacheability.Public);
        cache.SetNoTransforms();

        cache.SetMaxAge(TimeSpan.FromSeconds(downstreamTtl.ValueInSecs));
        cache.AppendCacheExtension("s-maxage=" + TimeSpan.FromSeconds(downstreamTtl.ValueInSecs).TotalSeconds);
        cache.SetExpires(now.AddSeconds(downstreamTtl.ValueInSecs));
        cache.SetLastModified(now);

        #endregion


        #region - Edge Cacheability

        response.AddHeader("Edge-Control",
          String.Format("downstream-ttl={0}{1}, cache-maxage={2}{3}",
            downstreamTtl.Value, downstreamTtl.ShortUnitName, edgeTtl.Value, edgeTtl.ShortUnitName));

        #endregion
      }
    }

  }
}
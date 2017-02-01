using Car.Com.Common;
using Car.Com.Common.Cacheability;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web.Http.Filters;


namespace Car.Com.Controllers.Filters
{
  // Ref: http://stackoverflow.com/questions/5826811/asp-net-mvc-3-0-configure-outputcache-attribute-to-produce-cache-control-publ
  public class ApiCacheabilityAttribute : ActionFilterAttribute
  {
    #region Declarations

    private static readonly IDictionary<string, TimeToLive> TimeToLivesForDownstream = new Dictionary<string, TimeToLive>();
    private static readonly IDictionary<string, TimeToLive> TimeToLivesForEdge = new Dictionary<string, TimeToLive>();

    #endregion


    static ApiCacheabilityAttribute()
    {
      var configStringForDownstream = WebConfig.Get<string>("Cacheability:Timeouts:Downstream");
      var configStringForEdge = WebConfig.Get<string>("Cacheability:Timeouts:Edge");

      CacheConfig.ParseTimeToLives(TimeToLivesForDownstream, configStringForDownstream);
      CacheConfig.ParseTimeToLives(TimeToLivesForEdge, configStringForEdge);
      CacheConfig.Validate(TimeToLivesForDownstream, TimeToLivesForEdge);
    }


    public TimeToLiveLevel TimeToLiveLevel { get; set; }

    public ApiCacheabilityAttribute()
    {
      TimeToLiveLevel = TimeToLiveLevel.Shortest;
    }


    public override void OnActionExecuted(HttpActionExecutedContext filterContext)
    {
      if (TimeToLiveLevel == TimeToLiveLevel.None)
      {
        var response = filterContext.Response;
        var downstreamTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForDownstream, TimeToLiveLevel);
        var edgeTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForEdge, TimeToLiveLevel);


        #region - Downstream Cacheability

        response.Headers.CacheControl = new CacheControlHeaderValue
        {
          Public = false,
          NoTransform = true,
          MaxAge = TimeSpan.FromSeconds(downstreamTtl.ValueInSecs),
          SharedMaxAge = TimeSpan.FromSeconds(downstreamTtl.ValueInSecs)
        };

        response.Headers.Add("Vary", "Accept-Encoding");

        #endregion


        #region - Edge Cacheability

        response.Headers.Add("Edge-Control",
          String.Format("downstream-ttl={0}{1}, cache-maxage={2}{3}",
            downstreamTtl.Value, downstreamTtl.ShortUnitName, edgeTtl.Value, edgeTtl.ShortUnitName));

        #endregion
      }
      else if (TimeToLiveLevel == TimeToLiveLevel.ZeroRevalidate)
      {
        var response = filterContext.Response;
        var downstreamTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForDownstream, TimeToLiveLevel);
        var edgeTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForEdge, TimeToLiveLevel);


        #region - Downstream Cacheability

        response.Headers.CacheControl = new CacheControlHeaderValue
        {
          Public = true,
          MustRevalidate = true,
          ProxyRevalidate = true,
          NoTransform = true,
          MaxAge = TimeSpan.FromSeconds(downstreamTtl.ValueInSecs),
          SharedMaxAge = TimeSpan.FromSeconds(downstreamTtl.ValueInSecs)
        };

        response.Headers.Add("Vary", "Accept-Encoding");

        #endregion


        #region - Edge Cacheability

        response.Headers.Add("Edge-Control",
          String.Format("downstream-ttl={0}{1}, cache-maxage={2}{3}",
            downstreamTtl.Value, downstreamTtl.ShortUnitName, edgeTtl.Value, edgeTtl.ShortUnitName));

        #endregion
      }
      else
      {
        var response = filterContext.Response;
        var downstreamTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForDownstream, TimeToLiveLevel);
        var edgeTtl = CacheConfig.ResolveTimeToLiveLevel(TimeToLivesForEdge, TimeToLiveLevel);
        var now = DateTime.Now;


        #region - Downstream Cacheability

        response.Content.Headers.LastModified = new DateTimeOffset(now);
        response.Content.Headers.Expires = new DateTimeOffset(now.AddSeconds(downstreamTtl.ValueInSecs));
        response.Headers.CacheControl = new CacheControlHeaderValue
        {
          Public = true,
          NoTransform = true,
          MaxAge = TimeSpan.FromSeconds(downstreamTtl.ValueInSecs),
          SharedMaxAge = TimeSpan.FromSeconds(downstreamTtl.ValueInSecs)
        };

        response.Headers.Add("Vary", "Accept-Encoding");

        #endregion


        #region - Edge Cacheability

        response.Headers.Add("Edge-Control",
          String.Format("downstream-ttl={0}{1}, cache-maxage={2}{3}",
            downstreamTtl.Value, downstreamTtl.ShortUnitName, edgeTtl.Value, edgeTtl.ShortUnitName));

        #endregion
      }
    }

  }
}
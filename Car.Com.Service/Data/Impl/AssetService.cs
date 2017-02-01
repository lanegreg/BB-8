using Car.Com.Common;
using Car.Com.Domain.Services;
using Car.Com.Service.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Web;

namespace Car.Com.Service.Data.Impl
{
  public sealed class AssetService : IAssetService, ICacheable
  {
    #region Declarations

    private const int RefreshIntervalInMins = 10;
    private static readonly object Mutex = new object();
    private static readonly MemoryCache Cache = MemoryCache.Default;
    private static readonly HttpServerUtility Server = HttpContext.Current.Server;
    private static string _inlineHeadScript;
    private static IDictionary<string, string> _inlineHeadStylesDictionary;
    private static readonly bool IsLocalDev = WebConfig.Get<bool>("Environment:IsLocalDev");

    #endregion

    #region Interface Impls

    public void Warm()
    {
      CacheAllGlobalSiteAssets();
      Cache.Set("Asset_Service", String.Empty, GetCachePolicy());
    }

    #endregion


    public HtmlString GetInlineHeadStyles(string assetsPrefix)
    {
      // Input params validation.
      if (assetsPrefix.IsNullOrEmpty())
        throw new ArgumentException("assetsPrefix");

      var key = Server.MapPath(String.Format("~/assets/above_the_fold/{0}.head.min.css", assetsPrefix));

      return new HtmlString(_inlineHeadStylesDictionary[key]);
    }

    public HtmlString GetInlineHeadScript()
    {
      return new HtmlString(_inlineHeadScript);
    }


    #region Private Static Methods

    private static void CacheUpdateHandler(CacheEntryUpdateArguments args)
    {
      CacheAllGlobalSiteAssets();
      var cacheItem = Cache.GetCacheItem(args.Key);

      if (cacheItem != null)
        cacheItem.Value = String.Empty;
      else
        cacheItem = new CacheItem("cacheItem", String.Empty);

      args.UpdatedCacheItem = cacheItem;
      args.UpdatedCacheItemPolicy = GetCachePolicy();
    }

    private static CacheItemPolicy GetCachePolicy()
    {
      return new CacheItemPolicy
      {
        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(IsLocalDev ? 5 : RefreshIntervalInMins * 60),
        UpdateCallback = CacheUpdateHandler
      };
    }

    private static void CacheAllGlobalSiteAssets()
    {
      var script =
        File.ReadAllText(Server.MapPath("~/assets/head.inline.min.js")) +
        File.ReadAllText(Server.MapPath("~/assets/modernizr-2.8.3.min.js"));

      lock (Mutex)
      {
        _inlineHeadScript = script;
      }


      var stylesDict = new Dictionary<string, string>();
      var files = Directory.GetFiles(Server.MapPath("~/assets/above_the_fold"), "*.head.min.css");
      foreach (var file in files)
      {
        var styles = File.ReadAllText(file);
        stylesDict.Add(file, styles);
      }

      lock (Mutex)
      {
        _inlineHeadStylesDictionary = stylesDict;
      }
    }

    #endregion
  }
}

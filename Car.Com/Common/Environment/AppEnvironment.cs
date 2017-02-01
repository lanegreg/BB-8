using Car.Com.Service.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Car.Com.Common.Environment
{
  public sealed class AppEnvironment : ICacheable
  {
    #region Declarations

    private const int RefreshIntervalInMins = 10;
    private static readonly object Mutex = new object();
    private static readonly MemoryCache Cache = MemoryCache.Default;
    private static ICollection<CacheBustedAsset> _cacheBustedAssets;
    private static readonly HttpServerUtility Server = HttpContext.Current.Server;

    #endregion


    public void Warm()
    {
      ProcessStaticCacheBusterManifest();
      Cache.Set("BusterManifestLoad", String.Empty, GetCachePolicy());
    }


    static AppEnvironment()
    {
      IsProduction = WebConfig.Get<bool>("Environment:IsProduction");
      IsLocalDev = WebConfig.Get<bool>("Environment:IsLocalDev");
      IsQaStage = !IsProduction && !IsLocalDev;
      ServerName = HttpContext.Current.Server.MachineName;

      try
      {
        BuildVersion =
          IsLocalDev
            ? "(local dev)"
            : File.ReadAllText(Server.MapPath("~/build.txt"));
      }
      catch (Exception ex)
      {
        const string missingBuildVersionBlurb = "FATAL: A NON-LocalDev environment has been enabled, but the ./build.txt file could NOT be found in the application root.";
        Log.Fatal(missingBuildVersionBlurb);
        throw new Exception(missingBuildVersionBlurb, ex);
      }

      WwwAssetsUrlPrefix = "/assets/";

      AssetsUrlPrefix =
        IsLocalDev
          ? "/app_assets/dist"
          : WebConfig.Get<string>("Environment:StaticUrlPrefix");

      StaticDistributionScriptUrlPrefix = AssetsUrlPrefix + "/js/";
      StaticDistributionStyleUrlPrefix = AssetsUrlPrefix + "/css/";
      StaticLibrariesUrlPrefix = String.Format("{0}/libs/", AssetsUrlPrefix);
    }


    #region Public Properties

    public static string WwwAssetsUrlPrefix { get; private set; }
    public static string AssetsUrlPrefix { get; private set; }
    public static bool IsProduction { get; private set; }
    public static bool IsLocalDev { get; private set; }
    public static bool IsQaStage { get; private set; }
    public static string BuildVersion { get; private set; }
    public static string ServerName { get; private set; }

    public static string StaticDistributionScriptUrlPrefix { get; private set; }
    public static string StaticDistributionStyleUrlPrefix { get; private set; }
    public static string StaticLibrariesUrlPrefix { get; private set; }
    public static string GlobalSiteJavaScriptAssetName { get; private set; }

    public static ICollection<CacheBustedAsset> GetCacheBustedAssets(string fileName)
    {
      var assets = _cacheBustedAssets.Where(a => a.PageName.StartsWith(fileName)).ToList();
      assets.AddRange(_cacheBustedAssets.Where(a => a.PageName.StartsWith("common.")));
      return assets;
    }

    public static string GetCacheBustedAssetNameByFileName(string fileName)
    {
      if (IsLocalDev)
        return fileName;

      var parts = fileName.Split(new[] {".min."}, StringSplitOptions.RemoveEmptyEntries);
      var filename = parts[0];
      var filetype = parts[1];
      var cacheBustedAsset = _cacheBustedAssets.FirstOrDefault(a => a.PageName == filename && a.FileType == filetype);

      return cacheBustedAsset != null ? cacheBustedAsset.CacheBustedAssetName : String.Empty;
    }

    #endregion


    #region Private Static Methods

    private static void CacheUpdateHandler(CacheEntryUpdateArguments args)
    {
      ProcessStaticCacheBusterManifest();
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
        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(IsLocalDev ? 5 : RefreshIntervalInMins*60),
        UpdateCallback = CacheUpdateHandler
      };
    }

    private static void ProcessStaticCacheBusterManifest()
    {
      const string globalSiteKeyName = "global_sitejs";

      try
      {
        var jsonString = File.ReadAllText(Server.MapPath("~/assets/buster_manifest.json"));

        jsonString = jsonString
          .Replace("../app_assets/dist/js/", String.Empty)
          .Replace("../app_assets/dist/css/", String.Empty)
          .Replace("../app_assets/dist/libs/", String.Empty)
          .Replace(".min.", String.Empty)
          .Replace(".", "_");

        dynamic busterManifest = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

        var cacheBustedAssets =
          (from asset in ((Dictionary<string, object>) busterManifest).Where(a => a.Key != globalSiteKeyName)
            let isScript = asset.Key.EndsWith("js")
            let fileNamePrefix = isScript
              ? asset.Key.Replace("js", String.Empty)
              : asset.Key.Replace("css", String.Empty)
            let deviceType = fileNamePrefix.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries)[2]
            select new CacheBustedAsset
            {
              PageName = fileNamePrefix.Replace("_", ".").Replace(deviceType, String.Empty).TrimEnd(new[] {'.'}),
              FileType = isScript ? "js" : "css",
              DeviceType = deviceType,
              Hash = asset.Value as string
            }).ToList();

        lock (Mutex)
        {
          _cacheBustedAssets = cacheBustedAssets;
        }

        lock (Mutex)
        {
          var globalSiteJavaScriptAsset = ((Dictionary<string, object>)busterManifest).First(a => a.Key == globalSiteKeyName);
          var assetNamePrefix = globalSiteJavaScriptAsset.Key.Replace("_", ".").TrimEnd(new[] {'j', 's'});
          GlobalSiteJavaScriptAssetName = IsLocalDev
            ? String.Format("{0}.min.js", assetNamePrefix)
            : String.Format("{0}-{1}.min.js", assetNamePrefix, globalSiteJavaScriptAsset.Value);
        }
      }
      catch (Exception ex)
      {
        const string brokenBusterManifestFileBlurb =
          "FATAL: The file ./assets/buster_manifest.json could NOT be found or failed to load properly.";
        Log.Fatal(brokenBusterManifestFileBlurb);
        throw new Exception(brokenBusterManifestFileBlurb, ex);
      }
    }

    #endregion
  }
}
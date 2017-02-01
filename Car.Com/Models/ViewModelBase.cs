using Car.Com.Common;
using Car.Com.Common.DeviceDetection;
using Car.Com.Common.Environment;
using Car.Com.Common.SiteMeta;
using Car.Com.Domain.Models.SiteMeta;
using Newtonsoft.Json;
using System;
using System.Web;

namespace Car.Com.Models
{
  public class ViewModelBase
  {
    private readonly HttpContextBase _currentHttpContext;


    public ViewModelBase(string assetsPrefix)
    {
      _currentHttpContext = new HttpContextWrapper(HttpContext.Current);

      AssetsPrefix = assetsPrefix;
      InlineHeadScript = new HtmlString("");
      InlineHeadStyles = new HtmlString("");
      BrowserCaps = _currentHttpContext.GetBrowserCaps();
      Environment = new Environment();
      PageMeta = new PageMeta();
      AdvertMeta = new AdvertMeta();
      TrackMeta = new TrackMeta();
      OfferMeta = new OfferMeta();
      OpenGraphMeta = new OpenGraphMeta();
    }

    public ViewModelBase(string assetsPrefix, IMetadata metadata)
      : this(assetsPrefix)
    {
      PageMeta = metadata.PageMeta;
      OmniPage = assetsPrefix;
    }

    public string AssetsPrefix { get; set; }
    public HtmlString InlineHeadStyles { get; set; }
    public HtmlString InlineHeadScript { get; set; }

    public PageMeta PageMeta { get; set; }
    public TrackMeta TrackMeta { get; set; }
    public AdvertMeta AdvertMeta { get; set; }
    public OfferMeta OfferMeta { get; set; }
    public OpenGraphMeta OpenGraphMeta { get; set; }
    public string OmniPage { get; set; }
    
    public IBrowserCaps BrowserCaps { get; private set; }
    public Environment Environment { get; private set; }
    public HttpContextBase CurrentHttpContext { get { return _currentHttpContext; } }

    public HtmlString GetStaticAssetsJsonString()
    {
      return new HtmlString(JsonConvert.SerializeObject(AppEnvironment.GetCacheBustedAssets(AssetsPrefix)));
    }

    public string GetNoScriptStylesUrl()
    {
      return AppEnvironment.StaticDistributionStyleUrlPrefix +
             AppEnvironment.GetCacheBustedAssetNameByFileName(AssetsPrefix + ".desk.min.css");
    }

    private string _jsonStrings = String.Empty;
    public void RegisterPageJson(string json, string variableName)
    {
      if ((json.StartsWith("{") || json.StartsWith("[")) && (json.EndsWith("}") || json.EndsWith("]")))
      {
        if (variableName.StartsWith("ABT.pageJson.") && !_jsonStrings.Contains(variableName))
          _jsonStrings += variableName + "=" + json + ";\r\n";
      }
    }
    
    public string EmmitPageJson()
    {
      return _jsonStrings.IsNotNullOrEmpty()
        ? "ABT.pageJson=ABT.pageJson||{};\r\n" + _jsonStrings
        : String.Empty;
    }
  }


  public class Environment
  {
    public bool IsProduction { get { return AppEnvironment.IsProduction; } }
    public bool IsLocalDev { get { return AppEnvironment.IsLocalDev; } }
    public bool IsQaStage { get { return AppEnvironment.IsQaStage; } }
  }
}
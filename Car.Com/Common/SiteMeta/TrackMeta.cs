using Car.Com.Domain.Models.SiteMeta;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Newtonsoft.Json;
using System.Web;

namespace Car.Com.Common.SiteMeta
{
  public class TrackMeta
  {

    #region ctors

    public TrackMeta()
    {
      DefaultAffiliateId = AffiliateService.CarDotComId;
    }
    
    public TrackMeta(IMetadata metadata)
      : this()
    {
      SiteSection = metadata.PageMeta.SiteSection;
      ContentSection = metadata.PageMeta.ContentSection;
      SubSection = metadata.PageMeta.SubSection;
      PageName = metadata.PageMeta.PageName;
    }

    #endregion


    #region Public Properties

    [JsonProperty("delayPageviewTracking")]
    public bool DelayPageviewTracking { get; set; }

    [JsonProperty("defaultId")]
    public int DefaultAffiliateId { get; private set; }

    [JsonProperty("siteSection", NullValueHandling = NullValueHandling.Ignore)]
    public string SiteSection { get; private set; }

    [JsonProperty("contentSection", NullValueHandling = NullValueHandling.Ignore)]
    public string ContentSection { get; private set; }

    [JsonProperty("subSection", NullValueHandling = NullValueHandling.Ignore)]
    public string SubSection { get; private set; }

    [JsonProperty("pageName", NullValueHandling = NullValueHandling.Ignore)]
    public string PageName { get; private set; }

    [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
    public string Make { get; set; }

    [JsonProperty("superModel", NullValueHandling = NullValueHandling.Ignore)]
    public string SuperModel { get; set; }

    [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
    public string Year { get; set; }

    [JsonProperty("trim", NullValueHandling = NullValueHandling.Ignore)]
    public string Trim { get; set; }

    [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
    public string Category { get; set; }

    [JsonProperty("articleId", NullValueHandling = NullValueHandling.Ignore)]
    public string ArticleId { get; set; }

    #endregion


    public HtmlString SerializePageCtxToJson()
    {
      return new HtmlString(JsonConvert.SerializeObject(this));
    }


    #region Services

    private static IAffiliateService AffiliateService
    {
      get { return ServiceLocator.Get<IAffiliateService>(); }
    }

    #endregion
  }
}
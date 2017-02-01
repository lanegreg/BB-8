using Car.Com.Common.Merger;
using Newtonsoft.Json;
using System;
using System.Web;

namespace Car.Com.Common.SiteMeta
{
  public class AdvertMeta
  {
    public AdvertMeta()
    {
      PageCtx = new object();
    }

    public AdvertMeta(object pageCtx)
    {
      PageCtx = pageCtx;
    }

    [JsonIgnore]
    public object PageCtx { get; set; }

    public HtmlString SerializePageCtxToJson()
    {
      var pageCtx = JsonConvert.SerializeObject(ObjectMerger.MergeObjects(this, PageCtx));
      
      return new HtmlString(pageCtx.Replace("\"PageCtx\":{},", String.Empty));
    }

    // ReSharper disable once InconsistentNaming
    public bool delayAdsLoading { get; set; }
  }
}

using Newtonsoft.Json;
using System.Web;

namespace Car.Com.Common.SiteMeta
{
  public class OfferMeta
  {
    public OfferMeta()
    {
      PageCtx = new object();
    }
    public OfferMeta(object pageCtx)
    {
      PageCtx = pageCtx;
    }


    public object PageCtx { get; set; }

    public HtmlString SerializePageCtxToJson()
    {
      return new HtmlString(JsonConvert.SerializeObject(PageCtx));
    }
  }
}

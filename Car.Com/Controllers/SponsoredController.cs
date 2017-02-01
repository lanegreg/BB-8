using Car.Com.Common.Cacheability;
using Car.Com.Common.SiteMeta;
using Car.Com.Controllers.Filters;
using Car.Com.Models;
using Car.Com.Models.Site;
using System.Web.Mvc;
using Car.Com.Models.Sponsored;


namespace Car.Com.Controllers
{

  public class SponsoredController : BaseController
  {

#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("sponsored", Name = "nativo")]
    public ActionResult Nativo()
    {
      const string assetsPrefix = "sponsored.nativo";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new NativoViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("Nativo", viewModel);
    }

  }
}
using System.Web.Mvc;
using Car.Com.Models.CompareCars;
using Newtonsoft.Json;

namespace Car.Com.Controllers
{
  [RoutePrefix("tools/car-comparison")]
  public class CompareCarsController : BaseController
  {
    /** 
     * This Controller's sole purpose is to handle all urls that belong to inventory car pages, nothing more!
     **/

    [Route("", Name = "CompareCars_Index"), HttpGet]
    public ActionResult Index()
    {
      const string assetsPrefix = "comparecars.index";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new IndexViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix)
      };

      return View("Index", viewModel);
    }

    [Route("results", Name = "CompareCars_Results"), HttpGet]
    public ActionResult Results()
    {
      const string assetsPrefix = "comparecars.results";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new ResultsViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix)
      };

      var leadformJsonStr = JsonConvert.SerializeObject(new
      {quoteButtonSelected = false, year = "", make = "", supermodel = "", trim = "" });
      viewModel.RegisterPageJson(leadformJsonStr, "ABT.pageJson.getaquote");
      return View("Results", viewModel);
    }

  }
}
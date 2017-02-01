using System.Web;
using Car.Com.Domain.Services;
using Car.Com.Models.HttpError;
using Car.Com.Service;
using System.Web.Mvc;

namespace Car.Com.Controllers
{
  [RoutePrefix("error")]
  public class HttpErrorController : BaseController
  {
    /** 
     * This Controller's sole purpose is to handle the /error/ urls (i.e. /status-404/, /status-500/, etc.), nothing more! 
     **/

    //[Route("status-404", Name = "HttpError_Status404"), HttpGet]
    public ActionResult Status404()
    {
      const string assetsPrefix = "httperror.status404";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var makesTask = VehicleSpecService.GetAllActiveMakesAsync();
      var categoriesTask = VehicleSpecService.GetAllCategoriesAsync();

      makesTask.Wait();
      categoriesTask.Wait();
      var makes = makesTask.Result;
      var categories = categoriesTask.Result;

      var viewModel = new ErrorViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        Makes = makes,
        Categories = categories
      };

      return View("Status404", viewModel);
    }

    //[Route("status-500", Name = "HttpError_Status500"), HttpGet]
    public ActionResult Status500()
    {
      const string assetsPrefix = "httperror.status500";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var makesTask = VehicleSpecService.GetAllActiveMakesAsync();
      var categoriesTask = VehicleSpecService.GetAllCategoriesAsync();

      makesTask.Wait();
      categoriesTask.Wait();
      var makes = makesTask.Result;
      var categories = categoriesTask.Result;

      var viewModel = new ErrorViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        Makes = makes,
        Categories = categories
      };

      return View(@"~\Views\HttpError\Status500.cshtml", viewModel);
    }

    #region Services

    private static IVehicleSpecService VehicleSpecService
    {
      get { return ServiceLocator.Get<IVehicleSpecService>(); }
    }

    #endregion
  }
}
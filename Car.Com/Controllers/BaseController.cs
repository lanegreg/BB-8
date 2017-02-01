using Car.Com.Common;
using Car.Com.Domain.Services;
using Car.Com.Service;
using System.Web.Mvc;

namespace Car.Com.Controllers
{
  public class BaseController : Controller
  {
    protected override void OnActionExecuted(ActionExecutedContext ctx)
    {
      base.OnActionExecuted(ctx);
      HttpContext.SetActionCount();
    }

    protected static IAssetService AssetService
    {
      get { return ServiceLocator.Get<IAssetService>(); }
    }

    protected static IMetadataService MetadataService
    {
      get { return ServiceLocator.Get<IMetadataService>(); }
    }

    protected static IAffiliateService AffiliateService
    {
      get { return ServiceLocator.Get<IAffiliateService>(); }
    }
  }
}
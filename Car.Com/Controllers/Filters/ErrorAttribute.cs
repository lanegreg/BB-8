using Car.Com.Common;
using System.Web.Mvc;
using System.Web.Routing;

namespace Car.Com.Controllers.Filters
{
  public class ErrorAttribute : HandleErrorAttribute
  {
    public override void OnException(ExceptionContext filterContext)
    {
      // First, log any errors that come through here.
      Log.Error(filterContext.Exception);

      // Then, check if we should show pretty error pages, as opposed to the default ASP.NET style.
      if (!WebConfig.Get<bool>("Environment:IsLocalDev"))
      {
        filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        filterContext.HttpContext.Response.StatusCode = 500;
        filterContext.ExceptionHandled = true;
        filterContext.HttpContext.Response.Clear();

        using (var httpErrorController = new HttpErrorController())
        {
          httpErrorController.ControllerContext = new ControllerContext(filterContext.HttpContext, new RouteData(), httpErrorController);
          filterContext.Result = httpErrorController.Status500();
        }
      }

      base.OnException(filterContext);
    }
  }
}
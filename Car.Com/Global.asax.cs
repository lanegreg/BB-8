using Car.Com.Controllers;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Car.Com
{
  public class MvcApplication : HttpApplication
  {
    protected void Application_Start()
    {
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      CacheableComponents.WarmThemUp();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
    }

    protected void Application_Error(object sender, EventArgs e)
    {
      Exception exception = Server.GetLastError();

      var httpException = exception as HttpException;

      var routeData = new RouteData();
      routeData.Values.Add("controller", "HttpError");

      if (httpException != null)
      {
        switch (httpException.GetHttpCode())
        {
          case 404:
            // Page not found.
            routeData.Values.Add("action", "Status404");
            break;
          case 500:
            // Server error.
            routeData.Values.Add("action", "Status500");
            break;

          // Here you can handle Views to other error codes.
          default:
            routeData.Values.Add("action", "Status500");
            break;
        }
      }

      // Call target Controller and pass the routeData.
      IController errorController = new HttpErrorController();
      errorController.Execute(new RequestContext(
           new HttpContextWrapper(Context), routeData));

    }
  }
}

using System.Web.Mvc;
using System.Web.Routing;

namespace Car.Com.Common
{
  public static class UrlHelperExtensions
  {
    public static string ActionEx(this UrlHelper url, string actionName)
    {
      return url.Action(actionName).EnsureTrailingForwardSlash();
    }


    public static string ActionEx(this UrlHelper url, string actionName, string controllerName)
    {
      return url.Action(actionName, controllerName).EnsureTrailingForwardSlash();
    }


    public static string ActionEx
      (this UrlHelper url, string actionName, string controllerName, object routeValues)
    {
      return url.Action(actionName, controllerName, routeValues).EnsureTrailingForwardSlash();
    }


    public static string ActionEx
      (this UrlHelper url, string actionName, string controllerName, RouteValueDictionary routeValues)
    {
      return url.Action(actionName, controllerName, routeValues).EnsureTrailingForwardSlash();
    }


    public static string RouteUrlEx(this UrlHelper url, string routeName)
    {
      return url.RouteUrl(routeName).EnsureTrailingForwardSlash();
    }


    public static string RouteUrlEx(this UrlHelper url, string routeName, object routeValues)
    {
      return url.RouteUrl(routeName, routeValues).EnsureTrailingForwardSlash();
    }


    public static string RouteUrlEx(this UrlHelper url, string routeName, RouteValueDictionary routeValues)
    {
      return url.RouteUrl(routeName, routeValues).EnsureTrailingForwardSlash();
    }
  }
}
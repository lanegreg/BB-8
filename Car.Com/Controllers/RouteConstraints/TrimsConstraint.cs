using Car.Com.Common;
using Car.Com.Service.Data.Impl;
using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Car.Com.Controllers.RouteConstraints
{
  public class TrimsConstraint : IRouteConstraint
  {
    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      var trimSeoName = values["trimSeoName"] as string ?? String.Empty;

      return trimSeoName.IsNotNullOrEmpty() &&
             (UriTokenTranslators.GetAllTrimTranslators()
               .Any(trim => trim.SeoName.Equals(trimSeoName, StringComparison.InvariantCultureIgnoreCase)));
    }
  }
}
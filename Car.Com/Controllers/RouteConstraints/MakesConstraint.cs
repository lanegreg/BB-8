using Car.Com.Common;
using Car.Com.Service.Data.Impl;
using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Car.Com.Controllers.RouteConstraints
{
  public class MakesConstraint : IRouteConstraint
  {
    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      var makeSeoName = values["makeSeoName"] as string ?? String.Empty;

      return makeSeoName.IsNotNullOrEmpty() &&
             UriTokenTranslators.GetAllMakeTranslators()
               .Where(m => m.IsActive)
               .Any(make => make.SeoName.Equals(makeSeoName, StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
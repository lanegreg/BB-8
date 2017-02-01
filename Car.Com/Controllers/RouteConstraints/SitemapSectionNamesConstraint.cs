using Car.Com.Common;
using Car.Com.Domain.Services;
using Car.Com.Service;
using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Car.Com.Controllers.RouteConstraints
{
  public class SitemapSectionNamesConstraint : IRouteConstraint
  {
    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      var sectionName = values["sectionName"] as string ?? String.Empty;

      return sectionName.IsNotNullOrEmpty() &&
             ServiceLocator.Get<ISitemapService>().GetSitemapSectionNames()
               .Any(name => name.Equals(sectionName, StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
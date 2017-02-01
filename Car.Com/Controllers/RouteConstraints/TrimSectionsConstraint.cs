using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace Car.Com.Controllers.RouteConstraints
{
  public class TrimSectionsConstraint : IRouteConstraint
  {
    private static readonly List<string> TrimSectionSeoNames = new List<string>
    {
      "",
      "specifications",
      "incentives",
      "warranty",
      "safety",
      "color",
      "pictures-videos"
    };

    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      var trimSectionSeoName = values["trimSectionSeoName"] as string ?? String.Empty;

      return TrimSectionSeoNames.Contains(trimSectionSeoName);
    }
  }
}
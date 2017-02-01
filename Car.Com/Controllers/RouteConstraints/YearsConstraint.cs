using Car.Com.Service.Data.Impl;
using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Car.Com.Controllers.RouteConstraints
{
  public class YearsConstraint : IRouteConstraint
  {
    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      var yearNumberStr = values["yearNumber"] as string ?? String.Empty;
      int yearNumber;

      return Int32.TryParse(yearNumberStr, out yearNumber)
             && UriTokenTranslators.GetAllYearTranslators()
               .Any(year => year.Number == yearNumber);
    }
  }
}
using Car.Com.Common;
using Car.Com.Service.Data.Impl;
using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Car.Com.Controllers.RouteConstraints
{
  public class CategoriesConstraint : IRouteConstraint
  {
    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      var categorySeoName = values["categorySeoName"] as string ?? String.Empty;

      return categorySeoName.IsNotNullOrEmpty() &&
             UriTokenTranslators.GetAllCategoryTranslators()
               .Any(category => category.SeoName.Equals(categorySeoName, StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
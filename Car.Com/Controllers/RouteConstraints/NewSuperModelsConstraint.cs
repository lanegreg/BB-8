using Car.Com.Common;
using Car.Com.Service.Data.Impl;
using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Car.Com.Controllers.RouteConstraints
{
  public class NewSuperModelsConstraint : IRouteConstraint
  {
    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      var superModelSeoName = values["superModelSeoName"] as string ?? String.Empty;

      return superModelSeoName.IsNotNullOrEmpty() &&
             UriTokenTranslators.GetAllNewSuperModelTranslators()
               .Any(superModel => superModel.SeoName.Equals(superModelSeoName, StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
using Car.Com.Common;
using Car.Com.Service.Data.Impl;
using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Car.Com.Controllers.RouteConstraints
{
  public class VehicleAttributesConstraint : IRouteConstraint
  {
    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      var vehicleAttributeSeoName = values["vehicleAttributeSeoName"] as string ?? String.Empty;

      return vehicleAttributeSeoName.IsNotNullOrEmpty() &&
             (UriTokenTranslators.GetAllVehicleAttributeTranslators()
               .Any(vehicleAttribute => vehicleAttribute.SeoName.Equals(vehicleAttributeSeoName, StringComparison.InvariantCultureIgnoreCase)));
    }
  }
}
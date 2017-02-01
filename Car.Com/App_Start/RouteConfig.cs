using Car.Com.Controllers.RouteConstraints;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace Car.Com
{
  // Ref: http://blogs.msdn.com/b/webdev/archive/2013/10/17/attribute-routing-in-asp-net-mvc-5.aspx#enabling-attribute-routing
  public static class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {

      #region - Route Constraints Registration

      var resolver = new DefaultInlineConstraintResolver();
      
      resolver.ConstraintMap.Add("makes", typeof(MakesConstraint));
      resolver.ConstraintMap.Add("abt_makes", typeof(AbtMakesConstraint));
      resolver.ConstraintMap.Add("super_models", typeof(SuperModelsConstraint));
      resolver.ConstraintMap.Add("new_super_models", typeof(NewSuperModelsConstraint));
      resolver.ConstraintMap.Add("years", typeof(YearsConstraint));
      resolver.ConstraintMap.Add("research_years", typeof(ResearchYearsConstraint));
      resolver.ConstraintMap.Add("trims", typeof(TrimsConstraint));
      resolver.ConstraintMap.Add("trim_sections", typeof(TrimSectionsConstraint));
      resolver.ConstraintMap.Add("categories", typeof(CategoriesConstraint));
      resolver.ConstraintMap.Add("vehicle_attributes", typeof(VehicleAttributesConstraint));
      resolver.ConstraintMap.Add("sitemap_section_names", typeof(SitemapSectionNamesConstraint));

      #endregion


      #region - Routes Registration

      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapMvcAttributeRoutes(resolver);

      routes.MapRoute(
        name: "FallThrough_Status404",
        url: "{*url}",
        defaults: new
        {
          controller = "HttpError",
          action = "Status404"
        });

        #endregion
    }
  }
}

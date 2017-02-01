using Car.Com.Controllers.Filters;
using System.Web.Mvc;

namespace Car.Com
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new ErrorAttribute());
      //filters.Add(new HandleErrorAttribute());

#if !DEBUG
    filters.Add(new CacheabilityAttribute());
#endif
    }
  }
}

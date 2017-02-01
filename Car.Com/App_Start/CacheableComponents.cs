using System.Linq;
using Car.Com.Service;
using Car.Com.Service.Common;

namespace Car.Com
{
  public class CacheableComponents
  {
    public static void WarmThemUp()
    {
      // Service Layer : Call Warm() on all ICacheable types.
      ServiceLocator.GetAll<ICacheable>().ToList().ForEach(cache => cache.Warm());
    }
  }
}
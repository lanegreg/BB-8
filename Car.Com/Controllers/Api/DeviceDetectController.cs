using Car.Com.Common.Api;
using Car.Com.Common.DeviceDetection;
using System.Web;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api")]
  public class DeviceDetectController : BaseApiController
  {
    [Route("device/detect", Name = "GetDeviceType")]
    public DataWrapper GetDeviceType()
    {
      var deviceType = DeviceDetector.GetBrowserCaps(Request.Properties["MS_HttpContext"] as HttpContextWrapper);

      return DataWrapper(new
      {
        isMobi = deviceType.IsMobile,
        isTabl = deviceType.IsTablet,
        isDesk = deviceType.IsDesktop,
        deviceOS = deviceType.DeviceOs,
        brand = deviceType.BrandName,
        model = deviceType.ModelName
      });
    }
  }
}
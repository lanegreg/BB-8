using System.Linq;
using System.Web;
using Car.Com.Common.Api;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using System;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api")]
  public class GeoLocationController : BaseApiController
  {

    [Route("geolocation/ip", Name = "GetLocationDataByIp"), HttpGet]
    public DataWrapper GetLocationDataByIp()
    {
      string ip = "";

      HttpContext context = HttpContext.Current;
      string ipAddress =
                  !string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_X_TRUE_CLIENT_IP"]) ?
                    context.Request.ServerVariables["HTTP_X_TRUE_CLIENT_IP"] :
                  !string.IsNullOrEmpty(context.Request.ServerVariables["X_TRUE_CLIENT_IP"]) ?
                    context.Request.ServerVariables["X_TRUE_CLIENT_IP"] : 
                  !string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) ? 
                    context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] :
                  !string.IsNullOrEmpty(context.Request.ServerVariables["X_FORWARDED_FOR"]) ?
                    context.Request.ServerVariables["X_FORWARDED_FOR"] : "";

      if (!string.IsNullOrEmpty(ipAddress))
      {
        string[] addresses = ipAddress.Split(',');
        if (addresses.Length != 0)
        {
          ip = addresses[0];
        }
      }
      if (string.IsNullOrEmpty(ip) || ip.Length < 7 || ip.Count(x => x == '.') != 3)
      {
        ip = context.Request.UserHostAddress;
        //ip = "98.139.183.24";
      }

      if (string.IsNullOrEmpty(ip) || ip.Length < 7 || ip.Count(x => x == '.') != 3)
        return DataWrapper(new {ipaddress = ip});

      try
      {
        var locationData = GeoService.GetLocationDataByIpAddressAsync(ip).Result;
        return DataWrapper(locationData);
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    #region Services

    private static IGeoService GeoService
    {
      get { return ServiceLocator.Get<IGeoService>(); }
    }

    #endregion

  }
}

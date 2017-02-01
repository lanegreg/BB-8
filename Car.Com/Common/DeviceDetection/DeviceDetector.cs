using System;
using System.Web;
using WURFL;

namespace Car.Com.Common.DeviceDetection
{
  public sealed class DeviceDetector
  {
    private const string BrowserCapsKeyName = "browser_caps";
    private static readonly IWURFLManager WurflManager;

    static DeviceDetector()
    {
      Log.Info("Start: Initializing the WURFL Manager");
      try
      {
        var wurflDataFilePath =
          HttpContext.Current.Server.MapPath(String.Format("~/{0}", WebConfig.Get<string>("WURFL:DatabaseFilePath")).Replace("//", "/"));
        var configurer = new WURFL.Config.InMemoryConfigurer().MainFile(wurflDataFilePath);

        var patches = WebConfig.Get<string>("WURFL:PatchFilePaths").Split(new[] { '|', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (patches.Length > 0)
        {
          foreach (var patch in patches)
          {
            var wurflPatchFilePath = HttpContext.Current.Server.MapPath(String.Format("~/{0}", patch).Replace("//", "/"));
            configurer.PatchFile(wurflPatchFilePath);
          }
        }

        WurflManager = WURFLManagerBuilder.Build(configurer);
        Log.Info("Stop: Initializing the WURFL Manager");
      }
      catch (Exception ex)
      {
        Log.Error("DeviceDetector() ctor:", ex);
        throw;
      }
    }

    // Ref: http://wurfl.sourceforge.net/dotnet_index.php
    public static IBrowserCaps GetBrowserCaps(HttpContextBase httpContext)
    {
      BrowserCaps browserCaps;

      // We do not need to requery the WURFL manager if we have already
      // cached the BrowserCaps on the HttpContextBase for this instance.
      if (httpContext.Items.Contains(BrowserCapsKeyName))
      {
        browserCaps = httpContext.Items[BrowserCapsKeyName] as BrowserCaps;
        if (browserCaps != null)
          return browserCaps;
      }


      // Otherwise, query the WURFL manager for this HttpRequest.
      Log.Debug("Start: WURFL Query");
      var device = WurflManager.GetDeviceForRequest(httpContext.Request.UserAgent ?? String.Empty);
      bool isMobile, isTablet, hasCookieSupport;
      Boolean.TryParse(device.GetCapability("is_wireless_device"), out isMobile);
      Boolean.TryParse(device.GetCapability("is_tablet"), out isTablet);
      Boolean.TryParse(device.GetCapability("cookie_support"), out hasCookieSupport);

      browserCaps = new BrowserCaps
      {
        IsMobile = isMobile,
        IsTablet = isTablet,
        IsDesktop = !(isMobile || isTablet),
        HasCookieSupport = hasCookieSupport,
        BrandName = device.GetCapability("brand_name"),
        ModelName = device.GetCapability("model_name"),
        DeviceOs = device.GetCapability("device_os")
      };

      Log.Debug("Stop: WURFL Query");
      return browserCaps;
    }
  }
}
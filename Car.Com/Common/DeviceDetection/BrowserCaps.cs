using System;

namespace Car.Com.Common.DeviceDetection
{
  public class BrowserCaps : IBrowserCaps
  {
    public BrowserCaps()
    {
      BrandName = String.Empty;
      ModelName = String.Empty;
      DeviceOs = String.Empty;
    }
    public bool IsTablet { get; set; }
    public bool IsMobile { get; set; }
    public bool IsDesktop { get; set; }

    public bool HasCookieSupport { get; set; }
    public string BrandName { get; set; }
    public string ModelName { get; set; }
    public string DeviceOs { get; set; }
  }
}
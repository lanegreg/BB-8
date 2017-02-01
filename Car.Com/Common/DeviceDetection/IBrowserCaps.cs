
namespace Car.Com.Common.DeviceDetection
{
  public interface IBrowserCaps
  {
    bool IsTablet { get; }
    bool IsMobile { get; }
    bool IsDesktop { get; }

    bool HasCookieSupport { get; }
    string BrandName { get; }
    string ModelName { get; }
    string DeviceOs { get; }
  }
}

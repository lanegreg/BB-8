using Newtonsoft.Json;

namespace Car.Com.Common.Advertisement.Common
{
  public class PageContext
  {
    private bool _isMobi;

    [JsonProperty("isMobi")]
    public bool IsMobile
    {
      get { return _isMobi && !IsTablet; }
      set { _isMobi = value; }
    }

    [JsonProperty("isTabl")]
    public bool IsTablet { get; set; }

    [JsonProperty("isDesk")]
    public bool IsDesktop { get; set; }
  }
}
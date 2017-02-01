using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class TrimColor : Entity, ITrimColor
  {
    [JsonProperty("colorcode", NullValueHandling = NullValueHandling.Ignore)]
    public string ColorCode { get; set; }

    [JsonProperty("colortype", NullValueHandling = NullValueHandling.Ignore)]
    public string ColorType { get; set; }

    [JsonProperty("colordesc", NullValueHandling = NullValueHandling.Ignore)]
    public string ColorDesc { get; set; }

    [JsonProperty("extimage", NullValueHandling = NullValueHandling.Ignore)]
    public string ExtImage { get; set; }

    [JsonProperty("rgb", NullValueHandling = NullValueHandling.Ignore)]
    public string RGB { get; set; }
  }
}

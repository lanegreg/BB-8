using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class UniqueTrim : IUniqueTrim
  {
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("seo_name", NullValueHandling = NullValueHandling.Ignore)]
    public string SeoName { get; set; }
  }
}

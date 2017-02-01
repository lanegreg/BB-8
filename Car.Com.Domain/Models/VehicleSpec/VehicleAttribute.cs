using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class VehicleAttribute : Entity, IVehicleAttribute
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("seo_name")]
    public string SeoName { get; set; }
  }
}

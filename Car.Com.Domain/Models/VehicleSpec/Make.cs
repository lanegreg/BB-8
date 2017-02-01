using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class Make : Entity, IMake
	{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("seo_name")]
    public string SeoName { get; set; }

    [JsonProperty("plural_name")]
    public string PluralName { get; set; }

    [JsonProperty("abt_make_id")]
    public int AbtMakeId { get; set; }

    [JsonProperty("is_active")]
    public bool IsActive { get; set; }
	}
}

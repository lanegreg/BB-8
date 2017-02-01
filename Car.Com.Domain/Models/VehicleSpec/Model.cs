using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class Model : Entity, IModel
	{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("seo_name")]
    public string SeoName { get; set; }

    [JsonProperty("make_id")]
    public int MakeId { get; set; }

    [JsonProperty("abt_make_id")]
    public int AbtMakeId { get; set; }
	}
}

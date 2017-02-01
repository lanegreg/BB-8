using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class Category : Entity, ICategory
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("plural_name")]
    public string PluralName { get; set; }

    [JsonProperty("seo_name")]
    public string SeoName { get; set; }
  }
}

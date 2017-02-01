using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class Specification : Entity, ISpecification
  {
    [JsonProperty("group", NullValueHandling = NullValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public string Title { get; set; }

    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public string Data { get; set; }

    [JsonProperty("availability", NullValueHandling = NullValueHandling.Ignore)]
    public string Availability { get; set; }
  }
}

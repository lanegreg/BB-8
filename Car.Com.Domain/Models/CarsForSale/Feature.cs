using Newtonsoft.Json;

namespace Car.Com.Domain.Models.CarsForSale
{
  public sealed class Feature : IFeature
  {
    public Feature() { }

    public Feature(string matchValue, string description)
    {
      MatchValue = matchValue;
      Description = description;
    }


    [JsonProperty("matchVal")]
    public string MatchValue { get; set; }

    [JsonProperty("descr")]
    public string Description { get; set; }
  }
}

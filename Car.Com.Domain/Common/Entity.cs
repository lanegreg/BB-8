using Newtonsoft.Json;

namespace Car.Com.Domain.Common
{
  public class Entity
  {
    [JsonProperty("id")]
    public int Id { get; set; }
  }
}

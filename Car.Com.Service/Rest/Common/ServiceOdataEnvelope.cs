using Newtonsoft.Json;

namespace Car.Com.Service.Rest.Common
{
  public class ServiceOdataEnvelope : IWrapper
  {
    [JsonProperty("Value")]
    public object Data { get; set; }
  }
}

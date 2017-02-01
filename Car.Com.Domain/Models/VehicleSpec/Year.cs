using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
	public class Year
	{
    [JsonProperty("is_new")]
    public bool IsNew { get; set; }

    [JsonProperty("number")]
    public int Number { get; set; }

    public bool IsUsed
    {
      get { return !IsNew; }
    }
	}
}

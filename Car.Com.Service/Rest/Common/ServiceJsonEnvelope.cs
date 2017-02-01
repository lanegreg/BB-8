using Newtonsoft.Json;

namespace Car.Com.Service.Rest.Common
{
  public class ServiceJsonEnvelope : IWrapper
	{
    [JsonProperty("_server")]
    public string Server { get; set; }

    [JsonProperty("_time_to_process")]
    public string TimeToProcess { get; set; }

		[JsonProperty("record_count")]
		public int RecordCount { get; set; }

		[JsonProperty("query_url")]
		public string QueryUrl { get; set; }

    [JsonProperty("query_time")]
    public string QueryTime { get; set; }

		[JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
		public string Errors { get; set; }

		[JsonProperty("data")]
		public object Data { get; set; }
	}
}
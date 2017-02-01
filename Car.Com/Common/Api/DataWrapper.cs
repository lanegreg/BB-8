using System;
using Newtonsoft.Json;

namespace Car.Com.Common.Api
{
  public class DataWrapper
  {
    public DataWrapper()
		{
			Status = Api.Status.Undetermined;
			Data = String.Empty;
		}

		[JsonProperty("_time_to_process")]
		public string TimeToProcess { get; set; }

		[JsonProperty("_server")]
		public string Server { get; set; }
		
    [JsonProperty("status")]
		public string Status { get; set; }
		
    [JsonProperty("record_count")]
		public int RecordCount { get; set; }

		[JsonProperty("query_time")]
		public string QueryTime { get; set; }
		
    [JsonProperty("query_url")]
		public string QueryUrl { get; set; }
		
    [JsonProperty("data")]
		public object Data { get; set; }
  }
}
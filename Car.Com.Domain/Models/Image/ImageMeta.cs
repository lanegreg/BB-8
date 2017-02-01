using Newtonsoft.Json;

namespace Car.Com.Domain.Models.Image
{
  public class ImageMeta : IImageMeta
  {
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public int Id { get; set; }

    [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
    public int Year { get; set; }

    [JsonProperty("trim_id", NullValueHandling = NullValueHandling.Ignore)]
    public int TrimId { get; set; }

    [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
    public string Make { get; set; }

    [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
    public string Model { get; set; }

    [JsonProperty("acode", NullValueHandling = NullValueHandling.Ignore)]
    public string Acode { get; set; }

    [JsonProperty("url_prefix", NullValueHandling = NullValueHandling.Ignore)]
    public string UrlPrefix { get; set; }

    [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
    public string Source { get; set; }

    [JsonProperty("view", NullValueHandling = NullValueHandling.Ignore)]
    public string View { get; set; }

    [JsonProperty("category_view", NullValueHandling = NullValueHandling.Ignore)]
    public string CategoryView { get; set; }
  }
}

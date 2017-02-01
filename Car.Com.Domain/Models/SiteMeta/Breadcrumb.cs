using Newtonsoft.Json;

namespace Car.Com.Domain.Models.SiteMeta
{
  public class Breadcrumb
  {
    [JsonProperty]
    public int OrdinalPosition { get; set; }

    [JsonProperty]
    public string UriPattern { get; set; }


    public bool IsAnchor { get; set; }
    public string Href { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
  }
}

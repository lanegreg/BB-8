using Newtonsoft.Json;

namespace Car.Com.Domain.Models.SiteMeta
{
  public class Keyword
  {
    //public Keyword(int ordinalPos, string text)
    //{
    //  OrdinalPosition = ordinalPos;
    //  Text = (text != null ? text.ToLower() : String.Empty);
    //}

    [JsonProperty]
    public int OrdinalPosition { get; set; }

    [JsonProperty]
    public string Text { get; set; }
  }
}

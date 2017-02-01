using System.Collections.Generic;

namespace Car.Com.Domain.Models.SiteMeta
{
  public class OpenGraphMeta
  {
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public IList<string> Images { get; set; } 
  }
}

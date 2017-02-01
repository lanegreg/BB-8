using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public class TopTenClassification : ITopTenClassification
  {
    public string Title { get; set; }
    public string Blurb { get; set; }
    public int Ordinal { get; set; }
    public IEnumerable<ITopTenCategory> TopTenCategories { get; set; }
  }
}

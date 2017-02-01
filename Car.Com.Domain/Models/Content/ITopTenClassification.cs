using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public interface ITopTenClassification
  {
    string Title { get; }
    string Blurb { get; }
    int Ordinal { get; }
    IEnumerable<ITopTenCategory> TopTenCategories { get; }
  }
}
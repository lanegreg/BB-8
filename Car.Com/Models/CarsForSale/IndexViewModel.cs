using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;

namespace Car.Com.Models.CarsForSale
{
  public class IndexViewModel : ViewModelBase
  {
    public IndexViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {}

    public IndexViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {}

    public IEnumerable<Dto.Make> Makes { get; set; }
    public IEnumerable<Dto.Category> Categories { get; set; }

    public static class Dto
    {
      public class Make
      {
        public bool CanShow { get; set; }
        public string MatchValue { get; set; }
        public string Name { get; set; }
        public string SeoName { get; set; }
      }

      public class Category
      {
        public string MatchValue { get; set; }
        public string Name { get; set; }
      }
    }
  }
}
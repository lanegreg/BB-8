using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;

namespace Car.Com.Models.CarsForSale
{
  public class MakeViewModel : ViewModelBase
  {
    public MakeViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {}

    public MakeViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {}


    public Dto.Make Make { get; set; }
    public IEnumerable<Dto.Model> Models { get; set; }


    public static class Dto
    {
      public class Make
      {
        public string Name { get; set; }
        public string SeoName { get; set; }
      }

      public class Model
      {
        public string MatchValue { get; set; }
        public string Name { get; set; }
        public string SeoName { get; set; }
      }
    }
  }
}
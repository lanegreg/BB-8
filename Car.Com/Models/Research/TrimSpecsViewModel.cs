using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;

namespace Car.Com.Models.Research
{
  public class TrimSpecsViewModel : ViewModelBase
  {
    public TrimSpecsViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public TrimSpecsViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public string TrimSectionSeoName { get; set; }
    public int Year { get; set; }
    public Dto.Make Make { get; set; }
    public Dto.SuperModel SuperModel { get; set; }
    public Dto.Trim Trim { get; set; }
    public IEnumerable<Dto.Specifications> StandardSpecs { get; set; }
    public IEnumerable<Dto.Specifications> StandardFeatures { get; set; }
    public IEnumerable<Dto.Specifications> OptionalFeatures { get; set; }

    public static class Dto
    {
      public class Make
      {
        public string Name { get; set; }
        public string SeoName { get; set; }
      }

      public class SuperModel
      {
        public string Name { get; set; }
        public string SeoName { get; set; }
      }

      public class Trim
      {
        public int Year { get; set; }
        public string Acode { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string FullDisplayName { get; set; }
        public string SeoName { get; set; }
        public string SuperTrim { get; set; }
        public string Msrp { get; set; }
      }

      public class Specifications
      {
        public string Group { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
        public string Availability { get; set; }
      }

    }
  }
}

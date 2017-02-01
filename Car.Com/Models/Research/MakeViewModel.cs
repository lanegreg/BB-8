using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;

namespace Car.Com.Models.Research
{
  public class MakeViewModel : ViewModelBase
  {
    public MakeViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public MakeViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public Dto.Make Make { get; set; }
    public IEnumerable<Dto.SuperModel> SuperModels { get; set; }


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
        public string ImageAcode { get; set; }
        public string StartingMsrp { get; set; }
        public string FormattedStartingMsrp { get; set; }
        public string ImagePath { get; set; }
      }
    }
  }
}
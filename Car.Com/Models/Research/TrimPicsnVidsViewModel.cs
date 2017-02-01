using System.Collections.Generic;
using Car.Com.Domain.Models.SiteMeta;
using System;

namespace Car.Com.Models.Research
{
  public class PicsnVidsViewModel : ViewModelBase
  {
    public PicsnVidsViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public PicsnVidsViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public string TrimSectionSeoName { get; set; }
    public string MainImage { get; set; }
    public int Year { get; set; }
    public Dto.Make Make { get; set; }
    public Dto.SuperModel SuperModel { get; set; }
    public Dto.Trim Trim { get; set; }
    public IEnumerable<Dto.Image> Images { get; set; }

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
        public string Model { get; set; }
        public string Name { get; set; }
        public string FullDisplayName { get; set; }
        public string SeoName { get; set; }
        public string SuperTrim { get; set; }
        public double Msrp { get; set; }
      }

      public class Image
      {
        public string Small { get; set; }
        public string Medium { get; set; }
        public string Large { get; set; }
        public string ImageId { get; set; }
      }

    }
  }
}
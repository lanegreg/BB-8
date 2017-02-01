using Car.Com.Domain.Models.SiteMeta;
using System;

namespace Car.Com.Models.Research
{
  public class TrimOverviewViewModel : ViewModelBase
  {
    public TrimOverviewViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public TrimOverviewViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public string TrimSectionSeoName { get; set; }
    public string MainImage { get; set; }
    public int Year { get; set; }
    public Dto.Make Make { get; set; }
    public Dto.SuperModel SuperModel { get; set; }
    public Dto.Trim Trim { get; set; }
    public Dto.Image Images { get; set; }

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

      public class Image
      {
        public string Small { get; set; }
        public string Medium { get; set; }
        public string Large { get; set; }
        public string ImageId { get; set; }
      }
      
      public class Trim
      {
        const double IntRate = 0.05 / 12;

        public int TrimId { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string FullDisplayName { get; set; }
        public string SeoName { get; set; }
        public string SuperTrim { get; set; }
        public double Invoice { get; set; }
        public string CityMpg { get; set; }
        public string HwyMpg { get; set; }
        public string Doors { get; set; }
        public int PrimaryCategoryId { get; set; }
        public string Seating { get; set; }
        public string DriveTrain { get; set; }
        public string EngineType { get; set; }
        public string EngineSize { get; set; }
        public string BodyStyle { get; set; }
        public string FuelType { get; set; }
        public string DriveRange { get; set; }
        public string TowingCapacity { get; set; }
        public string CargoCapacity { get; set; }
        public string BedLength { get; set; }
        public string HorsePower { get; set; }
        public string RoofType { get; set; }
        public string Time0To60 { get; set; }
        public string Overview { get; set; }
        public string Template { get; set; }
        public double Msrp { get; set; }

        public string GetPayment(int months)
        {
          return String.Format("{0:0,0}", Msrp*(IntRate + (IntRate/(Math.Pow((1 + IntRate), months) - 1))));
        }
      }
    }
  }
}
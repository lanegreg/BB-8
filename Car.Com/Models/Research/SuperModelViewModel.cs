using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;

namespace Car.Com.Models.Research
{
  public class SuperModelViewModel : ViewModelBase
  {
    public SuperModelViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public SuperModelViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public Dto.Make Make { get; set; }
    public Dto.SuperModel SuperModel { get; set; }
    public IEnumerable<Dto.Trim> Trims { get; set; }
    public IEnumerable<Dto.SuperModelFilter> SuperModelFilters { get; set; }
    public IEnumerable<Dto.SuperModelYearItem> SuperModelYearItems { get; set; }

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
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string Model { get; set; }
        public string Msrp { get; set; }
        public string Name { get; set; }
        public string SeoName { get; set; }
        public string SuperTrim { get; set; }
        public string Year { get; set; }
        public string FullDisplayName { get; set; }

        public string CityMpg { get; set; }
        public string HighwayMpg { get; set; }
        public string Doors { get; set; }
        public string Seating { get; set; }
        public string Drivetrain { get; set; }
        public string EngineType { get; set; }
        public string EngineSize { get; set; }
        public string BodyStyle { get; set; }
        public string FuelType { get; set; }
        public string DriveRange { get; set; }
        public string TowingCapacity { get; set; }
        public string CargoCapacity { get; set; }
        public string Bedlength { get; set; }
        public string Horsepower { get; set; }
        public string RoofType { get; set; }
        public string Time0To60 { get; set; }
      }

      public class SuperModelFilter
      {
        public string FilterGroupName { get; set; }
        public string Code { get; set; }
      }

      public class SuperModelYearsJson
      {
        public string YearsJson { get; set; }
      }

      public class SuperModelYearItem
      {
        public string year { get; set; }
        public string is_new { get; set; }
      }

    }
  }
}
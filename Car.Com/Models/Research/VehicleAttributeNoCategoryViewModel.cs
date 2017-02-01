using System.Collections.Generic;
using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Research
{
  public class VehicleAttributeNoCategoryViewModel : ViewModelBase
  {
    public VehicleAttributeNoCategoryViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public VehicleAttributeNoCategoryViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public Dto.Category Category { get; set; }
    public IEnumerable<Dto.Category> Categories { get; set; }
    public Dto.VehicleAttribute VehicleAttribute { get; set; }
    public IEnumerable<Dto.Trim> Trims { get; set; }
    public IEnumerable<Dto.CategoryFilter> CategoryFilters { get; set; }

    public static class Dto
    {
      public class Category
      {
        public string Name { get; set; }
        public string SeoName { get; set; }
      }

      public class VehicleAttribute
      {
        public string Name { get; set; }
        public string SeoName { get; set; }
      }

      public class Trim
      {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string Make { get; set; }
        public string MakeSeoName { get; set; }
        public string Model { get; set; }
        public string Msrp { get; set; }
        public string Name { get; set; }
        public string SeoName { get; set; }
        public string SuperModel { get; set; }
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

      public class CategoryFilter
      {
        public string FilterGroupName { get; set; }
        public string Code { get; set; }
      }
    }
  }
}
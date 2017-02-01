using System.Collections.Generic;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public interface ITrim
  {
    string Name { get; }
    string SeoName { get; }
    string ImagePath { get; }

    int Id { get; }
    int Year { get; }
    string Make { get; }
    string MakeSeoName { get; }
    string SuperModel { get; }
    string SuperModelSeoName { get; }
    string SuperTrim { get; }

    string Msrp { get; }
    string Invoice { get; }
    string CityMpg { get; }
    string HighwayMpg { get; }
    string Doors { get; }
    string Seating { get; }
    string Drivetrain { get; }
    string EngineType { get; }
    string EngineSize { get; }
    string BodyStyle { get; }
    string FuelType { get; }
    string DriveRange { get; }
    string TowingCapacity { get; }
    string CargoCapacity { get; }
    string BedLength { get; }
    string HorsePower { get; }
    string RoofType { get; }
    string Time0To60 { get; }
    string Overview { get; }

    bool IsNew { get; }
    string Model { get; }
    int CategoryId { get; }
    string CanonicalSeoName { get; }
    string Acode { get; }
    string AbtName { get; }
    string FullDisplayName { get; }

    IEnumerable<Specification> Specifications { get; }
    IEnumerable<Specification> OptionalSpecs { get; }
    IEnumerable<Specification> SafetyItems { get; }
    IEnumerable<TrimColor> Colors { get; }
    IEnumerable<TrimIncentive> Incentives { get; }
  }
}
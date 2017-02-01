using System;
using System.Collections.Generic;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public interface ISuperModelTrimFilterAttribute
  {
    int Id { get; }
    string BodyStyle { get; }
    string Drivetrain { get; }
    string EngineType { get; }
    string EngineSize { get; }
    string FuelType { get; }
    string Transmission { get; }
    string BedLength { get; }
    string TowingCapacity { get; }
    string Seating { get; }
    string CargoCapacity { get; }
    string Navigation { get; }
    string DriveRange { get; }
    string VehicleCategory { get; }

  }
}



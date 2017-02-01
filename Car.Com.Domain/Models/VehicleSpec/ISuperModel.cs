using System.Collections.Generic;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public interface ISuperModel
  {
    int Year { get; }
    string YearsJson { get; }
    IEnumerable<SuperModel.Dto.Year> Years { get; }
    string Make { get; }
    string Name { get; }
    string ImageAcode { get; }
    string StartingMsrp { get; }
    string FormattedStartingMsrp { get; }
    string SeoName { get; }
  }
}
using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class DriveTypeFilter : IFilter
  {
    private readonly IList<string> _driveTypes = new List<string>();

    internal DriveTypeFilter() { }

    public DriveTypeFilter(string driveTypes)
    {
      _driveTypes = (driveTypes ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .ToList();
    }
    
    public bool MatchesThis(CarForSale car)
    {
      return !_driveTypes.Any() || _driveTypes.Contains(car.DriveType);
    }
  }
}

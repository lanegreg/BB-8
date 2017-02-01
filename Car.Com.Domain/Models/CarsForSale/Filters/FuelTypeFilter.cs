using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class FuelTypeFilter : IFilter
  {
    private readonly IList<int> _fuelTypeIds = new List<int>();

    internal FuelTypeFilter() { }

    public FuelTypeFilter(string fuelTypeIds)
    {
      _fuelTypeIds = (fuelTypeIds ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .Select(Int32.Parse)
        .ToList();
    }
    
    public bool MatchesThis(CarForSale car)
    {
      return !_fuelTypeIds.Any() || _fuelTypeIds.Contains(car.FuelTypeId);
    }
  }
}

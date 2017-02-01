using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class MileageRangeFilter : IFilter
  {
    private readonly IList<int> _mileages = new List<int>();

    internal MileageRangeFilter () { }

    public MileageRangeFilter(string pipeDelimitedMileages)
    {
      _mileages = (pipeDelimitedMileages ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .Select(m => Int32.Parse(m) * 1000)
        .ToList();
    }
    
    public bool MatchesThis(CarForSale car)
    {
      // Because the vehicle's Mileage is a continuous value type, we are going to check between a range.
      return !_mileages.Any() || _mileages.Contains(car.Mileage) || (car.Mileage >= _mileages.First() && car.Mileage <= _mileages.Last());
    }
  }
}

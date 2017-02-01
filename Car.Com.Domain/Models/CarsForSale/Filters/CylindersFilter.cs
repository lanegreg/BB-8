using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class CylindersFilter : IFilter
  {
    private readonly IList<int> _cylinders = new List<int>();

    internal CylindersFilter() { }

    public CylindersFilter(string cylinders)
    {
      _cylinders = (cylinders ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .Select(Int32.Parse)
        .ToList();
    }
    
    public bool MatchesThis(CarForSale car)
    {
      return !_cylinders.Any() || _cylinders.Contains(car.Cylinders);
    }
  }
}

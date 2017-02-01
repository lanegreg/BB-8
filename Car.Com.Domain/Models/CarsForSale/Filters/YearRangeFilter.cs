using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class YearRangeFilter : IFilter
  {
    private readonly IList<int> _years = new List<int>();

    internal YearRangeFilter() { }

    public YearRangeFilter(string pipeDelimitedYears)
    {
      _years = (pipeDelimitedYears ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .Select(Int32.Parse)
        .ToList();
    }
    
    public bool MatchesThis(CarForSale car)
    {
      // Because the vehicle's Year is a discrete value type, we are going to check for a specific match before we check between a range.
      return !_years.Any() || _years.Contains(car.Year) || (car.Year >= _years.First() && car.Year <= _years.Last());
    }
  }
}

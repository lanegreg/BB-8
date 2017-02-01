using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class PriceRangeFilter : IFilter
  {
    private readonly IList<int> _prices = new List<int>();

    internal PriceRangeFilter() { }

    public PriceRangeFilter(string pipeDelimitedPrices)
    {
      _prices = (pipeDelimitedPrices ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .Select(p => Int32.Parse(p) * 1000)
        .ToList();
    }
    
    public bool MatchesThis(CarForSale car)
    {
      // Because the vehicle's AskingPrice is a continuous value type, we are going to check between a range.
      return !_prices.Any() || (car.AskingPrice >= _prices.First() && car.AskingPrice <= _prices.Last());
    }
  }
}

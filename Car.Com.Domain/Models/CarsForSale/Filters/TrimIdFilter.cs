using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class TrimIdFilter : IFilter
  {
    private readonly IList<int> _trimIds = new List<int>();

    internal TrimIdFilter () { }

    public TrimIdFilter(string pipeDelimitedTrimIds)
    {
      _trimIds = (pipeDelimitedTrimIds ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .Select(Int32.Parse)
        .ToList();
    }
    
    public bool MatchesThis(CarForSale car)
    {
      return !_trimIds.Any() || _trimIds.Contains(car.TrimId);
    }
  }
}

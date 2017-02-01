using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class TransmissionTypeFilter : IFilter
  {
    private readonly IList<string> _trannyTypes = new List<string>();

    internal TransmissionTypeFilter() { }

    public TransmissionTypeFilter(string trannyTypes)
    {
      _trannyTypes = (trannyTypes ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .ToList();
    }
    
    public bool MatchesThis(CarForSale car)
    {
      return !_trannyTypes.Any() || _trannyTypes.Contains(car.TransmissionType);
    }
  }
}

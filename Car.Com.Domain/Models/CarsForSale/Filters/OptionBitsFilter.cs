using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class OptionBitsFilter : IFilter
  {
    private readonly long _optionBits;

    internal OptionBitsFilter() { }

    public OptionBitsFilter(string optionIds)
    {
      var ids = (optionIds ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .Select(Int32.Parse)
        .ToList();

      foreach (var mask in ids.Select(id => (long)Math.Pow(2, id)))
        _optionBits = _optionBits | mask;
    }

    public bool MatchesThis(CarForSale car)
    {
      return ((_optionBits & car.OptionBits) == _optionBits);
    }
  }
}

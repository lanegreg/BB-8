using Car.Com.Common;
using Car.Com.Domain.Models.CarsForSale.Common;
using System;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class HighwayMpgFilter : IFilter
  {
    private readonly string _cityMpg = String.Empty;

    internal HighwayMpgFilter() { }

    public HighwayMpgFilter(string cityMpg)
    {
      _cityMpg = (cityMpg ?? String.Empty).Trim();
    }
    
    public bool MatchesThis(CarForSale car)
    {
      return _cityMpg.IsNullOrEmpty() || _cityMpg == car.CityMpg;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Car.Com.Domain.Models.CarsForSale.Common;

namespace Car.Com.Domain.Models.CarsForSale.Exclusions
{
  public sealed class DealersExclusion : IExclusion
  {
        private readonly IList<int> _dealers = new List<int>();

    internal DealersExclusion() { }

    public DealersExclusion(string pipeDelimitedDealers)
    {
      _dealers = (pipeDelimitedDealers ?? String.Empty)
        .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(d => Int32.Parse(d))
        .ToList();
    }

    bool IExclusion.MatchesThis(CarForSale car)
    {
      return !_dealers.Contains(car.DealerId);
    }
  }
}

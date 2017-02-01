using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.CarsForSale.Exclusions
{
    public sealed class CarsDealerDedupe : IEqualityComparer<CarForSale>
  {
    #region IEqualityComparer<CarForSale> Members

    public bool Equals(CarForSale x, CarForSale y)
    {
      return x.DealerId.Equals(y.DealerId);
    }

    public int GetHashCode(CarForSale obj)
    {
      return obj.DealerId.GetHashCode();
    }

    #endregion
  }
}

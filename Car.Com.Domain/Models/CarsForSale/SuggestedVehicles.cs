using Car.Com.Common;
using Car.Com.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale
{
  public class SuggestedVehicles : CarForSale, ISuggestedVehicles
  {
    #region Interface Implementation

    public int deltaMileage { get; set; }
    public int deltaPrice { get; set; }

    #endregion




  }
}

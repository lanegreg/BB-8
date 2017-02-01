using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.CarsForSale
{
  public interface ISuggestedVehicles
  {
    int deltaMileage { get; set; }
    int deltaPrice { get; set; }
  }
}

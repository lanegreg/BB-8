using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.Content
{
  public interface ICategory
  {
    int VehicleCategoryId { get; set; }
    string VehicleCategory { get; set; }
    string ContentPriority { get; set; }
  }
}

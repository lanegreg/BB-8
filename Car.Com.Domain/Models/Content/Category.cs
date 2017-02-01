using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.Content
{
  public class Category : ICategory
  {
    public int VehicleCategoryId { get; set; }
    public string VehicleCategory { get; set; }
    public string ContentPriority { get; set; }
  }
}

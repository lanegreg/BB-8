using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public class TopTenCategory : ITopTenCategory
  {
    public string Title { get; set; }
    public string CategoryCode { get; set; }
    public IEnumerable<ITopTenCar> TopTenCars { get; set; }
  }
}

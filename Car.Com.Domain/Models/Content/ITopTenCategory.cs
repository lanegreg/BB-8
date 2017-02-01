using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public interface ITopTenCategory
  {
    string Title { get; }
    string CategoryCode { get; }
    IEnumerable<ITopTenCar> TopTenCars { get; }
  }
}
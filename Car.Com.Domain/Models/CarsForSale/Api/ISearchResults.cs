using System.Collections.Generic;

namespace Car.Com.Domain.Models.CarsForSale.Api
{
  public interface ISearchResults
  {
    IEnumerable<ICarForSale> CarsForSale { get; }
    int InventoryCount { get; }
    IPage Page { get; }
    int MaxPrice { get; }
    int MaxMileage { get; }
    int MinPrice { get; }
    int MinMileage { get; }
    string PriceGroupQuantities { get; }
    string MileageGroupQuantities { get; }
  }
}
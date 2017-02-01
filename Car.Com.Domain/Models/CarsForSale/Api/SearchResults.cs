using System;
using System.Collections.Generic;

namespace Car.Com.Domain.Models.CarsForSale.Api
{
  public sealed class SearchResults : ISearchResults
  {
    public SearchResults(
      IEnumerable<ICarForSale> carsForSale, 
      int inventoryCount, 
      int maxPrice, 
      int maxMileage, 
      int minPrice, 
      int minMileage, 
      string priceGroupQuantities,
      string mileageGroupQuantities,
      SearchCriteria criteria)
    {
      CarsForSale = carsForSale;
      InventoryCount = inventoryCount;
      MaxPrice = maxPrice;
      MaxMileage = maxMileage;
      MinPrice = minPrice;
      MinMileage = minMileage;
      PriceGroupQuantities = priceGroupQuantities;
      MileageGroupQuantities = mileageGroupQuantities;

      var itemsPerPage = criteria.Take;
      var currentPage = ((criteria.Skip / itemsPerPage) + 1);
      var totalPages = (int)Math.Ceiling((decimal)inventoryCount / itemsPerPage);
      Page = new Page(currentPage, itemsPerPage, totalPages);
    }

    public IEnumerable<ICarForSale> CarsForSale { get; private set; }
    public int InventoryCount { get; private set; }
    public IPage Page { get; private set; }
    public int MaxPrice { get; private set; }
    public int MaxMileage { get; private set; }
    public int MinPrice { get; private set; }
    public int MinMileage { get; private set; }
    public string PriceGroupQuantities { get; private set; }
    public string MileageGroupQuantities { get; private set; }
  }
}
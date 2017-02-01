using Car.Com.Domain.Models.CarsForSale;
using Car.Com.Domain.Models.CarsForSale.Api;
using Car.Com.Domain.Models.CarsForSale.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Domain.Services
{
  public interface ICarsForSaleService
  {
    Task<ISearchResults> SearchAsync(SearchCriteria criteria);
    ISearchResults Search(SearchCriteria criteria);

    Task<ISearchResults> SuggestedSearchAsync(SearchCriteria criteria);
    ISearchResults SuggestedSearch(SearchCriteria criteria);

    Task<ICarForSale> GetCarForSaleByDealerIdByInventoryIdAsync(int dealerId, int inventoryId);
    ICarForSale GetCarForSaleByDealerIdByInventoryId(int dealerId, int inventoryId);

    Task<IEnumerable<IFeature>> GetModelsDomainByMakeIdAsync(int makeId);
    IEnumerable<IFeature> GetModelsDomainByMakeId(int makeId);

    Task<IEnumerable<IFeature>> GetMakesDomainByCategoryIdAsync(int categoryId);
    IEnumerable<IFeature> GetMakesDomainByCategoryId(int categoryId);

    IEnumerable<string> MakesBlackList { get; }
    IFilterDomains FilterDomains { get; }

    IDictionary<string, int> GetTopMakesWithInventoryCount(int take);
    IDictionary<string, int> GetAllMakesWithInventoryCount();
  }
}

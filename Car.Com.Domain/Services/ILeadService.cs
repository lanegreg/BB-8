using Car.Com.Domain.Models.Lead;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Domain.Services
{
  public interface ILeadService
  {
    Task<List<int>> GetLeadAutonationUsedCarDealersAsync();
    List<int> GetLeadAutonationUsedCarDealers();

    Task<IEnumerable<DealerHours>> GetLeadAutonationUsedCarDealerHoursAsync(int cyberId);
    IEnumerable<DealerHours> GetLeadAutonationUsedCarDealerHours(int cyberId);

    Task<List<int>> GetLeadAutonationNewCarDealersAsync();
    List<int> GetLeadAutonationNewCarDealers();

    Task<IEnumerable<IDealer>> GetLeadDealersByPrNumberAsync(int prnumber);
    IEnumerable<IDealer> GetLeadDealersByPrNumber(int prnumber);

    Task<IEnumerable<IDealer>> GetLeadDealersByPrNumberListAsync(string prnumberList);
    IEnumerable<IDealer> GetLeadDealersByPrNumberList(string prnumberList);
  }
}

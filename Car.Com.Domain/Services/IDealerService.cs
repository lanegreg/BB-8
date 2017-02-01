using Car.Com.Domain.Models.Dealer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Domain.Services
{
  public interface IDealerService
  {
    Task<IEnumerable<IDisplayContent>> GetDisplayContentsByDealerIdsAsync(IEnumerable<int> dealerIds);
    IEnumerable<IDisplayContent> GetDisplayContentsByDealerIds(IEnumerable<int> dealerIds);

    Task<IDisplayContent> GetDisplayContentByDealerIdAsync(int dealerId);
    IDisplayContent GetDisplayContentByDealerId(int dealerId);

    Task<IEnumerable<ITexasAdContent>> GetTexasAdContentsByDealerIdsAsync(IEnumerable<int> dealerIds);
    IEnumerable<ITexasAdContent> GetTexasAdContentsByDealerIds(IEnumerable<int> dealerIds);
  }
}

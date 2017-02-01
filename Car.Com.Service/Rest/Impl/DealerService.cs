using System.Linq;
using Car.Com.Common;
using Car.Com.Domain.Models.Dealer;
using Car.Com.Domain.Services;
using Car.Com.Service.Rest.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Service.Rest.Impl
{
  public class DealerService : RestServiceBase<ServiceOdataEnvelope>, IDealerService
  {
    #region Declarations

    private const string DealerTierTwoHashKey = "dlr_svc:t2_cache";
    private const string TierTwoDisplayContentByDealerIdFieldNameTempl = "display_content:by_dealer_id[{0}]";
    private const string TierTwoTexasAdContentByDealerIdFieldNameTempl = "texas_ad_content:by_dealer_id[{0}]";
    private const double LocalCacheTimeToLiveInSecs = 120; // 2 Minutes

    private static readonly ConnectionMultiplexer RedisReadable;
    private static readonly Uri Endpoint;
    private static readonly string PathPrefix;

    private static readonly int DatabaseNumber = WebConfig.Get<int>("DealerService:Redis:DatabaseNumber");

    #endregion

    #region ctors

    static DealerService()
    {
      RedisReadable = ConnectionMultiplexer.Connect(WebConfig.Get<string>("Redis:Readable:Config"));
      PathPrefix = String.Format("/api/v{0}/", WebConfig.Get<string>("DealerService:ApiVersion"));
      Endpoint = new Uri(String.Format("http://{0}", WebConfig.Get<string>("DealerService:Endpoint")));
    }

    public DealerService()
      : base(WebConfig.Get<int>("DealerService:Timeout_ms"))
    {}

    #endregion


    public async Task<IEnumerable<IDisplayContent>> GetDisplayContentsByDealerIdsAsync(IEnumerable<int> dealerIds)
    {
      const string pathTempl = "odata/GetCarDealerList(dealerIdList='{0}')"; // 132703, 133598, 164892, 100021, 12121, 42323, 12, 100029      

      var path = String.Format(pathTempl, dealerIds.Join(","));

      return await FetchResource<IEnumerable<DisplayContent>>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);
    }

    public IEnumerable<IDisplayContent> GetDisplayContentsByDealerIds(IEnumerable<int> dealerIds)
    {
      return GetDisplayContentsByDealerIdsAsync(dealerIds).Result;
    }



    public async Task<IDisplayContent> GetDisplayContentByDealerIdAsync(int dealerId)
    {
      const string pathTempl = "odata/CarDealer({0})"; // 132703, 133598, 164892, 100021, 12121, 42323, 12, 100029      

      var path = String.Format(pathTempl, dealerId);

      var displayContent = await FetchResource<IEnumerable<DisplayContent>>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

      return displayContent.FirstOrDefault();
    }

    public IDisplayContent GetDisplayContentByDealerId(int dealerId)
    {
      return GetDisplayContentByDealerIdAsync(dealerId).Result;
    }



    public async Task<IEnumerable<ITexasAdContent>> GetTexasAdContentsByDealerIdsAsync(IEnumerable<int> dealerIds)
    {
      const string pathTempl = "odata/GetCarTexasAdList(dealerIdList='{0}')"; // 132703, 133598, 164892, 100021, 12121, 42323, 12, 100029

      var path = String.Format(pathTempl, dealerIds.Join(","));

      return await FetchResource<IEnumerable<TexasAdContent>>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);
    }

    public IEnumerable<ITexasAdContent> GetTexasAdContentsByDealerIds(IEnumerable<int> dealerIds)
    {
      return GetTexasAdContentsByDealerIdsAsync(dealerIds).Result;
    }

  }
}

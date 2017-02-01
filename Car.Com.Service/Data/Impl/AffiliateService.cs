using Car.Com.Common;
using Car.Com.Domain.Models.Affiliate;
using Car.Com.Domain.Services;
using Car.Com.Service.Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;

namespace Car.Com.Service.Data.Impl
{
  public class AffiliateService : IAffiliateService, ICacheable
  {
    #region Declarations

    private const int CarDotDomId = 24093;
    private static readonly string AbtProdConnString = WebConfig.Get<string>("ConnectionString:Abt_Prod");
    private static readonly int RefreshIntervalInMins = WebConfig.Get<int>("AffiliateService:ReCacheInterval_mins");
    private static readonly object Mutex = new object();
    private static readonly MemoryCache Cache = MemoryCache.Default;
    
    private static IEnumerable<Affiliate> _affiliates;

    #endregion

    #region Interface Impls

    public void Warm()
    {
      CacheAllAffiliates();
      Cache.Set("Affiliate_Service", String.Empty, GetCachePolicy());
    }


    public int CarDotComId { get { return CarDotDomId; } }

    public IAffiliate GetAffiliateById(int id)
    {
      return _affiliates.FirstOrDefault(a => a.Id == id);
    }

    #endregion


    #region Private Static Methods

    private static void CacheUpdateHandler(CacheEntryUpdateArguments args)
    {
      CacheAllAffiliates();
      var cacheItem = Cache.GetCacheItem(args.Key);

      if (cacheItem != null)
        cacheItem.Value = String.Empty;
      else
        cacheItem = new CacheItem("cacheItem", String.Empty);

      args.UpdatedCacheItem = cacheItem;
      args.UpdatedCacheItemPolicy = GetCachePolicy();
    }

    private static CacheItemPolicy GetCachePolicy()
    {
      return new CacheItemPolicy
      {
        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(RefreshIntervalInMins),
        UpdateCallback = CacheUpdateHandler
      };
    }

    private static void CacheAllAffiliates()
    {
      using (var conn = AbtProdDbConn())
      {
        var affiliates = conn.Query<Affiliate>("CCWeb.GetAffiliatesCache");

        lock (Mutex)
        {
          _affiliates = affiliates;
        }
      }
    }

    #endregion

    #region Connections

    private static IDbConnection AbtProdDbConn()
    {
      return ServiceLocator.Get<IDbConnectionFactory>().CreateConnection(AbtProdConnString);
    }

    #endregion
  }
}

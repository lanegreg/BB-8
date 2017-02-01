using Car.Com.Common;
using Car.Com.Common.Cache;
using Car.Com.Domain.Models.Lead;
using Car.Com.Domain.Services;
using Car.Com.Service.Common;
using System.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Service.Soap.Impl
{
  public class LeadService : ILeadService
  {
    #region Declarations

    private static readonly string AbtProdConnString = WebConfig.Get<string>("ConnectionString:Abt_Prod");

    private const double LocalCacheTimeToLiveInSecs = 7200; // 2 Hours

    #endregion

    public async Task<List<int>> GetLeadAutonationNewCarDealersAsync()
    {
      var localCacheKey =
        String.Format("lead_pr_autonationdealerlistnew");

      // First, check LocalCache.
      var dealerList = LocalCache.Get<List<int>>(localCacheKey);
      if (dealerList != null)
        return dealerList;
      
      // If not found in cache, get data and store in cache.
      dealerList = GetAutonationNewCarDealers();
      LocalCache.Put(localCacheKey, dealerList, LocalCacheTimeToLiveInSecs);
      return dealerList;
    }

    public List<int> GetLeadAutonationNewCarDealers()
    {
      return GetLeadAutonationNewCarDealersAsync().Result;
    }

    public async Task<List<int>> GetLeadAutonationUsedCarDealersAsync()
    {
      var localCacheKey =
        String.Format("lead_pr_autonationdealerlist");

      // First, check LocalCache.
      var dealerList = LocalCache.Get<List<int>>(localCacheKey);
      if (dealerList != null)
        return dealerList;
      
      // If not found in cache, get data and store in cache.
      dealerList = GetAutonationUsedCarDealers();
      LocalCache.Put(localCacheKey, dealerList, LocalCacheTimeToLiveInSecs);
      return dealerList;
    }

    public List<int> GetLeadAutonationUsedCarDealers()
    {
      return GetLeadAutonationUsedCarDealersAsync().Result;
    }

    public async Task<IEnumerable<DealerHours>> GetLeadAutonationUsedCarDealerHoursAsync(int cyberId)
    {
      var localCacheKey =
        String.Format("lead_pr_autonationdealerhours:by_cyberid[{0}]", cyberId);

      // First, check LocalCache.
      var dealerHours = LocalCache.Get<IEnumerable<DealerHours>>(localCacheKey);
      if (dealerHours != null)
        return dealerHours;
      
      // If not found in cache, get data and store in cache.
      dealerHours = GetAutonationUsedCarDealerHours(cyberId);
      LocalCache.Put(localCacheKey, dealerHours, LocalCacheTimeToLiveInSecs);
      return dealerHours;
    }

    public IEnumerable<DealerHours> GetLeadAutonationUsedCarDealerHours(int cyberId)
    {
      return GetLeadAutonationUsedCarDealerHoursAsync(cyberId).Result;
    }

    public async Task<IEnumerable<IDealer>> GetLeadDealersByPrNumberAsync(int prnumber)
    {
      var localCacheKey =
        String.Format("lead_pr_dealerlist:by_prnumber[{0}]", prnumber);

      // First, check LocalCache.
      var cityList = LocalCache.Get<ICollection<IDealer>>(localCacheKey);
      if (cityList != null)
        return cityList;


      // If not found in cache, get data and store in cache.
      cityList = GetDealerListByPrNumber(prnumber);
      LocalCache.Put(localCacheKey, cityList, LocalCacheTimeToLiveInSecs);
      return cityList;
    }

    public IEnumerable<IDealer> GetLeadDealersByPrNumber(int prnumber)
    {
      return GetLeadDealersByPrNumberAsync(prnumber).Result;
    }

    public async Task<IEnumerable<IDealer>> GetLeadDealersByPrNumberListAsync(string prnumberList)
    {
      if (prnumberList.Length < 1)
        return new List<Dealer>();

      // Place string list into array of ints
      string[] prnumberStrArr = prnumberList.Split(',');
      var ints = new List<int>();
      foreach (var item in prnumberStrArr)
      {
        int v;
        if (int.TryParse(item, out v))
          ints.Add(v);
      }
      int[] prnumberArr = ints.ToArray();

      var resultDealerList = new List<Dealer>();

      foreach (var p in prnumberArr)
      {
        // First, check LocalCache.
        var localCacheKey =
          String.Format("lead_pr_dealerlist:by_prnumber[{0}]", p);
        var dealerList = LocalCache.Get<ICollection<IDealer>>(localCacheKey);
        if (dealerList != null)
        {
          foreach (var d in dealerList)
          {
            resultDealerList.Add(new Dealer()
            {
              Id = d.Id,
              Name = d.Name,
              Address = d.Address,
              City = d.City,
              State = d.State,
              Zip = d.Zip,
              Message = d.Message,
              Phone = d.Phone,
              LogoUrl = d.LogoUrl,
              LogoWidth = d.LogoWidth,
              LogoHeight = d.LogoHeight,
              ConfirmationNum = p,
            });
          }
        }
        // If not found in cache, get data and store in cache.
        else
        {
          dealerList = GetDealerListByPrNumber(p);
          LocalCache.Put(localCacheKey, dealerList, LocalCacheTimeToLiveInSecs);
          foreach (var d in dealerList)
          {
            resultDealerList.Add(new Dealer()
            {
              Id = d.Id,
              Name = d.Name,
              Address = d.Address,
              City = d.City,
              State = d.State,
              Zip = d.Zip,
              Message = d.Message,
              Phone = d.Phone,
              LogoUrl = d.LogoUrl,
              LogoWidth = d.LogoWidth,
              LogoHeight = d.LogoHeight,
              ConfirmationNum = p,
            });
          }
        }
        
      }

      return resultDealerList;
    }

    public IEnumerable<IDealer> GetLeadDealersByPrNumberList(string prnumberList)
    {
      return GetLeadDealersByPrNumberListAsync(prnumberList).Result;
    }


    #region Private Static Methods

    private static List<int> GetAutonationNewCarDealers()
    {
      using (var conn = AbtProdDbConn())
      {
        var dealerList = (List<int>)conn.Query<int>("CCWeb.GetAutoNationDealers", commandType: CommandType.StoredProcedure);
        return dealerList;
      }
    }

    private static List<int> GetAutonationUsedCarDealers()
    {
      using (var conn = AbtProdDbConn())
      {
        var dealerList = (List<int>)conn.Query<int>("CCWeb.GetAutoNationUsedCarDealers", commandType: CommandType.StoredProcedure);
        return dealerList;
      }
    }

    private static IEnumerable<DealerHours> GetAutonationUsedCarDealerHours(int cyberId)
    {
      using (var conn = AbtProdDbConn())
      {
        var dealerHours = (IEnumerable<DealerHours>)conn.Query<DealerHours>("Consumer.CCWeb.GetAutoNationUsedCarDealerHoursByCyberId", 
          new { CyberId = cyberId },
          commandType: CommandType.StoredProcedure);
        return dealerHours;
      }
    }

    private static ICollection<IDealer> GetDealerListByPrNumber(int prnumber)
    {
      using (var conn = AbtProdDbConn())
      {
        // This gets us the city list for a given zipcode.
        var dealerList = (ICollection<Dealer>)conn.Query<Dealer>("CCWeb.GetPurchaseRequestDealers",
            new { prnumber = prnumber },
            commandType: CommandType.StoredProcedure);

        var response = new List<IDealer>();

        foreach (var cItem in dealerList)
        {
          response.Add(new Dealer()
          {
            Id = cItem.Id,
            Name = cItem.Name,
            Address = cItem.Address,
            City = cItem.City,
            State = cItem.State,
            Zip = cItem.Zip,
            Message = cItem.Message,
            Phone = cItem.Phone,
            LogoUrl = cItem.LogoUrl,
            LogoWidth = cItem.LogoWidth,
            LogoHeight = cItem.LogoHeight,
          });
        }

        return response;
      }
    }


    #endregion






    #region Database Connections

    private static IDbConnection AbtProdDbConn()
    {
      return ServiceLocator.Get<IDbConnectionFactory>().CreateConnection(AbtProdConnString);
    }

    #endregion

    #region Local Memory Cache

    private static ILocalCache LocalCache
    {
      get { return ServiceLocator.Get<ILocalCache>(); }
    }

    #endregion

  }
}

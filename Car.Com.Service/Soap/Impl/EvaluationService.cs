using Car.Com.Common;
using Car.Com.Common.Cache;
using Car.Com.Domain.Models.Evaluation;
using Car.Com.Domain.Services;
using Car.Com.Service.Common;
using Car.Com.Service.Kbb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Car.Com.Service.Soap.Impl
{
  public sealed class EvaluationService : IEvaluationService, ICacheable
  {
    #region Declarations

    private static readonly string KbbServiceEndpoint = WebConfig.Get<string>("KbbService:Endpoint");
    private static readonly PricingService KbbService;

    private const int RefreshIntervalInMins = 1440;
    private static readonly object Mutex = new object();
    private static readonly MemoryCache Cache = MemoryCache.Default;

    private static IEnumerable<EvaluationType> _evaluationTypes;
    private static IEnumerable<Year> _years;
    private static IDictionary<int, IEnumerable<Make>> _makesByYearDict;

    #endregion



    #region ctors

    static EvaluationService()
    {
      KbbService = new PricingService {Url = KbbServiceEndpoint};

      _evaluationTypes = new List<EvaluationType>
      {
        new EvaluationType
        {
          Key = "TI",
          Value = "Trade-In"
        },
        new EvaluationType
        {
          Key = "PP",
          Value = "Private-Party"
        },
        new EvaluationType
        {
          Key = "R",
          Value = "Suggested-Retail"
        }
      };
    }

    #endregion


    #region Interface Method Implementations

    public void Warm()
    {
      CacheData();
      Cache.Set("Evaluation_Service", String.Empty, GetCachePolicy());
    }


    public IEnumerable<EvaluationType> EvaluationTypes { get { return _evaluationTypes; } }


    public IEnumerable<Year> GetYears()
    {
      return _years;
    }


    public IEnumerable<Make> GetMakesByYear(int year)
    {
      return _makesByYearDict.ContainsKey(year)
        ? _makesByYearDict[year]
        : new List<Make>();
    }


    public IEnumerable<Trim> GetTrimsByYearByMakeByEvaluationType(int year, string makeKey, string evalTypeKey)
    {
      var models = KbbService
        // This is some stupid crap we have to do because of the stinking SOAP service!
        .GetModelTrimByYearMake(year, makeKey, evalTypeKey).Years.First().Make.First().Trim
        .Select(m => new Trim
        {
          Key = m.ModelID,
          Value = m.TrimDesc
        })
        .ToList();
        
      return models;
    }


    public IEnumerable<Feature> GetFeaturesByTrimByFeatureType(string trimKey, FeatureType featureType)
    {
      const int localCacheTtl = 600; // 1 hour (in secs)
      var cacheKey = String.Format("eval_svc:features:by_trim_key[{0}]", trimKey);

      var features = LocalCache.Get<IEnumerable<Feature>>(cacheKey, () =>
      {
        var list = KbbService
          // This is some stupid crap we have to do because of the stinking SOAP service!
          .GetEquipmentsByVehicleID(trimKey).Years.First().Make.First().Trim.First().Equipments
          .Select(f => new Feature
          {
            Key = f.Code,
            Value = f.Description,
            PreSelect = f.PreSelected.Equals(true.ToString()),
            DisplayOrder = Int32.Parse(f.DisplayOrder),
            Type = GetFeatureTypeFromGroupCode(f.GroupCode)
          })
          .ToList();

        LocalCache.Put(cacheKey, list, localCacheTtl);

        return list;
      });


      return features
        .Where(f => f.Type == featureType)
        .OrderBy(f => f.DisplayOrder)
        .ToList();
    }

    public string GetVehicleValue(string trimKey, string valueType, string conditionType, string mileage, string zipCode, string equipments)
    {
      var vehicleValueList = new List<string>();

      var vehicleValue = KbbService
           .GetVehicleValue(trimKey, valueType, conditionType, mileage, zipCode, equipments).Years.First().Make.First().Trim.First().VehicleValue.First().Value;

      return vehicleValue;
    }


    public Evaluation GetEvaluation(Criteria criteria)
    {
      var evaluation = new Evaluation();

      return evaluation;
    }

    #endregion




    #region Private Static Methods

    private static FeatureType GetFeatureTypeFromGroupCode(string code)
    {
      switch (code)
      {
        case "D":
          return FeatureType.Drives;

        case "E":
          return FeatureType.Engines;

        case "T":
          return FeatureType.Transmissions;

        case "M":
          return FeatureType.Options;

        default:
          return FeatureType.None;
      }
    }

    #region Cache

    private static void CacheUpdateHandler(CacheEntryUpdateArguments args)
    {
      CacheData();
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

    private static void CacheData()
    {
      var years = KbbService
        .GetYearList().Years
        .Select(y => new Year
        {
          Key = Int32.Parse(y.ModelYear),
          Value = y.ModelYear
        })
        .ToList();

      lock (Mutex)
      {
        _years = years;
      }


      var makesByYearDict = new Dictionary<int, IEnumerable<Make>>();
      foreach (var year in years)
      {
        var makes = KbbService
          .GetMakeByYear(year.Key)
          // This is some stupid crap we have to do because of the stinking SOAP service!
          .Years.First().Make 
          .Select(m => new Make
          {
            Key = m.Description,
            Value = m.Description
          })
          .ToList();

        makesByYearDict.Add(year.Key, makes);
      }

      lock (Mutex)
      {
        _makesByYearDict = makesByYearDict;
      }
    }
    
    #endregion
    
    #endregion



    #region Local Memory Cache

    private static ILocalCache LocalCache
    {
      get { return ServiceLocator.Get<ILocalCache>(); }
    }

    #endregion
  }
}

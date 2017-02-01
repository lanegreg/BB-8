using Car.Com.Domain.Models.Translators;
using Car.Com.Domain.Services;
using Car.Com.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Car.Com.Service.Data.Impl
{
  public sealed class UriTokenTranslators : ICacheable
  {
    #region Declarations

    private const int RefreshIntervalInMins = 10;
    private static readonly object Mutex = new object();
    private static readonly MemoryCache Cache = MemoryCache.Default;
    private static IEnumerable<MakeTranslator> _makeTranslators;
    private static IEnumerable<ModelTranslator> _modelTranslators;
    private static IEnumerable<SuperModelTranslator> _newSuperModelTranslators;
    private static IEnumerable<SuperModelTranslator> _usedSuperModelTranslators;
    private static IEnumerable<YearTranslator> _yearTranslators;
    private static IEnumerable<YearTranslator> _researchYearTranslators;
    private static IEnumerable<TrimTranslator> _trimTranslators;
    private static IEnumerable<CategoryTranslator> _categoryTranslators;
    private static IEnumerable<VehicleAttributeTranslator> _vehicleAttributeTranslators;

    #endregion


    #region Interface Impls

    public void Warm()
    {
      CacheAllServiceFetchedTranslators();
      Cache.Set("UriTokenTranslators", String.Empty, GetCachePolicy());
    }

    #endregion


    #region Public Static Methods

    public static IEnumerable<IMakeTranslator> GetAllMakeTranslators()
    {
      return _makeTranslators;
    }
    
    public static IEnumerable<IYearTranslator> GetAllYearTranslators()
    {
      return _yearTranslators;
    }

    public static IEnumerable<IYearTranslator> GetAllResearchYearTranslators()
    {
      return _researchYearTranslators;
    }

    public static IEnumerable<ITrimTranslator> GetAllTrimTranslators()
    {
      return _trimTranslators;
    }

    public static IEnumerable<IVehicleAttributeTranslator> GetAllVehicleAttributeTranslators()
    {
      return _vehicleAttributeTranslators;
    }

    public static IEnumerable<ICategoryTranslator> GetAllCategoryTranslators()
    {
      return _categoryTranslators;
    }

    public static IEnumerable<ISuperModelTranslator> GetAllNewSuperModelTranslators()
    {
      return _newSuperModelTranslators;
    }

    public static IEnumerable<ISuperModelTranslator> GetAllNotNewSuperModelTranslators()
    {
      return _usedSuperModelTranslators;
    }


    public static IMakeTranslator GetMakeTranslatorBySeoName(string seoName)
    {
      return _makeTranslators
        .FirstOrDefault(make => make.SeoName.Equals(seoName, StringComparison.CurrentCultureIgnoreCase));
    }

    public static IMakeTranslator GetMakeTranslatorById(int id)
    {
      return _makeTranslators
        .FirstOrDefault(make => make.Id.Equals(id));
    }

    public static IMakeTranslator GetMakeTranslatorByName(string name)
    {
      return _makeTranslators
        .FirstOrDefault(make => make.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }


    public static IModelTranslator GetModelTranslatorBySeoName(string seoName)
    {
      return _modelTranslators
        .FirstOrDefault(make => make.SeoName.Equals(seoName, StringComparison.CurrentCultureIgnoreCase));
    }

    public static IModelTranslator GetModelTranslatorByName(string name)
    {
      return _modelTranslators
        .FirstOrDefault(make => make.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }


    public static ICategoryTranslator GetCategoryTranslatorBySeoName(string seoName)
    {
      return _categoryTranslators
        .FirstOrDefault(category => category.SeoName.Equals(seoName, StringComparison.CurrentCultureIgnoreCase));
    }

    public static ICategoryTranslator GetCategoryTranslatorByName(string name)
    {
      return _categoryTranslators
        .FirstOrDefault(category => category.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }

    public static ICategoryTranslator GetCategoryTranslatorById(int id)
    {
      return _categoryTranslators
        .FirstOrDefault(category => category.Id.Equals(id));
    }
    
        
    public static ITrimTranslator GetTrimTranslatorBySeoName(string seoName)
    {
      return _trimTranslators
        .FirstOrDefault(t => t.SeoName.Equals(seoName, StringComparison.CurrentCultureIgnoreCase));
    }

    public static ITrimTranslator GetTrimTranslatorByName(string name)
    {
      return _trimTranslators
        .FirstOrDefault(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }


    public static IYearTranslator GetYearTranslatorByNumber(int yearNumber)
    {
      return _yearTranslators.FirstOrDefault(y => y.Number == yearNumber);
    }

    public static ISuperModelTranslator GetNewSuperModelTranslatorByName(string name)
    {
      return _newSuperModelTranslators
        .FirstOrDefault(sm => sm.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }

    public static ISuperModelTranslator GetNewSuperModelTranslatorBySeoName(string seoName)
    {
      return _newSuperModelTranslators
        .FirstOrDefault(sm => sm.SeoName.Equals(seoName, StringComparison.CurrentCultureIgnoreCase));
    }

    public static ISuperModelTranslator GetNotNewSuperModelTranslatorBySeoName(string seoName)
    {
      return _usedSuperModelTranslators
        .FirstOrDefault(sm => sm.SeoName.Equals(seoName, StringComparison.CurrentCultureIgnoreCase));
    }

    public static ISuperModelTranslator GetSuperModelTranslatorBySeoName(string seoName)
    {
      return _newSuperModelTranslators.Concat(_usedSuperModelTranslators)
        .FirstOrDefault(sm => sm.SeoName.Equals(seoName, StringComparison.CurrentCultureIgnoreCase));
    }

    public static IVehicleAttributeTranslator GetVehicleAttributeTranslatorBySeoName(string seoName)
    {
      return _vehicleAttributeTranslators
        .FirstOrDefault(va => va.SeoName.Equals(seoName, StringComparison.CurrentCultureIgnoreCase));
    }

    #endregion


    #region Private Static Methods

    private static void CacheUpdateHandler(CacheEntryUpdateArguments args)
    {
      CacheAllServiceFetchedTranslators();
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

    private static void CacheAllServiceFetchedTranslators()
    {
      var vehicleSpecService = ServiceLocator.Get<IVehicleSpecService>();


      // re-cache MakeTranslators
      var makes = vehicleSpecService
        .GetAllMakes()
        .Select(mk => new MakeTranslator
        {
          Id = mk.Id,
          Name = mk.Name,
          SeoName = mk.SeoName,
          PluralName = mk.PluralName,
          AbtMakeId = mk.AbtMakeId,
          IsActive = mk.IsActive
        });

      lock (Mutex)
      {
        _makeTranslators = makes;
      }


      // re-cache *UNIQUE* ModelTranslators
      var models = vehicleSpecService
        .GetAllModels()
        .GroupBy(m => new { m.Name, m.SeoName })
        .Select(model => new ModelTranslator
        {
          Name = model.Key.Name,
          SeoName = model.Key.SeoName
        });

      lock (Mutex)
      {
        _modelTranslators = models;
      }


      // re-cache YearTranslators
      var years = vehicleSpecService
        .GetAllYears()
        .Select(y => new YearTranslator
        {
          Number = y.Number
        });

      lock (Mutex)
      {
        _yearTranslators = years;
      }


      // re-cache Research YearTranslators
      var researchYears = vehicleSpecService
        .GetAllResearchYears()
        .Select(y => new YearTranslator
        {
          Number = y.Number
        });

      lock (Mutex)
      {
        _researchYearTranslators = researchYears;
      }


      // re-cache CategoryTranslators
      var categories = vehicleSpecService
        .GetAllCategories()
        .Select(c => new CategoryTranslator
        {
          Id = c.Id,
          Name = c.Name,
          SeoName = c.SeoName,
          PluralName = c.PluralName
        });

      lock (Mutex)
      {
        _categoryTranslators = categories;
      }


      // re-cache *NEW* SuperModelTranslators
      var newSuperModels = vehicleSpecService
        .GetAllNewSuperModels()
        .Select(sm => new SuperModelTranslator
        {
          Make = sm.Make,
          Name = sm.Name,
          SeoName = sm.SeoName
        });

      lock (Mutex)
      {
        _newSuperModelTranslators = newSuperModels;
      }


      // re-cache *USED* SuperModelTranslators
      var usedSuperModels = vehicleSpecService
        .GetAllUsedSuperModels()
        .Select(sm => new SuperModelTranslator
        {
          Make = sm.Make,
          Name = sm.Name,
          SeoName = sm.SeoName
        });

      lock (Mutex)
      {
        _usedSuperModelTranslators = usedSuperModels;
      }


      // re-cache *NEW* TrimTranslators
      var trims = vehicleSpecService
        .GetAllUniqueTrims()
        .Select(t => new TrimTranslator
        {
          Name = t.Name,
          SeoName = t.SeoName
        });

      lock (Mutex)
      {
        _trimTranslators = trims;
      }

      // re-cache VehicleAttributeTranslators
      var vehicleAttributes = vehicleSpecService
        .GetAllVehicleAttributeNames()
        .Select(va => new VehicleAttributeTranslator
        {
          Name = va.Name,
          SeoName = va.SeoName
        });

      lock (Mutex)
      {
        _vehicleAttributeTranslators = vehicleAttributes;
      }

    }

    #endregion
  }
}
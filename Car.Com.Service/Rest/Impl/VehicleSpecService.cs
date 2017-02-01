using Car.Com.Common;
using Car.Com.Common.Cache;
using Car.Com.Domain.Models.VehicleSpec;
using Car.Com.Domain.Services;
using Car.Com.Service.Rest.Common;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car.Com.Service.Rest.Impl
{
  public sealed class VehicleSpecService : RestServiceBase<ServiceJsonEnvelope>, IVehicleSpecService
  {
    #region Declarations

    private const int MinimumResearchYear = 2007;
    private const string VehicleSpecTierOneHashKey = "vsp_svc:t1_cache";
    private const string VehicleSpecTierTwoHashKey = "vsp_svc:t2_cache";

    private const string ActiveMakesFieldName = "makes:active";
    private const string MakesFieldName = "makes";
    private const string ModelsFieldName = "models";
    private const string YearsFieldName = "years";
    private const string CategoriesFieldName = "categories";
    private const string VehicleAttributesFieldName = "vehicle_attributes";
    private const string CategoriesAbtFieldName = "categories_abt";
    private const string NewSuperModelsFieldName = "super_models:new";
    private const string UsedSuperModelsFieldName = "super_models:used";
    private const string UniqueTrimsFieldName = "trims:unique";
    private const double LocalCacheTimeToLiveInSecs = 120; // 2 Minutes

    private static readonly ConnectionMultiplexer RedisReadable;
    private static readonly Uri Endpoint;
    private static readonly string PathPrefix;

    private static readonly int DatabaseNumber = WebConfig.Get<int>("VehicleSpecService:Redis:DatabaseNumber");

    #endregion

    
    #region ctors

    static VehicleSpecService()
    {
      RedisReadable = ConnectionMultiplexer.Connect(WebConfig.Get<string>("Redis:Readable:Config"));
      PathPrefix = String.Format("/api/v{0}/vehicle-spec", WebConfig.Get<string>("VehicleSpecService:ApiVersion"));
      Endpoint = new Uri(String.Format("http://{0}", WebConfig.Get<string>("VehicleSpecService:Endpoint")));
    }

    public VehicleSpecService()
      : base(WebConfig.Get<int>("VehicleSpecService:Timeout_ms"))
    {}

    #endregion

    
    #region Make Methods

    public async Task<IEnumerable<IMake>> GetAllActiveMakesAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = ActiveMakesFieldName;
      var makes = LocalCache.Get<ICollection<Make>>(localCacheKey);
      if (makes != null)
        return makes;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierOneHashKey, ActiveMakesFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/makes";
        makes = await FetchResource<List<Make>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, makes);
        return makes;
      }

      makes = JsonConvert.DeserializeObject<List<Make>>(json);
      LocalCache.Put(localCacheKey, makes);
      return makes;
    }

    public IEnumerable<IMake> GetAllActiveMakes()
    {
      return GetAllActiveMakesAsync().Result;
    }



    public async Task<IEnumerable<IMake>> GetAllMakesAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = MakesFieldName;
      var makes = LocalCache.Get<ICollection<Make>>(localCacheKey);
      if (makes != null)
        return makes;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierOneHashKey, MakesFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/makes?active=false";
        makes = await FetchResource<List<Make>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, makes);
        return makes;
      }

      makes = JsonConvert.DeserializeObject<List<Make>>(json);
      LocalCache.Put(localCacheKey, makes);
      return makes;
    }

    public IEnumerable<IMake> GetAllMakes()
    {
      return GetAllMakesAsync().Result;
    }

    #endregion


    #region Category Methods

    public async Task<IEnumerable<ICategory>> GetAllCategoriesAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = CategoriesFieldName;
      var categories = LocalCache.Get<ICollection<Category>>(localCacheKey);
      if (categories != null)
        return categories;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierOneHashKey, CategoriesFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/categories";
        categories = await FetchResource<List<Category>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, categories);
        return categories;
      }

      categories = JsonConvert.DeserializeObject<List<Category>>(json);
      LocalCache.Put(localCacheKey, categories);
      return categories;
    }

    public IEnumerable<ICategory> GetAllCategories()
    {
      return GetAllCategoriesAsync().Result;
    }



    public async Task<IEnumerable<ICategory>> GetAllAbtCategoriesAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = CategoriesAbtFieldName;
      var categories = LocalCache.Get<ICollection<Category>>(localCacheKey);
      if (categories != null)
        return categories;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierOneHashKey, CategoriesAbtFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/categories?schema=abt";
        categories = await FetchResource<List<Category>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, categories);
        return categories;
      }

      categories = JsonConvert.DeserializeObject<List<Category>>(json);
      LocalCache.Put(localCacheKey, categories);
      return categories;
    }

    public IEnumerable<ICategory> GetAllAbtCategories()
    {
      return GetAllAbtCategoriesAsync().Result;
    }




    #endregion


    #region Category (filters and vehicleattributes)

    public async Task<IEnumerable<ICategoryFilterGroup>> GetCategoryFilterGroupDataByCategoryAsync(string categorySeoName)
    {
      var cacheKey =
        String.Format("{0}:category:filtergroup:by_category[{1}]",
          VehicleSpecTierTwoHashKey, categorySeoName);

      // First, check LocalCache.
      var categoryFilterGroup = LocalCache.Get<ICollection<CategoryFilterGroup>>(cacheKey);
      if (categoryFilterGroup != null)
        return categoryFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/category/{0}/categoryfiltergroups", categorySeoName);

        categoryFilterGroup = await FetchResource<List<CategoryFilterGroup>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(cacheKey, categoryFilterGroup);
        return categoryFilterGroup;
      }

      categoryFilterGroup = JsonConvert.DeserializeObject<List<CategoryFilterGroup>>(json);
      LocalCache.Put(cacheKey, categoryFilterGroup);
      return categoryFilterGroup;
    }

    public IEnumerable<ICategoryFilterGroup> GetCategoryFilterGroupDataByCategory(string categorySeoName)
    {
      return GetCategoryFilterGroupDataByCategoryAsync(categorySeoName).Result;
    }


    public async Task<IEnumerable<ICategory>> GetCategoriesByVehicleAttributeSeoNameAsync(string vehicleAttributeSeoName)
    {
      var cacheKey =
        String.Format("{0}:category:by_vehicleattrname[{1}]",
          VehicleSpecTierTwoHashKey, vehicleAttributeSeoName);

      // First, check LocalCache.
      var categories = LocalCache.Get<ICollection<Category>>(cacheKey);
      if (categories != null)
        return categories;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/vehicleattribute/{0}/categories", vehicleAttributeSeoName);

        categories = await FetchResource<List<Category>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(cacheKey, categories);
        return categories;
      }

      categories = JsonConvert.DeserializeObject<List<Category>>(json);
      LocalCache.Put(cacheKey, categories);
      return categories;
    }

    public IEnumerable<ICategory> GetCategoriesByVehicleAttributeSeoName(string vehicleAttributeSeoName)
    {
      return GetCategoriesByVehicleAttributeSeoNameAsync(vehicleAttributeSeoName).Result;
    }


    public async Task<IEnumerable<ICategoryTrimFilterAttribute>> GetCategoryTrimFilterAttributesByCategoryAsync(string categorySeoName)
    {
      var cacheKey =
        String.Format("{0}:category:trimfilterattributes:by_category[{1}]",
          VehicleSpecTierTwoHashKey, categorySeoName);

      // First, check LocalCache.
      var categoryFilterGroup = LocalCache.Get<ICollection<CategoryTrimFilterAttribute>>(cacheKey);
      if (categoryFilterGroup != null)
        return categoryFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/category/{0}/categorytrimfilterattributes", categorySeoName);

        categoryFilterGroup = await FetchResource<List<CategoryTrimFilterAttribute>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, categoryFilterGroup);
        return categoryFilterGroup;
      }

      categoryFilterGroup = JsonConvert.DeserializeObject<List<CategoryTrimFilterAttribute>>(json);
      LocalCache.Put(cacheKey, categoryFilterGroup);
      return categoryFilterGroup;
    }

    public IEnumerable<ICategoryTrimFilterAttribute> GetCategoryTrimFilterAttributesByCategory(string categorySeoName)
    {
      return GetCategoryTrimFilterAttributesByCategoryAsync(categorySeoName).Result;
    }


    public async Task<IEnumerable<ICategoryFilterGroup>> GetCustomFilterGroupDataByVehicleAttributeNameAsync(string vehicleAttributeSeoName)
    {
      var cacheKey =
        String.Format("{0}:category:filtergroup:by_vehicleattrname[{1}]",
          VehicleSpecTierTwoHashKey, vehicleAttributeSeoName);

      // First, check LocalCache.
      var categoryFilterGroup = LocalCache.Get<ICollection<CategoryFilterGroup>>(cacheKey);
      if (categoryFilterGroup != null)
        return categoryFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("{0}/categoryfiltergroups", vehicleAttributeSeoName);

        categoryFilterGroup = await FetchResource<List<CategoryFilterGroup>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, categoryFilterGroup);
        return categoryFilterGroup;
      }

      categoryFilterGroup = JsonConvert.DeserializeObject<List<CategoryFilterGroup>>(json);
      LocalCache.Put(cacheKey, categoryFilterGroup);
      return categoryFilterGroup;
    }

    public IEnumerable<ICategoryFilterGroup> GetCustomFilterGroupDataByVehicleAttributeName(string vehicleAttributeSeoName)
    {
      return GetCustomFilterGroupDataByVehicleAttributeNameAsync(vehicleAttributeSeoName).Result;
    }


    public async Task<IEnumerable<ICategoryFilterGroup>> GetCustomFilterGroupDataByCategoryAndVehicleAttributeNameAsync(string categorySeoName, string vehicleAttributeSeoName)
    {
      var cacheKey =
        String.Format("{0}:category:filtergroup:by_category[{1}]:by_vehicleattrname[{2}]",
          VehicleSpecTierTwoHashKey, categorySeoName, vehicleAttributeSeoName);

      // First, check LocalCache.
      var categoryFilterGroup = LocalCache.Get<ICollection<CategoryFilterGroup>>(cacheKey);
      if (categoryFilterGroup != null)
        return categoryFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("{0}/category/{1}/categoryfiltergroups", vehicleAttributeSeoName, categorySeoName);

        categoryFilterGroup = await FetchResource<List<CategoryFilterGroup>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, categoryFilterGroup);
        return categoryFilterGroup;
      }

      categoryFilterGroup = JsonConvert.DeserializeObject<List<CategoryFilterGroup>>(json);
      LocalCache.Put(cacheKey, categoryFilterGroup);
      return categoryFilterGroup;
    }

    public IEnumerable<ICategoryFilterGroup> GetCustomFilterGroupDataByCategoryAndVehicleAttributeName(string categorySeoName, string vehicleAttributeSeoName)
    {
      return GetCustomFilterGroupDataByCategoryAndVehicleAttributeNameAsync(categorySeoName, vehicleAttributeSeoName).Result;
    }


    public async Task<IEnumerable<ICategoryTrimFilterAttribute>> GetCustomTrimFilterAttributesByVehicleAttributeNameAsync(string vehicleAttributeSeoName)
    {
      var cacheKey =
        String.Format("{0}:category:trimfilterattributes:by_vehicleattrname[{1}]",
          VehicleSpecTierTwoHashKey, vehicleAttributeSeoName);

      // First, check LocalCache.
      var categoryFilterGroup = LocalCache.Get<ICollection<CategoryTrimFilterAttribute>>(cacheKey);
      if (categoryFilterGroup != null)
        return categoryFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("{0}/categorytrimfilterattributes", vehicleAttributeSeoName);

        categoryFilterGroup = await FetchResource<List<CategoryTrimFilterAttribute>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, categoryFilterGroup);
        return categoryFilterGroup;
      }

      categoryFilterGroup = JsonConvert.DeserializeObject<List<CategoryTrimFilterAttribute>>(json);
      LocalCache.Put(cacheKey, categoryFilterGroup);
      return categoryFilterGroup;
    }

    public IEnumerable<ICategoryTrimFilterAttribute> GetCustomTrimFilterAttributesByVehicleAttributeName(string vehicleAttributeSeoName)
    {
      return GetCustomTrimFilterAttributesByVehicleAttributeNameAsync(vehicleAttributeSeoName).Result;
    }
    

    public async Task<IEnumerable<ICategoryTrimFilterAttribute>> GetCustomTrimFilterAttributesByCategoryAndVehicleAttributeNameAsync(string categorySeoName, string vehicleAttributeSeoName)
    {
      var cacheKey =
        String.Format("{0}:category:trimfilterattributes:by_category[{1}]:by_vehicleattrname[{2}]",
          VehicleSpecTierTwoHashKey, categorySeoName, vehicleAttributeSeoName);

      // First, check LocalCache.
      var categoryFilterGroup = LocalCache.Get<ICollection<CategoryTrimFilterAttribute>>(cacheKey);
      if (categoryFilterGroup != null)
        return categoryFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("{0}/category/{1}/categorytrimfilterattributes", vehicleAttributeSeoName, categorySeoName);

        categoryFilterGroup = await FetchResource<List<CategoryTrimFilterAttribute>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, categoryFilterGroup);
        return categoryFilterGroup;
      }

      categoryFilterGroup = JsonConvert.DeserializeObject<List<CategoryTrimFilterAttribute>>(json);
      LocalCache.Put(cacheKey, categoryFilterGroup);
      return categoryFilterGroup;
    }

    public IEnumerable<ICategoryTrimFilterAttribute> GetCustomTrimFilterAttributesByCategoryAndVehicleAttributeName(string categorySeoName, string vehicleAttributeSeoName)
    {
      return GetCustomTrimFilterAttributesByCategoryAndVehicleAttributeNameAsync(categorySeoName, vehicleAttributeSeoName).Result;
    }

    
    public async Task<IEnumerable<IVehicleAttribute>> GetAllVehicleAttributeNamesAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = VehicleAttributesFieldName;
      var vehicleattributes = LocalCache.Get<ICollection<VehicleAttribute>>(localCacheKey);
      if (vehicleattributes != null)
        return vehicleattributes;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierOneHashKey, VehicleAttributesFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/vehicle_attributes";
        vehicleattributes = await FetchResource<List<VehicleAttribute>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, vehicleattributes);
        return vehicleattributes;
      }

      vehicleattributes = JsonConvert.DeserializeObject<List<VehicleAttribute>>(json);
      LocalCache.Put(localCacheKey, vehicleattributes);
      return vehicleattributes;
    }

    public IEnumerable<IVehicleAttribute> GetAllVehicleAttributeNames()
    {
      return GetAllVehicleAttributeNamesAsync().Result;
    }


    public async Task<IEnumerable<IVehicleAttribute>> GetVehicleAttributesForAltFuelTrimsAsync()
    {
      var cacheKey =
        String.Format("{0}:vehicleattributes:for_altfueltrims", VehicleSpecTierTwoHashKey);

      // First, check LocalCache.
      var vehicleAttributes = LocalCache.Get<ICollection<VehicleAttribute>>(cacheKey);
      if (vehicleAttributes != null)
        return vehicleAttributes;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = "vehicleattribute/altfueltrims";

        vehicleAttributes = await FetchResource<List<VehicleAttribute>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, vehicleAttributes);
        return vehicleAttributes;
      }

      vehicleAttributes = JsonConvert.DeserializeObject<List<VehicleAttribute>>(json);
      LocalCache.Put(cacheKey, vehicleAttributes);
      return vehicleAttributes;
    }

    public IEnumerable<IVehicleAttribute> GetVehicleAttributesForAltFuelTrims()
    {
      return GetVehicleAttributesForAltFuelTrimsAsync().Result;
    }
    
    #endregion


    #region Year Methods

    public async Task<IEnumerable<Year>> GetAllYearsAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = YearsFieldName;
      var years = LocalCache.Get<ICollection<Year>>(localCacheKey);
      if (years != null)
        return years;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierOneHashKey, YearsFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/years";
        years = await FetchResource<List<Year>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, years);
        return years;
      }

      years = JsonConvert.DeserializeObject<List<Year>>(json);
      LocalCache.Put(localCacheKey, years);
      return years;
    }

    public IEnumerable<Year> GetAllYears()
    {
      return GetAllYearsAsync().Result;
    }



    public async Task<IEnumerable<Year>> GetAllResearchYearsAsync()
    {
      return (await GetAllYearsAsync()
        .ConfigureAwait(false))
        .Where(y => y.Number >= MinimumResearchYear);
    }

    public IEnumerable<Year> GetAllResearchYears()
    {
      return GetAllResearchYearsAsync().Result;
    }

    #endregion


    #region SuperModel Methods

    public async Task<IEnumerable<ISuperModel>> GetAllNewSuperModelsAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = NewSuperModelsFieldName;
      var superModels = LocalCache.Get<ICollection<SuperModel>>(localCacheKey);
      if (superModels != null)
        return superModels;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierOneHashKey, NewSuperModelsFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/super-models?new=true";
        superModels = await FetchResource<List<SuperModel>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, superModels);
        return superModels;
      }

      superModels = JsonConvert.DeserializeObject<List<SuperModel>>(json);
      LocalCache.Put(localCacheKey, superModels);
      return superModels;
    }

    public IEnumerable<ISuperModel> GetAllNewSuperModels()
    {
      return GetAllNewSuperModelsAsync().Result;
    }



    public async Task<IEnumerable<ISuperModel>> GetAllUsedSuperModelsAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = UsedSuperModelsFieldName;
      var superModels = LocalCache.Get<ICollection<SuperModel>>(localCacheKey);
      if (superModels != null)
        return superModels;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierOneHashKey, UsedSuperModelsFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/super-models?new=false";
        superModels = await FetchResource<List<SuperModel>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, superModels);
        return superModels;
      }

      superModels = JsonConvert.DeserializeObject<List<SuperModel>>(json);
      LocalCache.Put(localCacheKey, superModels);
      return superModels;
    }

    public IEnumerable<ISuperModel> GetAllUsedSuperModels()
    {
      return GetAllUsedSuperModelsAsync().Result;
    }



    public async Task<IEnumerable<ISuperModel>> GetNewSuperModelsByMakeAsync(string makeSeoName)
    {
      var localCacheKey =
        String.Format("{0}:super_models:new:by_make[{1}]",
          VehicleSpecTierTwoHashKey, makeSeoName);

      // First, check LocalCache.
      var superModels = LocalCache.Get<ICollection<SuperModel>>(localCacheKey);
      if (superModels != null)
        return superModels;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .StringGetAsync(localCacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/make/{0}/super-models", makeSeoName);
        superModels = await FetchResource<List<SuperModel>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, superModels);
        return superModels;
      }

      superModels = JsonConvert.DeserializeObject<List<SuperModel>>(json);
      LocalCache.Put(localCacheKey, superModels);
      return superModels;
    }

    public IEnumerable<ISuperModel> GetNewSuperModelsByMake(string makeSeoName)
    {
      return GetNewSuperModelsByMakeAsync(makeSeoName).Result;
    }

    #endregion


    #region SuperModel (filters)
    
    public async Task<IEnumerable<ISuperModelFilterGroup>> GetSuperModelFilterGroupDataByMakeSuperModelAsync
      (string makeSeoName, string superModelSeoName)
    {
      var cacheKey =
        String.Format("{0}:supermodel:filtergroup:by_make[{1}]:by_super_model[{2}]",
          VehicleSpecTierTwoHashKey, makeSeoName, superModelSeoName);

      // First, check LocalCache.
      var superModelFilterGroup = LocalCache.Get<ICollection<SuperModelFilterGroup>>(cacheKey);
      if (superModelFilterGroup != null)
        return superModelFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path =
          String.Format("/make/{0}/super-model/{1}/supermodelfiltergroups",
            makeSeoName, superModelSeoName);

        superModelFilterGroup = await FetchResource<List<SuperModelFilterGroup>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, superModelFilterGroup);
        return superModelFilterGroup;
      }

      superModelFilterGroup = JsonConvert.DeserializeObject<List<SuperModelFilterGroup>>(json);
      LocalCache.Put(cacheKey, superModelFilterGroup);
      return superModelFilterGroup;
    }

    public IEnumerable<ISuperModelFilterGroup> GetSuperModelFilterGroupDataByMakeSuperModel
      (string makeSeoName, string superModelSeoName)
    {
      return GetSuperModelFilterGroupDataByMakeSuperModelAsync(makeSeoName, superModelSeoName).Result;
    }



    public async Task<IEnumerable<ISuperModelFilterGroup>> GetSuperModelFilterGroupDataByMakeSuperModelByYearAsync
      (string makeSeoName, string superModelSeoName, int yearNumber)
    {
      var cacheKey =
        String.Format("{0}:supermodel:filtergroup:by_make[{1}]:by_super_model[{2}]:by_year[{3}]",
          VehicleSpecTierTwoHashKey, makeSeoName, superModelSeoName, yearNumber);

      // First, check LocalCache.
      var superModelFilterGroup = LocalCache.Get<ICollection<SuperModelFilterGroup>>(cacheKey);
      if (superModelFilterGroup != null)
        return superModelFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path =
          String.Format("/make/{0}/super-model/{1}/year/{2}/supermodelfiltergroups",
            makeSeoName, superModelSeoName, yearNumber);

        superModelFilterGroup = await FetchResource<List<SuperModelFilterGroup>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, superModelFilterGroup);
        return superModelFilterGroup;
      }

      superModelFilterGroup = JsonConvert.DeserializeObject<List<SuperModelFilterGroup>>(json);
      LocalCache.Put(cacheKey, superModelFilterGroup);
      return superModelFilterGroup;
    }

    public IEnumerable<ISuperModelFilterGroup> GetSuperModelFilterGroupDataByMakeSuperModelByYear
      (string makeSeoName, string superModelSeoName, int yearNumber)
    {
      return GetSuperModelFilterGroupDataByMakeSuperModelByYearAsync(makeSeoName, superModelSeoName, yearNumber).Result;
    }


    public async Task<IEnumerable<ISuperModelTrimFilterAttribute>>GetSuperModelTrimFilterAttributesByMakeSuperModelAsync
      (string makeSeoName, string superModelSeoName)
    {
      var cacheKey =
        String.Format("{0}:supermodel:trimfilterattributes:by_make[{1}]:by_super_model[{2}]",
          VehicleSpecTierTwoHashKey, makeSeoName, superModelSeoName);

      // First, check LocalCache.
      var superModelFilterGroup = LocalCache.Get<ICollection<SuperModelTrimFilterAttribute>>(cacheKey);
      if (superModelFilterGroup != null)
        return superModelFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path =
          String.Format("/make/{0}/super-model/{1}/supermodeltrimfilterattributes",
            makeSeoName, superModelSeoName);

        superModelFilterGroup = await FetchResource<List<SuperModelTrimFilterAttribute>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, superModelFilterGroup);
        return superModelFilterGroup;
      }

      superModelFilterGroup = JsonConvert.DeserializeObject<List<SuperModelTrimFilterAttribute>>(json);
      LocalCache.Put(cacheKey, superModelFilterGroup);
      return superModelFilterGroup;
    }

    public IEnumerable<ISuperModelTrimFilterAttribute> GetSuperModelTrimFilterAttributesByMakeSuperModel
      (string makeSeoName, string superModelSeoName)
    {
      return GetSuperModelTrimFilterAttributesByMakeSuperModelAsync(makeSeoName, superModelSeoName).Result;
    }



    public async Task<IEnumerable<ISuperModelTrimFilterAttribute>> GetSuperModelTrimFilterAttributesByMakeSuperModelByYearAsync
      (string makeSeoName, string superModelSeoName, int yearNumber)
    {
      var cacheKey =
        String.Format("{0}:supermodel:trimfilterattributes:by_make[{1}]:by_super_model[{2}]:by_year[{3}]",
          VehicleSpecTierTwoHashKey, makeSeoName, superModelSeoName, yearNumber);

      // First, check LocalCache.
      var superModelFilterGroup = LocalCache.Get<ICollection<SuperModelTrimFilterAttribute>>(cacheKey);
      if (superModelFilterGroup != null)
        return superModelFilterGroup;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path =
          String.Format("/make/{0}/super-model/{1}/year/{2}/supermodeltrimfilterattributes",
            makeSeoName, superModelSeoName, yearNumber);

        superModelFilterGroup = await FetchResource<List<SuperModelTrimFilterAttribute>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, superModelFilterGroup);
        return superModelFilterGroup;
      }

      superModelFilterGroup = JsonConvert.DeserializeObject<List<SuperModelTrimFilterAttribute>>(json);
      LocalCache.Put(cacheKey, superModelFilterGroup);
      return superModelFilterGroup;
    }
    
    public IEnumerable<ISuperModelTrimFilterAttribute> GetSuperModelTrimFilterAttributesByMakeSuperModelByYear
      (string makeSeoName, string superModelSeoName, int yearNumber)
    {
      return GetSuperModelTrimFilterAttributesByMakeSuperModelByYearAsync(makeSeoName, superModelSeoName, yearNumber).Result;
    }

    #endregion


    #region Trim Methods

    public async Task<IEnumerable<IUniqueTrim>> GetAllUniqueTrimsAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = UniqueTrimsFieldName;
      var trims = LocalCache.Get<ICollection<UniqueTrim>>(localCacheKey);
      if (trims != null)
        return trims;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierOneHashKey, UniqueTrimsFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/trims/unique";
        trims = await FetchResource<List<UniqueTrim>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, trims);
        return trims;
      }

      trims = JsonConvert.DeserializeObject<List<UniqueTrim>>(json);
      LocalCache.Put(localCacheKey, trims);
      return trims;
    }

    public IEnumerable<IUniqueTrim> GetAllUniqueTrims()
    {
      return GetAllUniqueTrimsAsync().Result;
    }



    public async Task<IEnumerable<ITrim>> GetTrimsByCategoryAsync(string categorySeoName)
    {
      var localCacheKey =
        String.Format("{0}:trims:by_category[{1}]",
          VehicleSpecTierTwoHashKey, categorySeoName);

      // First, check LocalCache.
      var trims = LocalCache.Get<ICollection<Trim>>(localCacheKey);
      if (trims != null)
        return trims;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .StringGetAsync(localCacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/category/{0}/trims", categorySeoName);
        trims = await FetchResource<List<Trim>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, trims);
        return trims;
      }

      trims = JsonConvert.DeserializeObject<List<Trim>>(json);
      LocalCache.Put(localCacheKey, trims);
      return trims;
    }

    public IEnumerable<ITrim> GetTrimsByCategory(string categorySeoName)
    {
      return GetTrimsByCategoryAsync(categorySeoName).Result;
    }


    public async Task<IEnumerable<ITrim>> GetTrimsByVehicleAttributeNameAsync(string vehicleAttributeSeoName)
    {
      var localCacheKey =
        String.Format("{0}:trims:by_vehicleattrname[{1}]",
          VehicleSpecTierTwoHashKey, vehicleAttributeSeoName);

      // First, check LocalCache.
      var trims = LocalCache.Get<ICollection<Trim>>(localCacheKey);
      if (trims != null)
        return trims;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .StringGetAsync(localCacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("{0}/trims", vehicleAttributeSeoName);
        trims = await FetchResource<List<Trim>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, trims);
        return trims;
      }

      trims = JsonConvert.DeserializeObject<List<Trim>>(json);
      LocalCache.Put(localCacheKey, trims);
      return trims;
    }

    public IEnumerable<ITrim> GetTrimsByVehicleAttributeName(string vehicleAttributeSeoName)
    {
      return GetTrimsByVehicleAttributeNameAsync(vehicleAttributeSeoName).Result;
    }


    public async Task<IEnumerable<ITrim>> GetTrimsByCategoryAndVehicleAttributeNameAsync(string categorySeoName, string vehicleAttributeSeoName)
    {
      var localCacheKey =
        String.Format("{0}:trims:by_category[{1}]:by_vehicleattrname[{2}]",
          VehicleSpecTierTwoHashKey, categorySeoName, vehicleAttributeSeoName);

      // First, check LocalCache.
      var trims = LocalCache.Get<ICollection<Trim>>(localCacheKey);
      if (trims != null)
        return trims;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .StringGetAsync(localCacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("{0}/category/{1}/trims", vehicleAttributeSeoName, categorySeoName);
        trims = await FetchResource<List<Trim>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, trims);
        return trims;
      }

      trims = JsonConvert.DeserializeObject<List<Trim>>(json);
      LocalCache.Put(localCacheKey, trims);
      return trims;
    }

    public IEnumerable<ITrim> GetTrimsByCategoryAndVehicleAttributeName(string categorySeoName, string vehicleAttributeSeoName)
    {
      return GetTrimsByCategoryAndVehicleAttributeNameAsync(categorySeoName, vehicleAttributeSeoName).Result;
    }



    public async Task<IEnumerable<ITrim>> GetNewTrimsByMakeBySuperModelAsync(string makeSeoName, string superModelSeoName)
    {
      var localCacheKey =
        String.Format("{0}:trims:new:by_make[{1}]:by_super_model[{2}]",
          VehicleSpecTierTwoHashKey, makeSeoName, superModelSeoName);

      // First, check LocalCache.
      var trims = LocalCache.Get<ICollection<Trim>>(localCacheKey);
      if (trims != null)
        return trims;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, localCacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path =
          String.Format("/make/{0}/super-model/{1}/trims",
            makeSeoName, superModelSeoName);

        trims = await FetchResource<List<Trim>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, trims);
        return trims;
      }

      trims = JsonConvert.DeserializeObject<List<Trim>>(json);
      LocalCache.Put(localCacheKey, trims);
      return trims;
    }

    public IEnumerable<ITrim> GetNewTrimsByMakeBySuperModel(string makeSeoName, string superModelSeoName)
    {
      return GetNewTrimsByMakeBySuperModelAsync(makeSeoName, superModelSeoName).Result;
    }



    public async Task<IEnumerable<ITrim>> GetTrimsByMakeBySuperModelByYearAsync(string makeSeoName, string superModelSeoName, int yearNumber)
    {
      var localCacheKey =
        String.Format("{0}:trims:by_make[{1}]:by_super_model[{2}]:by_year[{3}]",
          VehicleSpecTierTwoHashKey, makeSeoName, superModelSeoName, yearNumber);

      // First, check LocalCache.
      var trims = LocalCache.Get<ICollection<Trim>>(localCacheKey);
      if (trims != null)
        return trims;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, localCacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path =
          String.Format("/make/{0}/super-model/{1}/year/{2}/trims",
            makeSeoName, superModelSeoName, yearNumber);

        trims = await FetchResource<List<Trim>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, trims);
        return trims;
      }

      trims = JsonConvert.DeserializeObject<List<Trim>>(json);
      LocalCache.Put(localCacheKey, trims);
      return trims;
    }

    public IEnumerable<ITrim> GetTrimsByMakeBySuperModelByYear(string makeSeoName, string superModelSeoName, int yearNumber)
    {
      return GetTrimsByMakeBySuperModelByYearAsync(makeSeoName, superModelSeoName, yearNumber).Result;
    }

    public async Task<IEnumerable<ITrim>> GetSimilarTrimsByTrimIdAsync(int trimId)
    {
      var localCacheKey = String.Format("similartrims:by_trimid[{0}]", trimId);

      // First, check LocalCache.
      var trims = LocalCache.Get<ICollection<Trim>>(localCacheKey);
      if (trims != null)
        return trims;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, localCacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/trimid/{0}/similartrims", trimId);

        trims = await FetchResource<List<Trim>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, trims);
        return trims;
      }

      trims = JsonConvert.DeserializeObject<List<Trim>>(json);
      LocalCache.Put(localCacheKey, trims);
      return trims;
    }

    public IEnumerable<ITrim> GetSimilarTrimsByTrimId(int trimId)
    {
      return GetSimilarTrimsByTrimIdAsync(trimId).Result;
    }

    public async Task<IEnumerable<ITrim>> GetSimilarTrimsByPriceAsync(int price)
    {
      var localCacheKey = String.Format("similartrims:by_price[{0}]", price);

      // First, check LocalCache.
      var trims = LocalCache.Get<ICollection<Trim>>(localCacheKey);
      if (trims != null)
        return trims;

      // Then, check Redis or the VehicleSpec service.
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleSpecTierTwoHashKey, localCacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty) 
      {
        var path = String.Format("/price/{0}/similartrims", price);

        trims = await FetchResource<List<Trim>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, trims);
        return trims;
      }

      trims = JsonConvert.DeserializeObject<List<Trim>>(json);
      LocalCache.Put(localCacheKey, trims);
      return trims;
    }

    public IEnumerable<ITrim> GetSimilarTrimsByPrice(int price)
    {
      return GetSimilarTrimsByPriceAsync(price).Result;
    }

    #endregion


    #region Trim Section Methods

    public async Task<ITrim> GetAbtTrimsByAcodeAsync
      (string acode)
    {
      var localCacheKey =
        String.Format("abt_trim:by_acode[{0}]", acode);

      // First, check LocalCache.
      var trim = LocalCache.Get<Trim>(localCacheKey);
      if (trim != null)
        return trim;

      // Then, check Redis.
      var redisCacheKey = String.Format("{0}:{1}", VehicleSpecTierTwoHashKey, localCacheKey);
      var result = RedisReadable.GetDatabase(DatabaseNumber).StringGetAsync(redisCacheKey);
      var json = RedisReadable.Wait(result);
      if (!json.IsNullOrEmpty)
      {
        trim = JsonConvert.DeserializeObject<Trim>(json);
        LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);
        return trim;
      }

      var path =
        String.Format("/acode/{0}/trim", acode);

      trim = await FetchResource<Trim>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

      LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);

      return trim;
    }

    public ITrim GetAbtTrimsByAcode (string acode)
    {
      return GetAbtTrimsByAcodeAsync(acode).Result;
    }

    
    public async Task<ITrim> GetTrimOverviewByMakeBySuperModelByYearByTrimAsync
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      var localCacheKey =
        String.Format("trim_overview:by_make[{0}]:by_super_model[{1}]:by_year[{2}]:by_trim[{3}]",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      // First, check LocalCache.
      var trim = LocalCache.Get<Trim>(localCacheKey);
      if (trim != null)
        return trim;

      // Then, check Redis.
      var redisCacheKey = String.Format("{0}:{1}", VehicleSpecTierTwoHashKey, localCacheKey);
      var result = RedisReadable.GetDatabase(DatabaseNumber).StringGetAsync(redisCacheKey);
      var json = RedisReadable.Wait(result);
      if (!json.IsNullOrEmpty)
      {
        trim = JsonConvert.DeserializeObject<Trim>(json);
        LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);
        return trim;
      }

      var path =
        String.Format("/make/{0}/super-model/{1}/year/{2}/trim/{3}/trim-overview",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      trim = await FetchResource<Trim>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

      LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);

      return trim;
     }

    public ITrim GetTrimOverviewByMakeBySuperModelByYearByTrim
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      return GetTrimOverviewByMakeBySuperModelByYearByTrimAsync(makeSeoName, superModelSeoName, yearNumber, trimSeoName).Result;
    }

    

    public async Task<ITrim> GetTrimSpecsByMakeBySuperModelByYearByTrimAsync
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      var localCacheKey =
        String.Format("trim_specifications:by_make[{0}]:by_super_model[{1}]:by_year[{2}]:by_trim[{3}]",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      // First, check LocalCache.
      var trim = LocalCache.Get<Trim>(localCacheKey);
      if (trim != null)
        return trim;

      // Then, check Redis.
      var redisCacheKey = String.Format("{0}:{1}", VehicleSpecTierTwoHashKey, localCacheKey);
      var result = RedisReadable.GetDatabase(DatabaseNumber).StringGetAsync(redisCacheKey);
      var json = RedisReadable.Wait(result);
      if (!json.IsNullOrEmpty)
      {
        trim = JsonConvert.DeserializeObject<Trim>(json);
        LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);
        return trim;
      }

      var path =
        String.Format("/make/{0}/super-model/{1}/year/{2}/trim/{3}/trim-specifications",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      trim = await FetchResource<Trim>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

      LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);

      return trim;
    }

    public ITrim GetTrimSpecsByMakeBySuperModelByYearByTrim
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      return GetTrimSpecsByMakeBySuperModelByYearByTrimAsync(makeSeoName, superModelSeoName, yearNumber, trimSeoName).Result;
    }



    public async Task<ITrim> GetTrimSafetyByMakeBySuperModelByYearByTrimAsync
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      var localCacheKey =
        String.Format("trim_safety:by_make[{0}]:by_super_model[{1}]:by_year[{2}]:by_trim[{3}]",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      // First, check LocalCache.
      var trim = LocalCache.Get<Trim>(localCacheKey);
      if (trim != null)
        return trim;

      // Then, check Redis.
      var redisCacheKey = String.Format("{0}:{1}", VehicleSpecTierTwoHashKey, localCacheKey);
      var result = RedisReadable.GetDatabase(DatabaseNumber).StringGetAsync(redisCacheKey);
      var json = RedisReadable.Wait(result);
      if (!json.IsNullOrEmpty)
      {
        trim = JsonConvert.DeserializeObject<Trim>(json);
        LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);
        return trim;
      }

      var path =
        String.Format("/make/{0}/super-model/{1}/year/{2}/trim/{3}/safety",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      trim = await FetchResource<Trim>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

      LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);

      return trim;
    }

    public ITrim GetTrimSafetyByMakeBySuperModelByYearByTrim
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      return GetTrimSafetyByMakeBySuperModelByYearByTrimAsync(makeSeoName, superModelSeoName, yearNumber, trimSeoName).Result;
    }



    public async Task<ITrim> GetTrimColorByMakeBySuperModelByYearByTrimAsync
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      var localCacheKey =
        String.Format("trim_colors:by_make[{0}]:by_super_model[{1}]:by_year[{2}]:by_trim[{3}]",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      // First, check LocalCache.
      var trim = LocalCache.Get<Trim>(localCacheKey);
      if (trim != null)
        return trim;

      // Then, check Redis.
      var redisCacheKey = String.Format("{0}:{1}", VehicleSpecTierTwoHashKey, localCacheKey);
      var result = RedisReadable.GetDatabase(DatabaseNumber).StringGetAsync(redisCacheKey);
      var json = RedisReadable.Wait(result);
      if (!json.IsNullOrEmpty)
      {
        trim = JsonConvert.DeserializeObject<Trim>(json);
        LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);
        return trim;
      }

      var path =
        String.Format("/make/{0}/super-model/{1}/year/{2}/trim/{3}/color",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      trim = await FetchResource<Trim>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

      LocalCache.Put(localCacheKey, trim, LocalCacheTimeToLiveInSecs);

      return trim;
    }

    public ITrim GetTrimColorByMakeBySuperModelByYearByTrim
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      return GetTrimColorByMakeBySuperModelByYearByTrimAsync(makeSeoName, superModelSeoName, yearNumber, trimSeoName).Result;
    }
    


    public async Task<ITrim> GetTrimIncentivesByMakeBySuperModelByYearByTrimByZipAsync
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName, string zip)
    {
      var localCacheKey =
        String.Format("trim_incentives:by_make[{0}]:by_super_model[{1}]:by_year[{2}]:by_trim[{3}]:by_zip[{4}]",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName, zip);

      // First, check LocalCache.
      var incentives = LocalCache.Get<ITrim>(localCacheKey);
      if (incentives != null)
        return incentives;

      // Then, check Redis.
      var redisCacheKey = String.Format("{0}:{1}", VehicleSpecTierTwoHashKey, localCacheKey);
      var result = RedisReadable.GetDatabase(DatabaseNumber).StringGetAsync(redisCacheKey);
      var json = RedisReadable.Wait(result);
      if (!json.IsNullOrEmpty)
      {
        incentives = JsonConvert.DeserializeObject<Trim>(json);
        LocalCache.Put(localCacheKey, incentives, LocalCacheTimeToLiveInSecs);
        return incentives;
      }

      var path =
        String.Format("/make/{0}/super-model/{1}/year/{2}/trim/{3}/zip/{4}/incentive",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName, zip);

      incentives = await FetchResource<Trim>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

      LocalCache.Put(localCacheKey, incentives, LocalCacheTimeToLiveInSecs);

      return incentives;
    }

    public ITrim GetTrimIncentivesByMakeBySuperModelByYearByTrimByZip
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName, string zip)
    {
      return GetTrimIncentivesByMakeBySuperModelByYearByTrimByZipAsync(makeSeoName, superModelSeoName, yearNumber, trimSeoName, zip).Result;
    }



    public async Task<IEnumerable<ISpecification>> GetTrimSpecOptionsByMakeBySuperModelByYearByTrimAsync
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      var cacheKey =
        String.Format("{0}:trim:specoptions:by_make[{1}]:by_super_model[{2}]:by_year[{3}]:by_trim[{4}]",
          VehicleSpecTierTwoHashKey, makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      // First, check LocalCache.
      var specs = LocalCache.Get<ICollection<Specification>>(cacheKey);
      if (specs != null)
        return specs;

      // Then, check Redis.
      var result = RedisReadable.GetDatabase(DatabaseNumber).StringGetAsync(cacheKey);
      var json = RedisReadable.Wait(result);
      if (!json.IsNullOrEmpty)
      {
        specs = JsonConvert.DeserializeObject<List<Specification>>(json);
        LocalCache.Put(cacheKey, specs, LocalCacheTimeToLiveInSecs);
        return specs;
      }

      var path =
        String.Format("/make/{0}/super-model/{1}/year/{2}/trim/{3}/specoptions",
          makeSeoName, superModelSeoName, yearNumber, trimSeoName);

      specs = await FetchResource<List<Specification>>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

      LocalCache.Put(cacheKey, specs, LocalCacheTimeToLiveInSecs);

      return specs;
    }

    public IEnumerable<ISpecification> GetTrimSpecOptionsByMakeBySuperModelByYearByTrim
      (string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName)
    {
      return GetTrimSpecOptionsByMakeBySuperModelByYearByTrimAsync(makeSeoName, superModelSeoName, yearNumber, trimSeoName).Result;
    }



    public async Task<IEnumerable<TrimIncentive>> GetTrimIncentivesByTrimIdByZipAsync(int trimId, string zip)
    {
      var localCacheKey = String.Format("trim_incentives:by_trimid[{0}]:by_zip[{1}]", trimId, zip);

      // First, check LocalCache.
      var trimIncentive = LocalCache.Get<ICollection<TrimIncentive>>(localCacheKey);
      if (trimIncentive != null)
        return trimIncentive;

      // Then, check Redis.
      var redisCacheKey = String.Format("{0}:{1}", VehicleSpecTierTwoHashKey, localCacheKey);
      var result = RedisReadable.GetDatabase(DatabaseNumber).StringGetAsync(redisCacheKey);
      var json = RedisReadable.Wait(result);

      if (!json.IsNullOrEmpty)
      {
        trimIncentive = JsonConvert.DeserializeObject<List<TrimIncentive>>(json);
        LocalCache.Put(localCacheKey, trimIncentive, LocalCacheTimeToLiveInSecs);
        return trimIncentive;
      }

      var path = String.Format("/trimid/{0}/zip/{1}/incentive", trimId, zip);

      trimIncentive = await FetchResource<List<TrimIncentive>>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

      LocalCache.Put(localCacheKey, trimIncentive, LocalCacheTimeToLiveInSecs);

      return trimIncentive;
    }
    
    public IEnumerable<TrimIncentive> GetTrimIncentivesByTrimIdByZip(int trimId, string zip)
    {
      return GetTrimIncentivesByTrimIdByZipAsync(trimId, zip).Result;
    }

    #endregion


    #region Compare Cars

    public Trim GetCompareCarOverviewByTrimId(int trimId)
    {
      var localCacheKey = String.Format("compare_car_overview:by_trimid[{0}]", trimId);

      // First, check LocalCache.
      var compareCarOverview = LocalCache.Get<Trim>(localCacheKey);
      if (compareCarOverview != null)
        return compareCarOverview;

      // Then, check Tier 2 Redis.
      var redisCacheKey = String.Format("{0}:{1}", VehicleSpecTierTwoHashKey, localCacheKey);
      var result = RedisReadable.GetDatabase(DatabaseNumber).StringGetAsync(redisCacheKey);
      var json = RedisReadable.Wait(result);

      if (!json.IsNullOrEmpty)
      {
        compareCarOverview = JsonConvert.DeserializeObject<Trim>(json);
        LocalCache.Put(localCacheKey, compareCarOverview, LocalCacheTimeToLiveInSecs);
        return compareCarOverview;
      }

      return null;
    }
    
    public async Task<IEnumerable<Trim>> GetCompareCarsByTrimIdListAsync(string trimIdList)
    {
      // Return empty collection if bad input params.
      if (trimIdList.Length < 1)
        return new List<Trim>();

      // Place string list into array of ints
      string[] trimIdStrArr = trimIdList.Split(',');
      var ints = new List<int>();
      foreach (var item in trimIdStrArr)
      {
        int v;
        if (int.TryParse(item, out v))
          ints.Add(v);
      }
      int[] trimIdArr = ints.ToArray();

      var serviceTrimIdList = new List<int>();

      var localTrimList = new List<Trim>();
      var serviceTrimList = new List<Trim>();
      var resultTrimList = new List<Trim>();

      // Get all Compare Trims that are in local or in redis cache
      foreach (var t in trimIdArr)
      {
        var trim = GetCompareCarOverviewByTrimId(t);
        if (trim == null)
        {
          serviceTrimIdList.Add(t);
        }
        else
        {
          localTrimList.Add(trim);
        }
      }

      // Get Compare Trims from service that are not in local or redis cache
      if (serviceTrimIdList.Any())
      {
        string serviceTrimIdListStr = string.Join(",", serviceTrimIdList.ToArray());

        var path = String.Format("/trimidlist/{0}", serviceTrimIdListStr);

        serviceTrimList = await FetchResource<List<Trim>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        
        if (serviceTrimList.Any())
        {
          string localCacheKey = "";
          foreach (var trimItem in serviceTrimList)
          {
            localCacheKey = String.Format("compare_car_overview:by_trimid[{0}]", trimItem.Id);
            LocalCache.Put(localCacheKey, trimItem, LocalCacheTimeToLiveInSecs);
          }
        }
      }

      foreach (var rt in trimIdArr)
      {
        if (localTrimList.FirstOrDefault(t => t.Id == rt) != null)
          resultTrimList.Add(localTrimList.FirstOrDefault(t => t.Id == rt));
        else if (serviceTrimList.FirstOrDefault(t => t.Id == rt) != null)
          resultTrimList.Add(serviceTrimList.FirstOrDefault(t => t.Id == rt));
      }

      return resultTrimList;
    }

    public IEnumerable<Trim> GetCompareCarsByTrimIdList(string trimIdList)
    {
      return GetCompareCarsByTrimIdListAsync(trimIdList).Result;
    }

    #endregion


    #region Model Methods

    public async Task<IEnumerable<IModel>> GetAllModelsAsync()
    {
      // First, check LocalCache.
      const string localCacheKey = ModelsFieldName;
      var models = LocalCache.Get<ICollection<Model>>(localCacheKey);
      if (models != null)
        return models;

      // Then, check Redis or the VehicleSpec service
      var json = await RedisReadable
        .GetDatabase()
        .HashGetAsync(VehicleSpecTierOneHashKey, ModelsFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/models";
        models = await FetchResource<List<Model>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, models);
        return models;
      }

      models = JsonConvert.DeserializeObject<List<Model>>(json);
      LocalCache.Put(localCacheKey, models);
      return models;
    }

    public IEnumerable<IModel> GetAllModels()
    {
      return GetAllModelsAsync().Result;
    }



    public async Task<IEnumerable<IModel>> GetModelsByMakeAsync(string makeSeoName)
    {
      // First, check LocalCache.
      var localCacheKey = String.Format("models:by_make[{0}]", makeSeoName);
      var models = LocalCache.Get<ICollection<Model>>(localCacheKey);
      if (models != null)
        return models;

      // Then, check Redis or the VehicleSpec service
      var redisCacheKey = String.Format("{0}:{1}", VehicleSpecTierTwoHashKey, localCacheKey);
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .StringGetAsync(redisCacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/make/{0}/models/", makeSeoName);
        models = await FetchResource<List<Model>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);

        LocalCache.Put(localCacheKey, models);
        return models;
      }

      models = JsonConvert.DeserializeObject<List<Model>>(json);
      LocalCache.Put(localCacheKey, models);
      return models;
    }

    public IEnumerable<IModel> GetModelsByMake(string makeSeoName)
    {
      return GetModelsByMakeAsync(makeSeoName).Result;
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

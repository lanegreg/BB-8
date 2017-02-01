using Car.Com.Common;
using Car.Com.Common.Cache;
using Car.Com.Domain.Models.CarsForSale;
using Car.Com.Domain.Models.CarsForSale.Api;
using Car.Com.Domain.Models.CarsForSale.Common;
using Car.Com.Domain.Models.CarsForSale.Exclusions;
using Car.Com.Domain.Services;
using Car.Com.Service.Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Car.Com.Service.Data.Impl
{
  public sealed class CarsForSaleService : ICarsForSaleService, ICacheable
  {
    #region Declarations

    private static readonly int RefreshIntervalInMins = WebConfig.Get<int>("CarsForSaleService:ReCacheInterval_mins");
    private static readonly bool IsLocalDev = WebConfig.Get<bool>("Environment:IsLocalDev");
    private static int _loadAmount = 3000000;

    private static readonly object Mutex = new object();
    private static readonly MemoryCache Cache = MemoryCache.Default;

    private static readonly string AbtProdConnString = WebConfig.Get<string>("ConnectionString:Abt_Prod");
    private static readonly string InventoryConnString = WebConfig.Get<string>("ConnectionString:Inventory");

    private static readonly IEnumerable<string> _makesBlackList;
    private static FilterDomains _filterDomains;
    private static IDictionary<string, int> _inventoryCountPerMakeDict; // (make_name, inventory_count)
    private static IDictionary<string, Dealer> _dealerByDealerIdDict; // (dealer_id, dealer_object)
    private static IDictionary<int, List<CarForSale>> _inventoryByDealerIdDict; // (dealer_id, dealer_inventory)
    private static IList<Dto.AutoCheck> _autoChecks;

    #endregion


    #region ctors

    static CarsForSaleService()
    {
      _filterDomains = new FilterDomains();

      _makesBlackList = new List<string>();
    }

    public CarsForSaleService()
    {
      if (IsLocalDev)
      {
        var loadAmount = WebConfig.Get<int>("Environment:LocalDev:CarsForSaleLoadAmount");
        if (loadAmount != 0)
          _loadAmount = loadAmount;
      }
    }

    #endregion


    #region Interface Impls

    public void Warm()
    {
      CacheAllCarsForSale();
      Cache.Set("CarsForSale_Service", String.Empty, GetCachePolicy());
    }

    #endregion


    #region Public Static Interface Methods

    public Task<IEnumerable<IFeature>> GetModelsDomainByMakeIdAsync(int makeId)
    {
      return Task.Run(() => _filterDomains.GetModelsDomainByMakeId(makeId));
    }

    public IEnumerable<IFeature> GetModelsDomainByMakeId(int makeId)
    {
      return GetModelsDomainByMakeIdAsync(makeId).Result;
    }


    public Task<IEnumerable<IFeature>> GetMakesDomainByCategoryIdAsync(int categoryId)
    {
      return Task.Run(() => _filterDomains.GetMakesDomainByCategoryId(categoryId));
    }

    public IEnumerable<IFeature> GetMakesDomainByCategoryId(int categoryId)
    {
      return GetMakesDomainByCategoryIdAsync(categoryId).Result;
    }



    public IDictionary<string, int> GetTopMakesWithInventoryCount(int takeAmount)
    {
      return _inventoryCountPerMakeDict
        .Take(takeAmount)
        .ToDictionary(i => i.Key, i => i.Value);
    }


    public IDictionary<string, int> GetAllMakesWithInventoryCount()
    {
      return _inventoryCountPerMakeDict;
    }


    public Task<ISearchResults> SearchAsync(SearchCriteria criteria)
    {
      return Task.Run<ISearchResults>(() =>
      {
        // STEP 1: Start by getting a list of cars that are near a zipcode based on dealer coverage, 
        // and then fall within a certain radius distance from the zipcode centroid.
        var inventoryByZipcodeByRadiusMiles = GetInventoryByZipcodeByRadiusMiles(criteria.Zipcode, criteria.RadiusMiles);


        // STEP 2: Reduce the list of cars based on the criteria in the reduction filters.
        var carsForSale = FilterInventory(inventoryByZipcodeByRadiusMiles, criteria).ToList();


        // STEP 3: Sort the reduced list by description and direction.
        var sortedCarsForSale = SortTheCarsForSale(carsForSale, criteria);


        // STEP 4: Paginate from the reduced, sorted list of cars.
        var carsForSaleByPage = sortedCarsForSale.Skip(criteria.Skip).Take(criteria.Take).ToList();


        // STEP 5: Get pricing and mileage values for filter use.
        var carsForSalePricesMin = 0;
        var carsForSalePricesMax = 0;
        var carsForSaleMileagesMin = 0;
        var carsForSaleMileagesMax = 0;
        var carsForSalePrices = new List<int>();
        var carsForSaleMileages = new List<int>();
        var priceGroupQuantities = "0|0|0|0|0";
        var mileageGroupQuantities = "0|0|0|0|0";

        if (carsForSale.Any())
        {
          for (int i = 0; i < carsForSale.Count(); i++)
          {
            carsForSalePrices.Add(carsForSale[i].AskingPrice);
            carsForSaleMileages.Add(carsForSale[i].Mileage);
          }
          carsForSalePricesMin = carsForSalePrices.Min();
          carsForSalePricesMax = carsForSalePrices.Max();
          carsForSaleMileagesMin = carsForSaleMileages.Min();
          carsForSaleMileagesMax = carsForSaleMileages.Max();

          if (criteria.MaxMileage.IsNotNullOrEmpty() && criteria.MaxMileage != "0")
          {
            carsForSaleMileagesMax = Convert.ToInt32(criteria.MaxMileage);
          }
          if (criteria.MaxPrice.IsNotNullOrEmpty() && criteria.MaxPrice != "0")
          {
            carsForSalePricesMax = Convert.ToInt32(criteria.MaxPrice);
          }

          var price20Pct = carsForSalePricesMax * 0.20;
          var price40Pct = carsForSalePricesMax * 0.40;
          var price60Pct = carsForSalePricesMax * 0.60;
          var price80Pct = carsForSalePricesMax * 0.80;

          var price20PctCount = carsForSalePrices.Count(p => p <= price20Pct);
          var price40PctCount = carsForSalePrices.Count(p => p > price20Pct && p < price40Pct);
          var price60PctCount = carsForSalePrices.Count(p => p > price40Pct && p < price60Pct);
          var price80PctCount = carsForSalePrices.Count(p => p > price60Pct && p < price80Pct);
          var price100PctCount = carsForSalePrices.Count(p => p > price80Pct && p <= carsForSalePricesMax);
          priceGroupQuantities = price20PctCount + "|" + price40PctCount + "|" + price60PctCount + "|" + price80PctCount + "|" + price100PctCount;

          var mileage20Pct = carsForSaleMileagesMax * 0.20;
          var mileage40Pct = carsForSaleMileagesMax * 0.40;
          var mileage60Pct = carsForSaleMileagesMax * 0.60;
          var mileage80Pct = carsForSaleMileagesMax * 0.80;

          var mileage20PctCount = carsForSaleMileages.Count(p => p <= mileage20Pct);
          var mileage40PctCount = carsForSaleMileages.Count(p => p > mileage20Pct && p < mileage40Pct);
          var mileage60PctCount = carsForSaleMileages.Count(p => p > mileage40Pct && p < mileage60Pct);
          var mileage80PctCount = carsForSaleMileages.Count(p => p > mileage60Pct && p < mileage80Pct);
          var mileage100PctCount = carsForSaleMileages.Count(p => p > mileage80Pct && p <= carsForSaleMileagesMax);
          mileageGroupQuantities = mileage20PctCount + "|" + mileage40PctCount + "|" + mileage60PctCount + "|" + mileage80PctCount + "|" + mileage100PctCount;

        }

        return new SearchResults(
          carsForSaleByPage, 
          carsForSale.Count(), 
          carsForSalePricesMax, 
          carsForSaleMileagesMax, 
          carsForSalePricesMin, 
          carsForSaleMileagesMin, 
          priceGroupQuantities,
          mileageGroupQuantities,
          criteria);
      });
    }

    public ISearchResults Search(SearchCriteria criteria)
    {
      return SearchAsync(criteria).Result;
    }


    public Task<ISearchResults> SuggestedSearchAsync(SearchCriteria criteria)
    {
      return Task.Run<ISearchResults>(() =>
      {
        // STEP 1: Start by getting a list of cars that are near a zipcode based on dealer coverage, 
        // and then fall within a certain radius distance from the zipcode centroid.
        var inventoryByZipcodeByRadiusMiles = GetInventoryByZipcodeByRadiusMiles(criteria.Zipcode, criteria.RadiusMiles);


        // STEP 2: Reduce the list of cars based on the criteria in the reduction filters.
        var carsForSale = FilterInventory(inventoryByZipcodeByRadiusMiles, criteria).ToList();


        // STEP 2a: Reduce the list of cars based on the criteria in the reduction / exclusion filters.
        var dedupedCarsForSale = ApplyExclusionsToInventory(carsForSale, criteria).ToList();


        //  STEP 2b:  De-dup CarsForSale by DealerID
        IEqualityComparer<CarForSale> DealerIdComparer = new CarsDealerDedupe();
        IEnumerable<CarForSale> distinctCarsForSale = dedupedCarsForSale.Distinct(DealerIdComparer).ToList();


        // STEP 3: Sort the reduced list by description and direction.
        var sortedCarsForSale = SortTheCarsForSale(distinctCarsForSale, criteria);


        // STEP 4: Paginate from the reduced, sorted list of cars.
        var carsForSaleByPage = sortedCarsForSale.Skip(criteria.Skip).Take(criteria.Take).ToList();


        // STEP 5: Set mock values for SearchResults class instance... Class is shared with other methods.
        var carsForSalePricesMin = 0;
        var carsForSalePricesMax = 0;
        var carsForSaleMileagesMin = 0;
        var carsForSaleMileagesMax = 0;
        var priceGroupQuantities = "0|0|0|0|0";
        var mileageGroupQuantities = "0|0|0|0|0";

        return new SearchResults(
          carsForSaleByPage, 
          distinctCarsForSale.Count(), 
          carsForSalePricesMax, 
          carsForSaleMileagesMax, 
          carsForSalePricesMin, 
          carsForSaleMileagesMin, 
          priceGroupQuantities, 
          mileageGroupQuantities,
          criteria);
      });
    }

    public ISearchResults SuggestedSearch(SearchCriteria criteria)
    {
      return SuggestedSearchAsync(criteria).Result;
    }


    public Task<ICarForSale> GetCarForSaleByDealerIdByInventoryIdAsync(int dealerId, int inventoryId)
    {
      return Task.Run<ICarForSale>(() =>
      {
        if (!_inventoryByDealerIdDict.ContainsKey(dealerId))
          return null;

        var dealerInventory = _inventoryByDealerIdDict[dealerId];
        var carForSale = dealerInventory.FirstOrDefault(i => i.Id == inventoryId);

        if (carForSale != null)
        {
          using (var conn = AbtProdDbConn())
          {
            var details = conn.Query<Dto.ItemDetails>("CCWeb.GetInventoryItemDetails",
              new { InventoryId = inventoryId },
              commandType: CommandType.StoredProcedure)
              .FirstOrDefault();

            if (details == null)
              return null;

            var categoryId = carForSale.CategoryId.ToString(CultureInfo.InvariantCulture);
            var categoryName = _filterDomains.Categories.First(c => c.MatchValue.Equals(categoryId)).Description;

            carForSale.Category = categoryName;
            carForSale.NumOfDoors = details.NumOfDoors;
            carForSale.InteriorColor = details.InteriorColor;
            carForSale.VehicleDetails = details.VehicleDetails.ToValueOrEmpty();
            carForSale.SellerNotes = details.SellerNotes.ToValueOrEmpty();
            carForSale.Dealer.Message = details.DealerMessage.ToValueOrEmpty();
          }

          return carForSale;
        }

        return null;
      });
    }

    public ICarForSale GetCarForSaleByDealerIdByInventoryId(int dealerId, int inventoryId)
    {
      return GetCarForSaleByDealerIdByInventoryIdAsync(dealerId, inventoryId).Result;
    }


    public IEnumerable<string> MakesBlackList
    {
      get { return _makesBlackList; }
    }

    public IFilterDomains FilterDomains
    {
      get { return _filterDomains; }
    }

    #endregion


    #region Private Static Methods

    private static void CacheUpdateHandler(CacheEntryUpdateArguments args)
    {
      CacheAllCarsForSale();
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


    private static void CacheAllCarsForSale()
    {
      CacheInventorySupportData();
      CacheInventoryData();
    }

    private static void CacheInventorySupportData()
    {
      using (var conn = AbtProdDbConn())
      {
        var recordsets = conn.QueryMultiple("CCWeb.GetInventorySupportCache");

        // Cache Options domain (1st recordset)
        var options = recordsets.Read<Feature>().ToList();
        _filterDomains.SetOptions(options);


        // Cache FuelTypes domain (2nd recordset)
        var fuelTypes = recordsets.Read<Feature>().ToList();
        _filterDomains.SetFuelTypes(fuelTypes);


        // Dealer's AutoCheckIds (3rd recordset)
        _autoChecks = recordsets.Read<Dto.AutoCheck>().ToList();


        // Cache Categories domain 
        var categories =
          UriTokenTranslators.GetAllCategoryTranslators()
          /* we need to temporarily filter-out 'Alternative Fuel' until we can be properly map it during the inventory feed */
            .Where(c => c.Id != 1)
            .Select(c => new Feature
            {
              MatchValue = c.Id.ToString(CultureInfo.InvariantCulture),
              Description = c.Name
            });
        _filterDomains.SetCategories(categories);
      }
    }

    private static void CacheInventoryData()
    {
      using (var conn = InventoryDbConn())
      {
        // Fetch Cache
        var recordsets = conn.QueryMultiple("CCWeb.GetInventoryCache",
          new { LoadAmount = _loadAmount },
          commandType: CommandType.StoredProcedure);


        // Cache Dealers (1st recordset)
        var dealerByDealerIdDict = new Dictionary<string, Dealer>();
        var dealerList = recordsets.Read<Dealer>().ToList();

        foreach (Dealer dealer in dealerList)
        {
          string key = string.Format("{0}-{1}", dealer.Id, dealer.ProgramId);
          if (!dealerByDealerIdDict.ContainsKey(key))
          {
            dealerByDealerIdDict.Add(key, dealer);
            if (_autoChecks.ToList().FindIndex(a => a.DealerId == dealer.Id) >= 0)
            {
              dealerByDealerIdDict[key].AutoCheckId =
              _autoChecks.ToList().Find(a => a.DealerId == dealer.Id).AutoCheckId;
            }
          }
        }

        lock (Mutex)
        {
          _dealerByDealerIdDict = dealerByDealerIdDict;
        }


        // Cache Inventory (2nd recordset)
        var inventory = recordsets.Read<CarForSale>().ToList();
        // Create inventory (CarForSale) dictionary by segmenting on DealerId.
        var inventoryByDealerIdDict = inventory
          .GroupBy(i => i.DealerId)
          .Select(g => new { DealerId = g.Key, Inventory = g.ToList() })
          .ToDictionary(d => d.DealerId, d => d.Inventory);

        foreach (var dealerItem in inventoryByDealerIdDict.Select(dealerInventory => dealerInventory.Key))
        {
          for (int invItemIndx = 0; invItemIndx < inventoryByDealerIdDict[dealerItem].Count; invItemIndx++)
          {
            string key = string.Format("{0}-{1}", inventoryByDealerIdDict[dealerItem][invItemIndx].DealerId, inventoryByDealerIdDict[dealerItem][invItemIndx].ProgramId);
            inventoryByDealerIdDict[dealerItem][invItemIndx].Dealer = _dealerByDealerIdDict[key];
          }
        }

        lock (Mutex)
        {
          _inventoryByDealerIdDict = inventoryByDealerIdDict;
        }


        // Cache Years domain
        var years = inventory
          .GroupBy(i => new { Year = i.Year, yearString = i.Year.ToString(CultureInfo.InvariantCulture) })
          .OrderBy(y => y.Key.Year)
          .Select(y => new Feature(y.Key.yearString, y.Key.yearString))
          .ToList();

        _filterDomains.SetYears(years);


        // Get a list of make|model combinations
        var makeModels = inventory
          .Where(i => !_makesBlackList.Contains(i.Make))
          .GroupBy(i => new { i.MakeId, i.Make, i.ModelId, i.Model })
          .Select(mm => mm.Key)
          .ToList();


        // Cache Makes domain
        var makes = makeModels
          .GroupBy(i => new { i.MakeId, i.Make })
          .Select(ma => new Feature(ma.Key.MakeId.ToString(CultureInfo.InvariantCulture), ma.Key.Make))
          .ToList();

        _filterDomains.SetMakes(makes);


        // Get counts per Make
        // Cache inventory counts, by storing in a dictionary, segmented by MakeId.
        _inventoryCountPerMakeDict = inventory
          .GroupBy(i => i.Make)
          .OrderByDescending(i => i.Count())
          .ToDictionary(ma => ma.Key, ma => ma.Count());


        // Cache Models domain
        // Cache Make's Models, by storing in a dictionary, segmented by MakeId.
        var modelsDictByMakeId = new Dictionary<int, IEnumerable<Feature>>();
        foreach (var make in _filterDomains.Makes)
        {
          var makeId = Int32.Parse(make.MatchValue);
          var modelList = makeModels
            .Where(mamo => mamo.MakeId == makeId)
            .GroupBy(mamo => mamo.Model)
            .Select(mo => new Feature(mo.Key, mo.Key))
            .ToList();

          modelsDictByMakeId.Add(makeId, modelList);
        }

        _filterDomains.SetModelsDictionary(modelsDictByMakeId);


        // Get a list of category|make combinations
        var categoryMakes = inventory
          .GroupBy(i => new { i.CategoryId, i.Category, i.MakeId, i.Make })
          .Select(cm => cm.Key)
          .ToList();


        // Cache Makes domain
        // Cache Category's Makes, by storing in a dictionary, segmented by CategoryId.
        var makesDictByCategoryId = new Dictionary<int, IEnumerable<Feature>>();
        foreach (var category in _filterDomains.Categories)
        {
          var categoryId = Int32.Parse(category.MatchValue);
          var makeList = categoryMakes
            .Where(cama => cama.CategoryId == categoryId)
            .GroupBy(cama => cama.Make)
            .Select(ma => new Feature(ma.Key, ma.Key))
            .ToList();

          makesDictByCategoryId.Add(categoryId, makeList);
        }

        _filterDomains.SetMakesDictionary(makesDictByCategoryId);
      }
    }

    #endregion


    #region Private Static Methods for Sub-Filtering

    private static IEnumerable<CarForSale> GetInventoryByZipcodeByRadiusMiles(string zipcode, int radiusMiles)
    {
      using (var conn = AbtProdDbConn())
      {
        // This gets us the current dealer coverage for a given zipcode.
        var dealerCoverage = (ICollection<DealerCoverage>)conn.Query<DealerCoverage>("CCWeb.GetDealerCoverage",
          new { Zipcode = zipcode },
          commandType: CommandType.StoredProcedure);

        // Then, get only the dealers that fall within the radius constraint.
        var validDealerIds = dealerCoverage
          .Where(d => d.Distance <= radiusMiles)
          .Select(d => d.DealerId);

        // Then, get just the inventory that belongs to these covered dealers.
        var inventory = new List<CarForSale>();
        var inventoryForTheseDealers = validDealerIds
          .Where(dealerId => _inventoryByDealerIdDict.ContainsKey(dealerId))
          .Select(dealerId => new
          {
            DealerId = dealerId,
            Inventory = _inventoryByDealerIdDict[dealerId]
          });

        foreach (var inventoryForThisDealer in inventoryForTheseDealers)
        {
          foreach (var carForSale in inventoryForThisDealer.Inventory)
            carForSale.DistanceInMiles = dealerCoverage
              .First(d => d.DealerId == inventoryForThisDealer.DealerId)
              .Distance;

          inventory.AddRange(inventoryForThisDealer.Inventory);
        }

        return inventory;
      }
    }

    private static IEnumerable<CarForSale> FilterInventory(IEnumerable<CarForSale> inventory, SearchCriteria criteria)
    {
      #region - Business Rules
      /**
       * If *all* ComboFilters are NotDefined, we show *all* inventory
       * ...otherwise, we filter based on combo matches.
       */
      #endregion

      var showAllInventory = criteria.CategoryMakeComboFilter.NotDefined && criteria.MakeModelComboFilter.NotDefined;

      return inventory
        .Where(
          carForSale => (showAllInventory ||
                         (criteria.MakeModelComboFilter.MatchesThis(carForSale) ||
                          criteria.CategoryMakeComboFilter.MatchesThis(carForSale))) &&
                        criteria.NewStatusFilter.MatchesThis(carForSale) &&
                        criteria.PriceRangeFilter.MatchesThis(carForSale) &&
                        criteria.MileageRangeFilter.MatchesThis(carForSale) &&
                        criteria.YearRangeFilter.MatchesThis(carForSale) &&
                        criteria.FuelTypeFilter.MatchesThis(carForSale) &&
                        criteria.OptionBitsFilter.MatchesThis(carForSale) &&
                        criteria.TrimIdFilter.MatchesThis(carForSale)
                        )
        .ToList();
    }

    private static IEnumerable<CarForSale> ApplyExclusionsToInventory(IEnumerable<CarForSale> inventory, SearchCriteria criteria)
    {
      #region - Business Rules
      /**
       * If *all* ComboFilters are NotDefined, we show *all* inventory
       * ...otherwise, we filter based on combo matches.
       */
      #endregion

      return inventory
        .Where(
          carForSale => criteria.DealersExclusion.MatchesThis(carForSale))
        .ToList();
    }

    private static IEnumerable<CarForSale> SortTheCarsForSale(IEnumerable<CarForSale> carsForSale, SearchCriteria criteria)
    {
      if (Sort.By.SuggestedRanking == criteria.Sort.By)
      {
        List<SuggestedVehicles> suggestedCarsForSale = new List<SuggestedVehicles>();

        foreach (var item in carsForSale)
        {
          int sDeltaPrice = 0;
          int sDeltaMileage = 0;
          int vdpMileage = 0;
          int vdpPrice = 0;

          if (int.TryParse(criteria.vdpMileage, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out vdpMileage))
          {
            sDeltaMileage = Math.Abs(vdpMileage - item.Mileage);
          }

          if (int.TryParse(criteria.vdpPrice, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out vdpPrice))
          {
            sDeltaPrice = Math.Abs(vdpPrice - item.AskingPrice);
          }

          suggestedCarsForSale.Add(new SuggestedVehicles()
          {
            Id = item.Id,
            Vin = item.Vin,
            MakeId = item.MakeId,
            Make = item.Make,
            Model = item.Model,
            Year = item.Year,
            IsNewStatus = item.IsNewStatus,
            CityMpg = item.CityMpg,
            HighwayMpg = item.HighwayMpg,
            Mileage = item.Mileage,
            //HasMissingMileage = item.Mileage <= 0,
            AskingPrice = item.AskingPrice,
            //HasMissingPrice = !item.HasValidAskingPrice,
            Cylinders = item.Cylinders,
            ExteriorColor = item.ExteriorColor,
            InteriorColor = item.InteriorColor,
            DriveType = item.DriveType,
            TransmissionType = item.TransmissionType,
            DistanceInMiles = item.DistanceInMiles,
            PrimaryImage = item.PrimaryImage.IsNotNullOrEmpty() ? item.PrimaryImage : String.Empty,
            //Details = (query.IsDetailPage ? c.VehicleDetails : null),
            SellerNotes = item.SellerNotes,
            ImageUrls = item.ImageUrls,
            VehicleDetails = item.VehicleDetails,
            Category = item.Category,
            NumOfDoors = item.NumOfDoors,
            SquishVin = item.SquishVin,
            deltaPrice = sDeltaPrice,
            deltaMileage = sDeltaMileage,
            TrimId = item.TrimId,
            CategoryId = item.CategoryId,
            DisplayName = item.DisplayName,
            FuelTypeId = item.FuelTypeId,
            Dealer = new Dealer
            {
              Id = item.Dealer.Id,
              Name = item.Dealer.Name,
              Phone = item.Dealer.Phone,
              City = item.Dealer.City,
              State = item.Dealer.State,
              IsPremiumPlacement = item.Dealer.IsPremiumPlacement,
              Message = (item.Dealer.Message),
            },
          });
        };

        return suggestedCarsForSale.OrderBy(c => c.deltaPrice)
          .ThenBy(c => c.deltaMileage);
      }


      if (Sort.By.BestMatch == criteria.Sort.By)
      {
        return carsForSale
          .OrderByDescending(c => c.HasPrimaryImage)
          .ThenByDescending(c => c.Dealer.IsPremiumPlacement)
          .ThenByDescending(c => c.HasValidAskingPrice)
          .ThenByDescending(c => c.Dealer.RevenueScore)
          .ThenBy(c => c.Mileage)
          .ThenBy(c => c.DistanceInMiles);

      }


      if (Sort.By.Distance == criteria.Sort.By)
      {
        if (Sort.Direction.Descending == criteria.Sort.Direction)
        {
          return carsForSale
            .OrderByDescending(c => c.HasValidAskingPrice)
            .ThenByDescending(c => c.DistanceInMiles)
            .ThenByDescending(c => c.Dealer.RevenueScore);
        }

        return carsForSale
          .OrderByDescending(c => c.HasValidAskingPrice)
          .ThenBy(c => c.DistanceInMiles)
          .ThenByDescending(c => c.Dealer.RevenueScore);
      }


      if (Sort.By.Price == criteria.Sort.By)
      {
        if (Sort.Direction.Descending == criteria.Sort.Direction)
        {
          return carsForSale
            .OrderByDescending(c => c.HasValidAskingPrice)
            .ThenByDescending(c => c.AskingPrice)
            .ThenByDescending(c => c.Dealer.RevenueScore);
        }

        return carsForSale
          .OrderByDescending(c => c.HasValidAskingPrice)
          .ThenBy(c => c.AskingPrice)
          .ThenByDescending(c => c.Dealer.RevenueScore);
      }


      if (Sort.By.Mileage == criteria.Sort.By)
      {
        if (Sort.Direction.Descending == criteria.Sort.Direction)
        {
          return carsForSale
            .OrderByDescending(c => c.HasValidAskingPrice)
            .ThenByDescending(c => c.Mileage)
            .ThenByDescending(c => c.Dealer.RevenueScore);
        }

        return carsForSale
          .OrderByDescending(c => c.HasValidAskingPrice)
          .ThenBy(c => c.Mileage)
          .ThenByDescending(c => c.Dealer.RevenueScore);
      }


      return carsForSale
        .OrderByDescending(c => c.Dealer.IsPremiumPlacement)
        .ThenByDescending(c => c.HasValidAskingPrice)
        .ThenByDescending(c => c.Dealer.RevenueScore)
        .ThenBy(c => c.DistanceInMiles);
    }

    #endregion



    public static class Dto
    {
      public class ItemDetails
      {
        public int Id { get; set; }
        public string InteriorColor { get; set; }
        public string NumOfDoors { get; set; }
        public string VehicleDetails { get; set; }
        public string SellerNotes { get; set; }
        public string DealerMessage { get; set; }
      }

      public class AutoCheck
      {
        public int DealerId { get; set; }
        public int AutoCheckId { get; set; }
      }
    }


    #region Local Memory Cache

    private static ILocalCache LocalCache
    {
      get { return ServiceLocator.Get<ILocalCache>(); }
    }

    #endregion


    #region Database Connections

    private static IDbConnection AbtProdDbConn()
    {
      return ServiceLocator.Get<IDbConnectionFactory>().CreateConnection(AbtProdConnString);
    }

    private static IDbConnection InventoryDbConn()
    {
      return ServiceLocator.Get<IDbConnectionFactory>().CreateConnection(InventoryConnString);
    }

    #endregion
  }
}
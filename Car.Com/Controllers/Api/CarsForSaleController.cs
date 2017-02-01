using Car.Com.Common;
using Car.Com.Common.Api;
using Car.Com.Common.Cacheability;
using Car.Com.Controllers.Filters;
using Car.Com.Domain.Models.CarsForSale.Api;
using Car.Com.Domain.Models.CarsForSale.Filters;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Car.Com.Domain.Models.CarsForSale.Exclusions;


namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api/cars-for-sale")]
  public class CarsForSaleController : BaseApiController
  {

    #region - Client JavaScript Example

    /** CLIENT_SIDE EXAMPLE:
     * 
        $.post('/api/cars-for-sale/search/', {
            '': JSON.stringify({
              "zipcode": "92627",
              "radius_miles": 100,
              "page": {
                "current": 1,
                "items_per_page": 12
              },
              "sort": {
                "by": "best_match",
                "dir": "desc"
              },
              "filters": {
                "price_range": "0|300",
                "mileage_range": "0|300",
                "year_range": "2000|2016",
                "makes": "",
                "make_models": "19_grand cherokee",
                "city_mpg": "",
                "highway_mpg": "",
                "categories": "",
                "cylinders": "",
                "drive_types": "",
                "fuel_types": "",
                "tranny_types": "",
                "options": ""
              }
            })
          },
          function(data) {
            console.log(data);
          });
     * 
     * REF: http://encosia.com/using-jquery-to-post-frombody-parameters-to-web-api/
     **/

    #endregion
    

#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("search", Name = "GetInventory"), HttpPost]
    public DataWrapper GetInventory([FromBody] string jsonString)
    {
      Query query;
      try { query = JsonConvert.DeserializeObject<Query>(jsonString); }
      catch { throw new HttpResponseException(HttpStatusCode.NotFound); }


      #region - STEP 1: Create search-criteria model

      var searchCriteria = new SearchCriteria
      {
        Zipcode = query.Zipcode,
        MaxPrice = query.MaxPrice,
        MaxMileage = query.MaxMileage,
        RadiusMiles = query.RadiusMiles,
        Skip = query.Page.Skip,
        Take = query.Page.Take,
        Sort = new SearchCriteria.Dto.Sort(query.Sort.By, query.Sort.Direction),

        MakeModelComboFilter = new MakeModelComboFilter(query.Filters.MakeIds, query.Filters.MakeIdModels),
        CategoryMakeComboFilter = new CategoryMakeComboFilter(query.Filters.CategoryIds, query.Filters.CategoryIdMakes),
        PriceRangeFilter = new PriceRangeFilter(query.Filters.PriceRange),
        MileageRangeFilter = new MileageRangeFilter(query.Filters.MileageRange),
        YearRangeFilter = new YearRangeFilter(query.Filters.YearRange),
        FuelTypeFilter = new FuelTypeFilter(query.Filters.FuelTypeIds),
        OptionBitsFilter = new OptionBitsFilter(query.Filters.OptionIds)
      };

      #endregion


      #region - STEP 2: Query service and create response model

      var searchResults = CarsForSaleService.SearchAsync(searchCriteria).Result;

      // if we have a page requested that no longer has any inventory,
      // we will reset the page to 1, requery, and return page number one's results.
      if (!searchResults.CarsForSale.Any() && query.Page.Current > 1)
      {
        query.Page.Current = 1;
        searchCriteria.Skip = query.Page.Skip;
        searchResults = CarsForSaleService.SearchAsync(searchCriteria).Result;
      }

      List<int> autonationDealers = LeadService.GetLeadAutonationUsedCarDealers();
      
      var response = new Response
      {
        Zipcode = query.Zipcode,
        InventoryCount = searchResults.InventoryCount,
        MaxPrice = searchResults.MaxPrice,
        MaxMileage = searchResults.MaxMileage,
        MinPrice = searchResults.MinPrice,
        MinMileage = searchResults.MinMileage,
        PriceGroupQuantities = searchResults.PriceGroupQuantities,
        MileageGroupQuantities = searchResults.MileageGroupQuantities,
        Page = new Page
        {
          Current = searchResults.Page.Current,
          ItemsPerPage = searchResults.Page.ItemsPerPage,
          TotalPages = searchResults.Page.TotalPages
        },
        CarsForSale = searchResults.CarsForSale
          .Select(c => new CarForSale
          {
            Id = c.Id,
            Vin = c.Vin,
            MakeId = c.MakeId,
            Make = c.Make,
            Model = c.Model,
            Year = c.Year,
            IsNewStatus = c.IsNewStatus,
            CityMpg = c.CityMpg,
            HighwayMpg = c.HighwayMpg,
            Mileage = c.Mileage.ToString("#,###"),
            HasMissingMileage = c.Mileage <= 0,
            AskingPrice = c.HasValidAskingPrice ? c.AskingPrice.ToString("#,###") : String.Empty,
            HasMissingPrice = !c.HasValidAskingPrice,
            Cylinders = c.Cylinders.ToString(CultureInfo.InvariantCulture),
            ExteriorColor = c.ExteriorColor,
            InteriorColor = c.InteriorColor,
            DriveType = c.DriveType,
            TransmissionType = c.TransmissionType,
            Distance = c.DistanceInMiles,
            PrimaryImageUrl = c.PrimaryImage.IsNotNullOrEmpty() ? c.PrimaryImage : String.Empty,
            Details = (query.IsDetailPage ? c.VehicleDetails : null),
            Dealer = new Dealer
            {
              Id = c.Dealer.Id,
              Name = c.Dealer.Name,
              Phone = c.Dealer.Phone,
              City = c.Dealer.City,
              State = c.Dealer.State,
              IsTrusted = c.Dealer.IsPremiumPlacement,
              Message = (query.IsDetailPage ? c.Dealer.Message : null),
              Latitude = c.Dealer.Latitude,
              Longitude = c.Dealer.Longitude,
              AutonationDealer = autonationDealers.Contains(c.Dealer.Id)
            },
            DealerHours = new DealerHours
            {
              SalesMonOpen = GetDealerHour(c.Dealer.Id, "SalesMonOpen"), 
              SalesMonClose = GetDealerHour(c.Dealer.Id, "SalesMonClose"), 
              SalesTueOpen = GetDealerHour(c.Dealer.Id, "SalesTueOpen"),
              SalesTueClose = GetDealerHour(c.Dealer.Id, "SalesTueClose"), 
              SalesWedOpen = GetDealerHour(c.Dealer.Id, "SalesWedOpen"), 
              SalesWedClose = GetDealerHour(c.Dealer.Id, "SalesWedClose"),
              SalesThrOpen = GetDealerHour(c.Dealer.Id, "SalesThrOpen"), 
              SalesThrClose = GetDealerHour(c.Dealer.Id, "SalesThrClose"), 
              SalesFriOpen = GetDealerHour(c.Dealer.Id, "SalesFriOpen"), 
              SalesFriClose = GetDealerHour(c.Dealer.Id, "SalesFriClose"), 
              SalesSatOpen = GetDealerHour(c.Dealer.Id, "SalesSatOpen"),   
              SalesSatClose = GetDealerHour(c.Dealer.Id, "SalesSatClose"), 
              SalesSunOpen = GetDealerHour(c.Dealer.Id, "SalesSunOpen"),  
              SalesSunClose = GetDealerHour(c.Dealer.Id, "SalesSunClose") 
            }
          })
      };

      #endregion


      #region - Handle additional data for the vehicle detail page (singles)

      if (query.IsDetailPage && response.CarsForSale.Any())
      {
        var car = response.CarsForSale.ToList()[0];
        var additionalData = CarsForSaleService.GetCarForSaleByDealerIdByInventoryId(car.Dealer.Id, car.Id);

        if (additionalData != null)
        {
          car.SellerNotes = additionalData.SellerNotes;
          car.ImageUrls = additionalData.ImageUrls;
          car.Details = additionalData.VehicleDetails;
          car.Dealer.Message = additionalData.Dealer.Message;
        }
        
        response.CarsForSale = new List<CarForSale> { car };
      }

      #endregion
      

      return DataWrapper(response, response.CarsForSale.Count());
    }
    

    #region - API methods

#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("suggested", Name = "GetSuggested"), HttpPost]
    public DataWrapper GetSuggested([FromBody] string jsonString)
    {
      SuggestedQuery query;
      try { query = JsonConvert.DeserializeObject<SuggestedQuery>(jsonString); }
      catch { throw new HttpResponseException(HttpStatusCode.NotFound); }

      #region - STEP 1: Create search-criteria model

      var searchCriteria = new SearchCriteria
      {
        Zipcode = query.Zipcode,
        vdpPrice = query.VdpPrice,
        vdpMileage = query.VdpMileage,
        RadiusMiles = query.RadiusMiles,
        Skip = query.Page.Skip,
        Take = query.Page.Take,
        Sort = new SearchCriteria.Dto.Sort(query.Sort.By, query.Sort.Direction),

        MakeModelComboFilter = new MakeModelComboFilter(query.Filters.MakeIds, query.Filters.MakeIdModels),
        PriceRangeFilter = new PriceRangeFilter(query.Filters.PriceRange),
        MileageRangeFilter = new MileageRangeFilter(query.Filters.MileageRange),
        YearRangeFilter = new YearRangeFilter(query.Filters.YearRange),
        DealersExclusion = new DealersExclusion(query.Exclusions.Dealers)

      };

      #endregion


      #region - STEP 2: Query service and create response model



      var searchResults = CarsForSaleService.SuggestedSearchAsync(searchCriteria).Result;

      // if we have a page requested that no longer has any inventory,
      // we will reset the page to 1, requery, and return page number one's results.
      if (!searchResults.CarsForSale.Any() && query.Page.Current > 1)
      {
        query.Page.Current = 1;
        searchCriteria.Skip = query.Page.Skip;
        searchResults = CarsForSaleService.SuggestedSearchAsync(searchCriteria).Result;
      }
      
      var response = new Response
      {
        Zipcode = query.Zipcode,
        InventoryCount = searchResults.InventoryCount,
        Page = new Page
        {
          Current = searchResults.Page.Current,
          ItemsPerPage = searchResults.Page.ItemsPerPage,
          TotalPages = searchResults.Page.TotalPages
        },
        CarsForSale = searchResults.CarsForSale
          .Select(c => new CarForSale
          {
            Id = c.Id,
            Vin = c.Vin,
            MakeId = c.MakeId,
            Make = c.Make,
            Model = c.Model,
            Year = c.Year,
            IsNewStatus = c.IsNewStatus,
            CityMpg = c.CityMpg,
            HighwayMpg = c.HighwayMpg,
            Mileage = c.Mileage.ToString("#,###"),
            HasMissingMileage = c.Mileage <= 0,
            AskingPrice = c.HasValidAskingPrice ? c.AskingPrice.ToString("#,###") : String.Empty,
            HasMissingPrice = !c.HasValidAskingPrice,
            Cylinders = c.Cylinders.ToString(CultureInfo.InvariantCulture),
            ExteriorColor = c.ExteriorColor,
            InteriorColor = c.InteriorColor,
            DriveType = c.DriveType,
            TransmissionType = c.TransmissionType,
            Distance = c.DistanceInMiles,
            PrimaryImageUrl = c.PrimaryImage.IsNotNullOrEmpty() ? c.PrimaryImage : String.Empty,
            Details = (query.IsDetailPage ? c.VehicleDetails : null),
            Dealer = new Dealer
            {
              Id = c.Dealer.Id,
              Name = c.Dealer.Name,
              Phone = c.Dealer.Phone,
              City = c.Dealer.City,
              State = c.Dealer.State,
              IsTrusted = c.Dealer.IsPremiumPlacement,
              Message = (query.IsDetailPage ? c.Dealer.Message : null),
            }
          })
      };

      #endregion


      #region - Handle additional data for the vehicle detail page (singles)

      if (query.IsDetailPage && response.CarsForSale.Any())
      {
        var car = response.CarsForSale.ToList()[0];
        var additionalData = CarsForSaleService.GetCarForSaleByDealerIdByInventoryId(car.Dealer.Id, car.Id);

        car.SellerNotes = additionalData.SellerNotes;
        car.ImageUrls = additionalData.ImageUrls;
        car.Details = additionalData.VehicleDetails;
        car.Dealer.Message = additionalData.Dealer.Message;

        response.CarsForSale = new List<CarForSale> { car };
      }

      #endregion
      
      return DataWrapper(response, response.CarsForSale.Count());
    }



#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("filter-domains", Name = "GetFilterDomains"), HttpGet]
    public DataWrapper GetFilterDomains()
    {
      return DataWrapper(CarsForSaleService.FilterDomains);
    }


#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("makes", Name = "GetMakesWithInventoryCount"), HttpGet]
    public DataWrapper GetMakesWithInventoryCount()
    {
      var filterMakes = CarsForSaleService.FilterDomains.Makes;
      var makes = CarsForSaleService.GetAllMakesWithInventoryCount()
      .Select(m => new
      {
        description = m.Key,
        match_value = filterMakes.First(fm => fm.Description == m.Key).MatchValue,
        count = m.Value
      });

      return DataWrapper(makes, makes.Count());
    }


#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("make/{makeId:int}/models", Name = "GetModelsByMakeId"), HttpGet]
    public DataWrapper GetModelsByMakeId(int makeId)
    {
      var models = CarsForSaleService.GetModelsDomainByMakeId(makeId)
        .OrderBy(m => m.Description)
        .ToList();

      return DataWrapper(new
      {
        make_id = makeId,
        models
      },
      models.Count());
    }


#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("category/{categoryId:int}/makes", Name = "GetMakesByCategoryId"), HttpGet]
    public DataWrapper GetMakesByCategoryId(int categoryId)
    {
      var makes = CarsForSaleService.GetMakesDomainByCategoryId(categoryId)
        .OrderBy(m => m.Description)
        .ToList();

      return DataWrapper(new
      {
        category_id = categoryId,
        makes
      },
      makes.Count());
    }

#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("make/{makeId:int}", Name = "GetMakeByMakeId"), HttpGet]
    public DataWrapper GetMakeByMakeId(int makeId)
    {
      var make = CarsForSaleService.FilterDomains.Makes.FirstOrDefault(x => x.MatchValue == makeId.ToString()).Description;

      return DataWrapper(new
      {
        make
      });
    }

#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("category/{categoryId:int}", Name = "GetCategoryByCategoryId"), HttpGet]
    public DataWrapper GetCategoryByCategoryId(int categoryId)
    {
      var category = CarsForSaleService.FilterDomains.Categories.FirstOrDefault(x => x.MatchValue == categoryId.ToString()).Description;

      return DataWrapper(new
      {
        category
      });
    }

    #endregion


    #region - Inventory Ping Code

    /** CLIENT_SIDE EXAMPLE:
     * 
        $.post('/api/cars-for-sale/availability-ping/zipcode/84041/', {
            '': JSON.stringify({ make_id: 6, model: 'Enclave'})
          },
          function(data) {
            console.log(data);
          });
     * 
     * 
        $.post('/api/cars-for-sale/availability-ping/zipcode/92627/', {
            '': JSON.stringify({ make_id: 8, model: 'Camaro', trim_id: 22852})
          },
          function(data) {
            console.log('new by make', data.data.availability.new_cars.by_make);
            console.log('new by make|model', data.data.availability.new_cars.by_make_model);
            console.log('new by trim', data.data.availability.new_cars.by_trim);
            console.log('used by make', data.data.availability.used_cars.by_make);
            console.log('used by make|model', data.data.availability.used_cars.by_make_model);
            console.log('used by trim', data.data.availability.used_cars.by_trim);
          });
     * 
     * FYI: Need at least 200,000 recs loaded for this to show new car counts
     * REF: http://encosia.com/using-jquery-to-post-frombody-parameters-to-web-api/
     **/


#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("availability-ping/zipcode/{zipcode:minlength(5)}", Name = "GetInventoryAvailability"), HttpPost]
    public DataWrapper GetInventoryAvailability(string zipcode, [FromBody] string jsonString)
    {
      AvailabilityPingContext pingCtx;
      try { pingCtx = JsonConvert.DeserializeObject<AvailabilityPingContext>(jsonString); }
      catch { throw new HttpResponseException(HttpStatusCode.NotFound); }

      pingCtx.Zipcode = zipcode;

      if (pingCtx.Zipcode.Length != 5)
        return DataWrapper(StructureTheResponse(pingCtx, new Counts(), new Counts(), new Counts()));

      if (pingCtx.MakeId.IsNullOrEmpty() && pingCtx.Make.IsNotNullOrEmpty())
        pingCtx.MakeId = CarsForSaleService.FilterDomains.Makes.First(m => m.Description == pingCtx.Make).MatchValue;

      var makeLevelMatchCountsTask = GetMakeLevelMatchCounts(pingCtx);
      var makeModelLevelMatchCountsTask = GetMakeModelLevelMatchCounts(pingCtx);
      var trimLevelMatchCountsTask = GetTrimLevelMatchCounts(pingCtx);

      makeLevelMatchCountsTask.Wait();
      makeModelLevelMatchCountsTask.Wait();
      trimLevelMatchCountsTask.Wait();

      var makeLevelMatchCounts = makeLevelMatchCountsTask.Result;
      var makeModelLevelMatchCounts = makeModelLevelMatchCountsTask.Result;
      var trimLevelMatchCount = trimLevelMatchCountsTask.Result;


      return DataWrapper(StructureTheResponse(pingCtx, makeLevelMatchCounts, makeModelLevelMatchCounts, trimLevelMatchCount));
    }


    private static AvailabilityPingResponse StructureTheResponse(
      AvailabilityPingContext pingCtx,
      Counts makeLevelMatchCounts,
      Counts makeModelLevelMatchCounts,
      Counts trimLevelMatchCount)
    {
      return new AvailabilityPingResponse
      {
        PingContext = pingCtx,
        Availability = new
        {
          new_cars = new
          {
            by_make = new
            {
              count = makeLevelMatchCounts.NewCars
            },
            by_make_model = new
            {
              count = makeModelLevelMatchCounts.NewCars
            },
            by_trim = new
            {
              count = trimLevelMatchCount.NewCars
            }
          },
          used_cars = new
          {
            by_make = new
            {
              count = makeLevelMatchCounts.UsedCars
            },
            by_make_model = new
            {
              count = makeModelLevelMatchCounts.UsedCars
            },
            by_trim = new
            {
              count = trimLevelMatchCount.UsedCars
            }
          }
        }
      };
    }


    private static Task<Counts> GetMakeLevelMatchCounts(AvailabilityPingContext pingCtx)
    {
      if (pingCtx.MakeId.IsNullOrEmpty())
        return Task.Run(() => new Counts());

      return Task.Run(() =>
      {
        var newMakesTask = CarsForSaleService.SearchAsync(new SearchCriteria
        {
          Zipcode = pingCtx.Zipcode,
          MakeModelComboFilter = new MakeModelComboFilter(pingCtx.MakeId, String.Empty),
          NewStatusFilter = new NewStatusFilter(NewStatusFilter.Status.New)
        });

        var usedMakesTask = CarsForSaleService.SearchAsync(new SearchCriteria
        {
          Zipcode = pingCtx.Zipcode,
          MakeModelComboFilter = new MakeModelComboFilter(pingCtx.MakeId, String.Empty),
          NewStatusFilter = new NewStatusFilter(NewStatusFilter.Status.Used)
        });


        newMakesTask.Wait();
        usedMakesTask.Wait();

        var newMakes = newMakesTask.Result;
        var usedMakes = usedMakesTask.Result;

        return new Counts
        {
          NewCars = newMakes.InventoryCount,
          UsedCars = usedMakes.InventoryCount
        };
      });
    }


    private static Task<Counts> GetMakeModelLevelMatchCounts(AvailabilityPingContext pingCtx)
    {
      if (pingCtx.MakeId.IsNullOrEmpty() || pingCtx.Model.IsNullOrEmpty())
        return Task.Run(() => new Counts());

      return Task.Run(() =>
      {
        var newMakesTask = CarsForSaleService.SearchAsync(new SearchCriteria
        {
          Zipcode = pingCtx.Zipcode,
          MakeModelComboFilter = new MakeModelComboFilter(String.Empty, String.Format("{0}~{1}", pingCtx.MakeId, pingCtx.Model)),
          TrimIdFilter = new TrimIdFilter(pingCtx.TrimId),
          NewStatusFilter = new NewStatusFilter(NewStatusFilter.Status.New)
        });

        var usedMakesTask = CarsForSaleService.SearchAsync(new SearchCriteria
        {
          Zipcode = pingCtx.Zipcode,
          MakeModelComboFilter = new MakeModelComboFilter(String.Empty, String.Format("{0}~{1}", pingCtx.MakeId, pingCtx.Model)),
          TrimIdFilter = new TrimIdFilter(pingCtx.TrimId),
          NewStatusFilter = new NewStatusFilter(NewStatusFilter.Status.Used)
        });


        newMakesTask.Wait();
        usedMakesTask.Wait();

        var newMakes = newMakesTask.Result;
        var usedMakes = usedMakesTask.Result;

        return new Counts
        {
          NewCars = newMakes.InventoryCount,
          UsedCars = usedMakes.InventoryCount
        };
      });
    }


    private static Task<Counts> GetTrimLevelMatchCounts(AvailabilityPingContext pingCtx)
    {
      if (pingCtx.TrimId.IsNullOrEmpty())
        return Task.Run(() => new Counts());

      return Task.Run(() =>
      {
        var newMakesTask = CarsForSaleService.SearchAsync(new SearchCriteria
        {
          Zipcode = pingCtx.Zipcode,
          TrimIdFilter = new TrimIdFilter(pingCtx.TrimId),
          NewStatusFilter = new NewStatusFilter(NewStatusFilter.Status.New)
        });

        var usedMakesTask = CarsForSaleService.SearchAsync(new SearchCriteria
        {
          Zipcode = pingCtx.Zipcode,
          TrimIdFilter = new TrimIdFilter(pingCtx.TrimId),
          NewStatusFilter = new NewStatusFilter(NewStatusFilter.Status.Used)
        });


        newMakesTask.Wait();
        usedMakesTask.Wait();

        var newMakes = newMakesTask.Result;
        var usedMakes = usedMakesTask.Result;

        return new Counts
        {
          NewCars = newMakes.InventoryCount,
          UsedCars = usedMakes.InventoryCount
        };
      });
    }

    #endregion


    #region - Private methods

    private string GetDealerHour(int cyberId, string hourAttribute)
    {
      var autonationDealerHours = LeadService.GetLeadAutonationUsedCarDealerHours(cyberId).FirstOrDefault();
      string dealerHour = "please call";

      if (autonationDealerHours != null)
      {
        switch (hourAttribute)
        {
          case "SalesMonOpen":
            dealerHour = autonationDealerHours.SalesMonOpen != null && autonationDealerHours.SalesMonOpen.Length > 1 ? autonationDealerHours.SalesMonOpen : "please call";
            break;
          case "SalesMonClose":
            dealerHour = autonationDealerHours.SalesMonClose != null && autonationDealerHours.SalesMonClose.Length > 1 ? autonationDealerHours.SalesMonClose : "please call";
            break;
          case "SalesTueOpen":
            dealerHour = autonationDealerHours.SalesTueOpen != null && autonationDealerHours.SalesTueOpen.Length > 1 ? autonationDealerHours.SalesTueOpen : "please call";
            break;
          case "SalesTueClose":
            dealerHour = autonationDealerHours.SalesTueClose != null && autonationDealerHours.SalesTueClose.Length > 1 ? autonationDealerHours.SalesTueClose : "please call";
            break;
          case "SalesWedOpen":
            dealerHour = autonationDealerHours.SalesWedOpen != null && autonationDealerHours.SalesWedOpen.Length > 1 ? autonationDealerHours.SalesWedOpen : "please call";
            break;
          case "SalesWedClose":
            dealerHour = autonationDealerHours.SalesWedClose != null && autonationDealerHours.SalesWedClose.Length > 1 ? autonationDealerHours.SalesWedClose : "please call";
            break;
          case "SalesThrOpen":
            dealerHour = autonationDealerHours.SalesThrOpen != null && autonationDealerHours.SalesThrOpen.Length > 1 ? autonationDealerHours.SalesThrOpen : "please call";
            break;
          case "SalesThrClose":
            dealerHour = autonationDealerHours.SalesThrClose != null && autonationDealerHours.SalesThrClose.Length > 1 ? autonationDealerHours.SalesThrClose : "please call";
            break;
          case "SalesFriOpen":
            dealerHour = autonationDealerHours.SalesFriOpen != null && autonationDealerHours.SalesFriOpen.Length > 1 ? autonationDealerHours.SalesFriOpen : "please call";
            break;
          case "SalesFriClose":
            dealerHour = autonationDealerHours.SalesFriClose != null && autonationDealerHours.SalesFriClose.Length > 1 ? autonationDealerHours.SalesFriClose : "please call";
            break;
          case "SalesSatOpen":
            dealerHour = autonationDealerHours.SalesSatOpen != null && autonationDealerHours.SalesSatOpen.Length > 1 ? autonationDealerHours.SalesSatOpen : "please call";
            break;
          case "SalesSatClose":
            dealerHour = autonationDealerHours.SalesSatClose != null && autonationDealerHours.SalesSatClose.Length > 1 ? autonationDealerHours.SalesSatClose : "please call";
            break;
          case "SalesSunOpen":
            dealerHour = autonationDealerHours.SalesSunOpen != null && autonationDealerHours.SalesSunOpen.Length > 1 ? autonationDealerHours.SalesSunOpen : "please call";
            break;
          case "SalesSunClose":
            dealerHour = autonationDealerHours.SalesSunClose != null && autonationDealerHours.SalesSunClose.Length > 1 ? autonationDealerHours.SalesSunClose : "please call";
            break;
        }
      }

      if (dealerHour != "please call")
        dealerHour = Get12HourTimeString(dealerHour);
      
      return dealerHour;
    }
    
    private string Get12HourTimeString(string input24HourTimeString)
    {
      var timeFromInput = DateTime.ParseExact(input24HourTimeString, "H:m:s", null, DateTimeStyles.None);
      return timeFromInput.ToString("h:mm tt", CultureInfo.InvariantCulture);
    }

    #endregion


    #region - Services

    private static ICarsForSaleService CarsForSaleService
    {
      get { return ServiceLocator.Get<ICarsForSaleService>(); }
    }

    private static ILeadService LeadService
    {
      get { return ServiceLocator.Get<ILeadService>(); }
    }

    #endregion


    #region - Input / Output Models

    #region Availability Models

    protected class AvailabilityPingContext
    {
      [JsonProperty("make_id")]
      public string MakeId { get; set; }

      [JsonProperty("make")]
      public string Make { get; set; }

      [JsonProperty("model")]
      public string Model { get; set; }

      [JsonProperty("trim_id")]
      public string TrimId { get; set; }

      [JsonProperty("squish_vin")]
      public string SquishVin { get; set; }

      [JsonProperty("acode")]
      public string Acode { get; set; }

      [JsonProperty("zipcode")]
      public string Zipcode { get; set; }
    }

    protected class Counts
    {
      public int NewCars { get; set; }
      public int UsedCars { get; set; }
    }

    protected class AvailabilityPingResponse
    {
      [JsonProperty("ping_ctx")]
      public object PingContext { get; set; }

      [JsonProperty("availability")]
      public object Availability { get; set; }
    }

    #endregion


    // Input (query)
    protected class Query
    {
      [JsonProperty("zipcode")]
      public string Zipcode { get; set; }

      [JsonProperty("max_mileage")]
      public string MaxMileage { get; set; }

      [JsonProperty("max_price")]
      public string MaxPrice { get; set; }

      [JsonProperty("radius_miles")]
      public int RadiusMiles { get; set; }

      [JsonProperty("sort")]
      public Sort Sort { get; set; }

      [JsonProperty("page")]
      public Page Page { get; set; }

      [JsonProperty("filters")]
      public Filters Filters { get; set; }

      [JsonProperty("exclusions")]
      public Exclusions Exclusions { get; set; }

      [JsonIgnore]
      public bool IsDetailPage
      {
        get { return Page.ItemsPerPage == 1; }
      }
    }

    protected class SuggestedQuery
    {
      [JsonProperty("zipcode")]
      public string Zipcode { get; set; }

      [JsonProperty("vdp_price")]
      public string VdpPrice { get; set; }

      [JsonProperty("vdp_mileage")]
      public string VdpMileage { get; set; }

      [JsonProperty("radius_miles")]
      public int RadiusMiles { get; set; }

      [JsonProperty("sort")]
      public Sort Sort { get; set; }

      [JsonProperty("page")]
      public Page Page { get; set; }

      [JsonProperty("filters")]
      public Filters Filters { get; set; }

      [JsonProperty("exclusions")]
      public Exclusions Exclusions { get; set; }

      [JsonIgnore]
      public bool IsDetailPage
      {
        get { return Page.ItemsPerPage == 1; }
      }
    }

    protected class Exclusions
    {
      [JsonProperty("dealers")]
      public string Dealers { get; set; }
    }

    protected class Filters
    {
      [JsonProperty("price_range")]
      public string PriceRange { get; set; }

      [JsonProperty("mileage_range")]
      public string MileageRange { get; set; }

      [JsonProperty("year_range")]
      public string YearRange { get; set; }

      [JsonProperty("makes")]
      public string MakeIds { get; set; }

      [JsonProperty("make_models")]
      public string MakeIdModels { get; set; }

      [JsonProperty("city_mpg")]
      public string CityMpg { get; set; }

      [JsonProperty("highway_mpg")]
      public string HighwayMpg { get; set; }

      [JsonProperty("categories")]
      public string CategoryIds { get; set; }

      [JsonProperty("category_makes")]
      public string CategoryIdMakes { get; set; }

      [JsonProperty("cylinders")]
      public string Cylinders { get; set; }

      [JsonProperty("drive_types")]
      public string DriveTypes { get; set; }

      [JsonProperty("fuel_types")]
      public string FuelTypeIds { get; set; }

      [JsonProperty("tranny_types")]
      public string TransmissionTypes { get; set; }

      [JsonProperty("options")]
      public string OptionIds { get; set; }
    }

    protected class Sort
    {
      [JsonProperty("by")]
      public string By { get; set; }

      [JsonProperty("dir")]
      public string Direction { get; set; }
    }


    // Common
    protected class Page
    {
      [JsonProperty("current")]
      public int? Current { get; set; }

      [JsonProperty("items_per_page", NullValueHandling = NullValueHandling.Ignore)]
      public int? ItemsPerPage { get; set; }

      [JsonProperty("total_pages", NullValueHandling = NullValueHandling.Ignore)]
      public int? TotalPages { get; set; }

      [JsonIgnore]
      public int Skip
      {
        get
        {
          return ((PageNumber - 1) * Take);
        }
      }

      [JsonIgnore]
      public int Take
      {
        get
        {
          return (ItemsPerPage.HasValue ? ItemsPerPage.Value : 10);
        }
      }

      private int PageNumber
      {
        get
        {
          return (Current.HasValue ? Current.Value : 1);
        }
      }
    }


    // Output (response)
    protected class Response
    {
      [JsonProperty("zipcode")]
      public string Zipcode { get; set; }

      [JsonProperty("inventory_count")]
      public int InventoryCount { get; set; }

      [JsonProperty("max_price")]
      public int MaxPrice { get; set; }

      [JsonProperty("max_mileage")]
      public int MaxMileage { get; set; }

      [JsonProperty("min_price")]
      public int MinPrice { get; set; }

      [JsonProperty("min_mileage")]
      public int MinMileage { get; set; }

      [JsonProperty("price_group_quantities")]
      public string PriceGroupQuantities { get; set; }

      [JsonProperty("mileage_group_quantities")]
      public string MileageGroupQuantities { get; set; }

      [JsonProperty("page")]
      public Page Page { get; set; }

      [JsonProperty("filter_counts")]
      public FilterCounts FilterCounts { get; set; }

      [JsonProperty("cars_for_sale")]
      public IEnumerable<CarForSale> CarsForSale { get; set; }
    }

    protected class FilterCounts
    {
      [JsonProperty("make_model")]
      public IEnumerable<Feature> MakeModel { get; set; }

      [JsonProperty("city_mpg")]
      public IEnumerable<Feature> CityMpg { get; set; }

      [JsonProperty("highway_mpg")]
      public IEnumerable<Feature> HighwayMpg { get; set; }

      [JsonProperty("categories")]
      public IEnumerable<Feature> Categories { get; set; }

      [JsonProperty("years")]
      public IEnumerable<Feature> Years { get; set; }

      [JsonProperty("cylinders")]
      public IEnumerable<Feature> Cylinders { get; set; }

      [JsonProperty("drive_types")]
      public IEnumerable<Feature> DriveTypes { get; set; }

      [JsonProperty("fuel_types")]
      public IEnumerable<Feature> FuelTypes { get; set; }

      [JsonProperty("tranny_types")]
      public IEnumerable<Feature> TransmissionTypes { get; set; }

      [JsonProperty("options")]
      public IEnumerable<Feature> Options { get; set; }
    }

    protected class Feature
    {
      [JsonProperty("match_value")]
      public string MatchValue { get; set; }

      [JsonProperty("count")]
      public int Count { get; set; }
    }

    protected class CarForSale
    {
      [JsonProperty("id")]
      public int Id { get; set; }

      [JsonProperty("vin")]
      public string Vin { get; set; }

      [JsonProperty("make_id")]
      public int MakeId { get; set; }

      [JsonProperty("make")]
      public string Make { get; set; }

      [JsonProperty("model")]
      public string Model { get; set; }

      [JsonProperty("year")]
      public int Year { get; set; }

      [JsonProperty("is_new")]
      public int IsNewStatus { get; set; }

      [JsonProperty("mileage")]
      public string Mileage { get; set; }

      [JsonProperty("has_missing_mileage")]
      public bool HasMissingMileage { get; set; }

      [JsonProperty("city_mpg")]
      public string CityMpg { get; set; }

      [JsonProperty("highway_mpg")]
      public string HighwayMpg { get; set; }

      [JsonProperty("price")]
      public string AskingPrice { get; set; }

      [JsonProperty("has_missing_price")]
      public bool HasMissingPrice { get; set; }

      [JsonProperty("tranny_type")]
      public string TransmissionType { get; set; }

      [JsonProperty("drive_type")]
      public string DriveType { get; set; }

      [JsonProperty("cylinders")]
      public string Cylinders { get; set; }

      [JsonProperty("ext_color", NullValueHandling = NullValueHandling.Ignore)]
      public string ExteriorColor { get; set; }

      [JsonProperty("int_color", NullValueHandling = NullValueHandling.Ignore)]
      public string InteriorColor { get; set; }

      [JsonProperty("trim", NullValueHandling = NullValueHandling.Ignore)]
      public string Trim { get; set; }

      [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
      public string Details { get; set; }

      [JsonProperty("seller_notes", NullValueHandling = NullValueHandling.Ignore)]
      public string SellerNotes { get; set; }

      [JsonProperty("distance")]
      public int Distance { get; set; }

      [JsonProperty("primary_image_url")]
      public string PrimaryImageUrl { get; set; }

      [JsonProperty("image_urls")]
      public IEnumerable<string> ImageUrls { get; set; }

      [JsonProperty("dealer")]
      public Dealer Dealer { get; set; }

      [JsonProperty("dealerhours")]
      public DealerHours DealerHours { get; set; }
    }

    protected class Dealer
    {
      [JsonProperty("id")]
      public int Id { get; set; }

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("city")]
      public string City { get; set; }

      [JsonProperty("state")]
      public string State { get; set; }

      [JsonProperty("phone")]
      public string Phone { get; set; }

      [JsonProperty("is_trusted")]
      public bool IsTrusted { get; set; }

      [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
      public string Message { get; set; }

      [JsonProperty("latitude")]
      public string Latitude { get; set; }

      [JsonProperty("longitude")]
      public string Longitude { get; set; }

      [JsonProperty("autonationdealer")]
      public bool AutonationDealer { get; set; }
    }

    protected class DealerHours
    {
      [JsonProperty("salesmonopen")]
      public string SalesMonOpen { get; set; }

      [JsonProperty("salesmonclose")]
      public string SalesMonClose { get; set; }

      [JsonProperty("salestueopen")]
      public string SalesTueOpen { get; set; }

      [JsonProperty("salestueclose")]
      public string SalesTueClose { get; set; }

      [JsonProperty("saleswedopen")]
      public string SalesWedOpen { get; set; }

      [JsonProperty("saleswedclose")]
      public string SalesWedClose { get; set; }

      [JsonProperty("salesthropen")]
      public string SalesThrOpen { get; set; }

      [JsonProperty("salesthrclose")]
      public string SalesThrClose { get; set; }

      [JsonProperty("salesfriopen")]
      public string SalesFriOpen { get; set; }

      [JsonProperty("salesfriclose")]
      public string SalesFriClose { get; set; }

      [JsonProperty("salessatopen")]
      public string SalesSatOpen { get; set; }

      [JsonProperty("salessatclose")]
      public string SalesSatClose { get; set; }

      [JsonProperty("salessunopen")]
      public string SalesSunOpen { get; set; }

      [JsonProperty("salessunclose")]
      public string SalesSunClose { get; set; }
    }

    #endregion

  }
}
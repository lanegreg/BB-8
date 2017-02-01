using Car.Com.Common.Cacheability;
using Car.Com.Common.Crypto;
using Car.Com.Common.SiteMeta;
using Car.Com.Controllers.Filters;
using Car.Com.Domain.Services;
using Car.Com.Models.CarsForSale;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Car.Com.Domain.Models.Lead;

namespace Car.Com.Controllers
{
  [RoutePrefix("cars-for-sale")]
  public class CarsForSaleController : BaseController
  {
    /** 
     * This Controller's sole purpose is to handle all urls that belong to /cars-for-sale/ section, nothing more!
     **/



#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("", Name = "CarsForSale_Index"), HttpGet]
    public ActionResult Index()
    {
      const string assetsPrefix = "carsforsale.index";
      const int numberOfMakesToInitiallyShow = 36;
      
      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var makesTask = VehicleSpecService.GetAllMakesAsync();

      var topMakeNames = CarsForSaleService.GetTopMakesWithInventoryCount(numberOfMakesToInitiallyShow);
      var categories = CarsForSaleService.FilterDomains.Categories;
      
      makesTask.Wait();
      var makes = makesTask.Result;

      makes = makes.Where(m => !CarsForSaleService.MakesBlackList.Contains(m.Name)).ToList();


      var viewModel = new IndexViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        AdvertMeta = new AdvertMeta(),
        TrackMeta = new TrackMeta(metadata),
        Makes = makes.Select(ma => new IndexViewModel.Dto.Make
        {
          CanShow = topMakeNames.ContainsKey(ma.Name),
          MatchValue = ma.AbtMakeId.ToString(CultureInfo.InvariantCulture),
          Name = ma.Name,
          SeoName = ma.SeoName
        }),
        Categories = categories.Select(cat => new IndexViewModel.Dto.Category
        {
          MatchValue = cat.MatchValue,
          Name = cat.Description
        })
      };

      return View("Index", viewModel);
    }



#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("{makeSeoName:minlength(3):abt_makes}", Name = "CarsForSale_Make"), HttpGet]
    public ActionResult Make(string makeSeoName)
    {
      const string assetsPrefix = "carsforsale.make";

      var makeTranslator = UriTokenTranslators.GetMakeTranslatorBySeoName(makeSeoName);
      if (makeTranslator == null)
        return HttpNotFound();


      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var vspModelsTask = VehicleSpecService.GetModelsByMakeAsync(makeSeoName);
      // availableModels => represents models we actually have inventory for
      var availableModelsTask = CarsForSaleService.GetModelsDomainByMakeIdAsync(makeTranslator.AbtMakeId);

      vspModelsTask.Wait();
      availableModelsTask.Wait();

      var vspModels = vspModelsTask.Result;
      var availableModels = availableModelsTask.Result.Select(m => m.Description).ToList();

      var models = vspModels
        .Where(m => availableModels.Contains(m.Name))
        .Select(mo => new MakeViewModel.Dto.Model
        {
          MatchValue = String.Format("{0}~{1}", mo.AbtMakeId, mo.Name),
          Name = mo.Name,
          SeoName = mo.SeoName
        });

      var viewModel = new MakeViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name
        }),
        Make = new MakeViewModel.Dto.Make
        {
          Name = makeTranslator.Name, 
          SeoName = makeTranslator.SeoName
        },
        Models = models
      };

      return View("Make", viewModel);
    }



#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("results", Name = "CarsForSale_Results"), HttpGet]
    public ActionResult Results()
    {
      const string assetsPrefix = "carsforsale.results";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new ResultsViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        AdvertMeta = new AdvertMeta
        {
          delayAdsLoading  = true
        },
        TrackMeta = new TrackMeta(metadata)
        {
          DelayPageviewTracking = true
        },
        FirstLoad = true
      };

      //We need to register pageJson.inventory for current version of lead for popup on results page.
      // Values will be populated on click event
      viewModel.RegisterPageJson("{}", "ABT.pageJson.inventory");
      return View("Results", viewModel);
    }

    [Route("selectmodels", Name = "CarsForSale_SelectModels"), HttpGet]
    public ActionResult SelectModels(string q)
    {
      const string assetsPrefix = "carsforsale.selectmodels";
      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new SelectModelsViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
        },
        AdvertMeta = new AdvertMeta(new
        {
        }),
        OfferMeta = new OfferMeta(new
        {
        })
      };

      return View("SelectModels", viewModel);
    }

    [Route("selectcategorymakes", Name = "CarsForSale_SelectCategoryMakes"), HttpGet]
    public ActionResult SelectCategoryMakes(string q)
    {
      const string assetsPrefix = "carsforsale.selectcategorymakes";
      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new SelectCategoryMakesViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
        },
        AdvertMeta = new AdvertMeta(new
        {
        }),
        OfferMeta = new OfferMeta(new
        {
        })
      };

      return View("SelectCategoryMakes", viewModel);
    }

#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("vehicle-details", Name = "CarsForSale_VehicleDetails"), HttpGet]
    public ActionResult VehicleDetails(string q)
    {
      const string assetsPrefix = "carsforsale.vehicledetails";

      // If no query on querystring, redirect to /cars-for-sale/ landing page.
      q = q ?? String.Empty;
      var parts = q.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
      if (parts.Length < 2)
        return RedirectToRoute("CarsForSale_Index");
        
      var dealerId = Int32.Parse(parts[0]);
      var inventoryId = Int32.Parse(parts[1]);

      var carForSale = CarsForSaleService.GetCarForSaleByDealerIdByInventoryId(dealerId, inventoryId);

      // If inventory_id no longer exists, redirect to /cars-for-sale/results/ page.
      if (carForSale == null)
        return RedirectToRoute("CarsForSale_Results", new {q = String.Join("|", parts.Skip(2))});

      List<int> autonationDealers = LeadService.GetLeadAutonationUsedCarDealers();
      List<DealerHours> autonationDealerHours = LeadService.GetLeadAutonationUsedCarDealerHours(dealerId) as List<DealerHours>;
      DealerHours hours = autonationDealerHours.FirstOrDefault();

      const string na = "NA";
      string salesMonOpen = na, salesMonClose = na, salesTueOpen = na, salesTueClose = na, salesWedOpen = na,
        salesWedClose = na, salesThrOpen = na, salesThrClose = na, salesFriOpen = na, salesFriClose = na,
        salesSatOpen = na, salesSatClose = na, salesSunOpen = na, salesSunClose = na;

      if (autonationDealerHours !=null && autonationDealerHours.Any())
      {
        salesMonOpen = hours.SalesMonOpen.Length > 1 ? hours.SalesMonOpen : na;
        salesMonClose = hours.SalesMonClose.Length > 1 ? hours.SalesMonClose : na;
        salesTueOpen = hours.SalesTueOpen.Length > 1 ? hours.SalesTueOpen : na;
        salesTueClose = hours.SalesTueClose.Length > 1 ? hours.SalesTueClose : na;
        salesWedOpen = hours.SalesWedOpen.Length > 1 ? hours.SalesWedOpen : na;
        salesWedClose = hours.SalesWedClose.Length > 1 ? hours.SalesWedClose : na;
        salesThrOpen = hours.SalesThrOpen.Length > 1 ? hours.SalesThrOpen : na;
        salesThrClose = hours.SalesThrClose.Length > 1 ? hours.SalesThrClose : na;
        salesFriOpen = hours.SalesFriOpen.Length > 1 ? hours.SalesFriOpen : na;
        salesFriClose = hours.SalesFriClose.Length > 1 ? hours.SalesFriClose : na;
        salesSatOpen = hours.SalesSatOpen.Length > 1 ? hours.SalesSatOpen : na;
        salesSatClose = hours.SalesSatClose.Length > 1 ? hours.SalesSatClose : na;
        salesSunOpen = hours.SalesSunOpen != null && hours.SalesSunOpen.Length > 1 ? hours.SalesSunOpen : na;
        salesSunClose = hours.SalesSunClose != null && hours.SalesSunClose.Length > 1 ? hours.SalesSunClose : na;
      }
      
      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var viewModel = new VehicleDetailsViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = carForSale.Make,
          SuperModel = carForSale.Model,
          Year = carForSale.Year.ToString(CultureInfo.InvariantCulture)
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = carForSale.Make,
          model = carForSale.Model,
          year = carForSale.Year,
          category = carForSale.Category
        }),
        OfferMeta = new OfferMeta(new
        {
          make = carForSale.Make,
          model = carForSale.Model
        }),
        CarForSale = new VehicleDetailsViewModel.Dto.CarForSale
        {
          InventoryId = carForSale.Id,
          MakeId = carForSale.MakeId,
          Make = carForSale.Make,
          Model = carForSale.Model,
          Year = carForSale.Year,
          Trim = carForSale.Trim,
          Category = carForSale.Category,
          Mileage = carForSale.Mileage,
          HasMissingMileage = carForSale.Mileage <= 0,
          AskingPrice = carForSale.AskingPrice,
          HasMissingPrice = carForSale.AskingPrice <= 0,
          ExteriorColor = carForSale.ExteriorColor,
          InteriorColor = carForSale.InteriorColor,
          Cylinders = carForSale.Cylinders.ToString(CultureInfo.InvariantCulture),
          NumOfDoors = carForSale.NumOfDoors,
          TransmissionType = carForSale.TransmissionType,
          ImageUrls = carForSale.ImageUrls,
          Vin = carForSale.Vin,
          CityMpg = carForSale.CityMpg,
          HighwayMpg = carForSale.HighwayMpg,
          VehicleDetails = carForSale.VehicleDetails,
          SellerNotes = carForSale.SellerNotes,

          Dealer = new VehicleDetailsViewModel.Dto.Dealer
          {
            Name = carForSale.Dealer.Name,
            Phone = carForSale.Dealer.Phone,
            City = carForSale.Dealer.City,
            State = carForSale.Dealer.State,
            IsTrusted = carForSale.Dealer.IsPremiumPlacement,
            Message = carForSale.Dealer.Message,
            Latitude = carForSale.Dealer.Latitude,
            Longitude = carForSale.Dealer.Longitude,
            AutonationDealer = autonationDealers.Contains(carForSale.Dealer.Id)
          },

          DealerHours = new VehicleDetailsViewModel.Dto.DealerHours()
          {
            SalesMonOpen = salesMonOpen,
            SalesMonClose = salesMonClose,
            SalesTueOpen = salesTueOpen,
            SalesTueClose = salesTueClose,
            SalesWedOpen = salesWedOpen,
            SalesWedClose = salesWedClose,
            SalesThrOpen = salesThrOpen,
            SalesThrClose = salesThrClose,
            SalesFriOpen = salesFriOpen,
            SalesFriClose = salesFriClose,
            SalesSatOpen = salesSatOpen,
            SalesSatClose = salesSatClose,
            SalesSunOpen = salesSunOpen,
            SalesSunClose = salesSunClose
          }
        }
      };

      var primaryImage = viewModel.CarForSale.ImageUrls.FirstOrDefault();

      var inventoryJsonStr = JsonConvert.SerializeObject(new
      {
        id = inventoryId, 
        dealerId,
        city = viewModel.CarForSale.Dealer.City,
        state = viewModel.CarForSale.Dealer.State,
        vin = viewModel.CarForSale.Vin,
        price = viewModel.CarForSale.AskingPrice,
        mileage = viewModel.CarForSale.Mileage,
        hasAutocheck = carForSale.Dealer.HasAutoCheckId,
        year = viewModel.CarForSale.Year,
        dealerPhone = viewModel.CarForSale.Dealer.Phone,
        hasValidPhoneNum = viewModel.CarForSale.Dealer.HasValidPhoneNumber,
        makeId = viewModel.CarForSale.MakeId,
        make = viewModel.CarForSale.Make,
        model = viewModel.CarForSale.Model,
        primaryImage = primaryImage ?? String.Empty
      });

      viewModel.RegisterPageJson(inventoryJsonStr, "ABT.pageJson.inventory");

      if (autonationDealers.Contains(carForSale.Dealer.Id))
      {
        return View("VehicleDetailsBranded", viewModel);
      }

      return View("VehicleDetails", viewModel);
    }




    #region Services

    private static IVehicleSpecService VehicleSpecService
    {
      get { return ServiceLocator.Get<IVehicleSpecService>(); }
    }

    private static ICarsForSaleService CarsForSaleService
    {
      get { return ServiceLocator.Get<ICarsForSaleService>(); }
    }

    private static ILeadService LeadService
    {
      get { return ServiceLocator.Get<ILeadService>(); }
    }

    #endregion

  }
}
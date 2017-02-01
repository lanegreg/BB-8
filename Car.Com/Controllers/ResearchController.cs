using Car.Com.Common;
using Car.Com.Common.Cacheability;
using Car.Com.Common.SiteMeta;
using Car.Com.Controllers.Filters;
using Car.Com.Domain.Models.Translators;
using Car.Com.Domain.Services;
using Car.Com.Models.Research;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Car.Com.Controllers
{
  public class ResearchController : BaseController
  {

    [Route("car-research/", Name = "Research_Index"), HttpGet]
    public ActionResult Index()
    {
      const string assetsPrefix = "research.index";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var makesTask = VehicleSpecService.GetAllActiveMakesAsync();
      var categoriesTask = VehicleSpecService.GetAllCategoriesAsync();

      makesTask.Wait();
      categoriesTask.Wait();
      var makes = makesTask.Result;
      var categories = categoriesTask.Result;


      var viewModel = new IndexViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata),
        AdvertMeta = new AdvertMeta(new{}),
        Makes = makes,
        Categories = categories
      };

      return View("Index", viewModel);
    }


    #region Research Views

    [Route("alternative-fuel-vehicles/", Name = "Research_AlternativeFuel"), HttpGet]
    public ActionResult AlternativeFuel()
    {
      const string assetsPrefix = "research.alternativefuel";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var vehicleAttributesTask = VehicleSpecService.GetVehicleAttributesForAltFuelTrimsAsync();

      vehicleAttributesTask.Wait();

      var vehicleAttributes = vehicleAttributesTask.Result
        .Select(c => new AlternativeFuelViewModel.Dto.VehicleAttribute
        {
          Name = c.Name,
          SeoName = c.SeoName
        });

      var viewModel = new AlternativeFuelViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),

        VehicleAttributes = vehicleAttributes

      };

      return View("AlternativeFuel", viewModel);
    }

#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("{categorySeoName:minlength(3):categories}/", Name = "Research_Category"), HttpGet]
    public ActionResult Category(string categorySeoName)
    {
      const string assetsPrefix = "research.category";

      var categoryTranslator = UriTokenTranslators.GetCategoryTranslatorBySeoName(categorySeoName);
      if (categoryTranslator == null)
        return HttpNotFound();
      
      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var trimsTask = VehicleSpecService.GetTrimsByCategoryAsync(categoryTranslator.SeoName);
      trimsTask.Wait();

      var trims = trimsTask.Result
        .OrderBy(t => t.Make)
        .ThenBy(t => t.Model)
        .ThenBy(t => Convert.ToInt32(t.Msrp))
        .ThenBy(t => t.SuperTrim)
        .Where(t => (UriTokenTranslators.GetYearTranslatorByNumber(t.Year) != null) &&
          (UriTokenTranslators.GetMakeTranslatorBySeoName(t.MakeSeoName) != null) &&
          (UriTokenTranslators.GetNewSuperModelTranslatorBySeoName(t.SuperModel) != null) &&
          (UriTokenTranslators.GetTrimTranslatorBySeoName(t.SeoName) != null))
        .Select(t => new CategoryViewModel.Dto.Trim
        {
          Id = t.Id,
          Make = t.Make,
          MakeSeoName = t.MakeSeoName,
          Model = t.Model,
          Msrp = t.Msrp,
          Name = t.Name,
          SeoName = t.SeoName,
          SuperModel = t.SuperModel,
          SuperTrim = t.SuperTrim,
          Year = t.Year.ToString(CultureInfo.CurrentCulture),
          ImagePath = t.ImagePath,
          CityMpg = t.CityMpg,
          HighwayMpg = t.HighwayMpg,
          Horsepower = t.HorsePower,
          FullDisplayName = t.FullDisplayName,
          Seating = t.Seating
        });

      var filtersTask = VehicleSpecService.GetCategoryFilterGroupDataByCategoryAsync(categoryTranslator.SeoName);
      filtersTask.Wait();
      var categoryFilters = filtersTask.Result
        .Select(t => new CategoryViewModel.Dto.CategoryFilter
        {
          Code = t.Code.ToLower(),
          FilterGroupName = t.FilterGroupName.ToLower()
        });

      var trimList = trims as IList<CategoryViewModel.Dto.Trim> ?? trims.ToList();
      
      var viewModel = new CategoryViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Category = categoryTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          category = categoryTranslator.Name
        }),
        Category = new CategoryViewModel.Dto.Category
        {
          Name = categoryTranslator.Name,
          SeoName = categoryTranslator.SeoName
        },
        Trims = trimList,
        CategoryFilters = categoryFilters
      };

      return View("Category", viewModel);
    }


#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("{makeSeoName:minlength(3):makes}/", Name = "Research_Make"), HttpGet]
    public ActionResult Make(string makeSeoName)
    {
      const string assetsPrefix = "research.make";
      
      var makeTranslator = UriTokenTranslators.GetMakeTranslatorBySeoName(makeSeoName);
      if (makeTranslator == null)
        return HttpNotFound();
      
      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var superModelsTask = VehicleSpecService.GetNewSuperModelsByMakeAsync(makeSeoName);

      superModelsTask.Wait();
      var superModels = superModelsTask.Result
        .Where(sm => (UriTokenTranslators.GetMakeTranslatorByName(sm.Make) != null) &&
          (UriTokenTranslators.GetNewSuperModelTranslatorBySeoName(sm.SeoName) != null))
        .GroupBy(sm => new MakeViewModel.Dto.SuperModel
        {
          Name = sm.Name,
          SeoName = sm.SeoName,
          StartingMsrp = sm.StartingMsrp,
          FormattedStartingMsrp = sm.FormattedStartingMsrp
        }).Select(sm => sm.Key);

      var superModelList = superModels as IList<MakeViewModel.Dto.SuperModel> ?? superModels.ToList();
      var superModelListIndex = 0;
      var noTrimSuperModelList = new List<int>();
      foreach (var superModel in superModelList)
      {
        try
        {
          var trimsTask = VehicleSpecService.GetNewTrimsByMakeBySuperModelAsync(makeTranslator.SeoName, superModel.SeoName);
          trimsTask.Wait();

          //we want to try and show the most expensive vehicle trim image for the make if possible, 
          //so we get a list of them for this superModel...
          var trims = trimsTask.Result.OrderByDescending(t => t.Msrp);

          //taren has requested that we do not show supermodels with no trims on the make view, 
          //but we don't want to remove the trims from the tier1 cache, so we will just remove them from the viewmodel
          if (!trims.ToList().Any()) { noTrimSuperModelList.Add(superModelListIndex); }

          foreach (var trim in trims)
          {
            var trimImageTask = ImageMetaService.GetImagesByTrimIdAsync(trim.Id);
            trimImageTask.Wait();

            var trimImage = trimImageTask.Result.FirstOrDefault(); //initializes the variable with the correct object type

            //here we will try to update to the best trimImage we can for the make in steps...
            try { trimImage = trimImageTask.Result.First(i => i.CategoryView == "Exterior Standard" && i.View == "Three-Quarters View"); }
            catch (InvalidOperationException) //Sequence contains no matching element
            {
              try { trimImage = trimImageTask.Result.First(i => i.UrlPrefix != null && i.CategoryView == "Exterior Standard"); }
              catch (InvalidOperationException) //Sequence contains no matching element
              {
                try { trimImage = trimImageTask.Result.First(i => i.UrlPrefix != null); }
                catch (InvalidOperationException) //Sequence contains no matching element
                {
                  trimImage = trimImageTask.Result.FirstOrDefault();
                }
              }
            }

            if (trimImage != null) //we found an image, let's move on
            {
              superModel.ImagePath = trimImage.UrlPrefix;
              break;
            }
          }
        }
        catch (AggregateException) { superModel.ImagePath = ""; }
        superModelListIndex++;
      }

      var sortedSuperModels2BeRemoved = noTrimSuperModelList.OrderByDescending(i => i);
      foreach (var i in sortedSuperModels2BeRemoved)
      {
        superModelList.RemoveAt(i);
      }

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
          make = makeTranslator.Name,
          makeId = makeTranslator.Id
        }),
        SuperModels = superModelList,
        Make = new MakeViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        }
      };

      return View("Make", viewModel);
    }


#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("{makeSeoName:minlength(3):makes}/{superModelSeoName:minlength(1):new_super_models}/", 
      Name = "Research_SuperModel"), HttpGet]
    public ActionResult SuperModel(string makeSeoName, string superModelSeoName)
    {
      const string assetsPrefix = "research.supermodel";

      var makeTranslator = UriTokenTranslators.GetMakeTranslatorBySeoName(makeSeoName);
      var superModelTranslator = UriTokenTranslators.GetNewSuperModelTranslatorBySeoName(superModelSeoName);
      if (makeTranslator == null || superModelTranslator == null)
        return HttpNotFound();

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var trimsTask = VehicleSpecService.GetNewTrimsByMakeBySuperModelAsync(makeTranslator.SeoName, superModelTranslator.SeoName);
      var filtersTask = VehicleSpecService.GetSuperModelFilterGroupDataByMakeSuperModelAsync(makeTranslator.SeoName, superModelTranslator.SeoName);
      var newSuperModelsTask = VehicleSpecService.GetAllNewSuperModelsAsync();
      var oldSuperModelsTask = VehicleSpecService.GetAllUsedSuperModelsAsync();

      newSuperModelsTask.Wait();
      var newSuperModelYears = newSuperModelsTask.Result
        .Where(t => t.Make.ToLower() == makeSeoName && t.SeoName == superModelSeoName)
        .Select(t => new SuperModelViewModel.Dto.SuperModelYearsJson
        {
          YearsJson = t.YearsJson
        });

      oldSuperModelsTask.Wait();
      var oldSuperModelYears = oldSuperModelsTask.Result
        .Where(t => t.Make.ToLower() == makeSeoName && t.SeoName == superModelSeoName)
        .Select(t => new SuperModelViewModel.Dto.SuperModelYearsJson
        {
          YearsJson = t.YearsJson
        });

      string superModelYears = "";

      var superModelYearJsons = newSuperModelYears as SuperModelViewModel.Dto.SuperModelYearsJson[] ?? newSuperModelYears.ToArray();
      var olderModelYearJsons = oldSuperModelYears as SuperModelViewModel.Dto.SuperModelYearsJson[] ?? oldSuperModelYears.ToArray();
      if (superModelYearJsons.Count() == 1)
      {
        superModelYears = superModelYearJsons[0].YearsJson;
      }
      else if (olderModelYearJsons.Count() == 1)
      {
        superModelYears = olderModelYearJsons[0].YearsJson;
      }

      var js = new JavaScriptSerializer();
      var superModelYearItems = js.Deserialize<SuperModelViewModel.Dto.SuperModelYearItem[]>("[" + superModelYears.TrimEnd(',') + "]");

      filtersTask.Wait();
      var superModelFilters = filtersTask.Result
        .Select(t => new SuperModelViewModel.Dto.SuperModelFilter
        {
          Code = t.Code.ToLower(),
          FilterGroupName = t.FilterGroupName.ToLower()
        });

      trimsTask.Wait();
      var trims = trimsTask.Result
        .OrderBy(t => t.SuperTrim)
        .ThenBy(t => t.Year)
        .ThenBy(t => Convert.ToInt32(t.Msrp))
        .ThenBy(t => t.Name)
        .Where(t => (UriTokenTranslators.GetYearTranslatorByNumber(t.Year) != null) &&
          (UriTokenTranslators.GetMakeTranslatorByName(t.Make) != null) &&
          (UriTokenTranslators.GetNewSuperModelTranslatorByName(t.SuperModel) != null) &&
          (UriTokenTranslators.GetTrimTranslatorBySeoName(t.SeoName) != null))
        .Select(t => new SuperModelViewModel.Dto.Trim
        {
          Id = t.Id,
          Model = t.Model,
          Msrp = t.Msrp,
          Name = t.Name,
          SeoName = t.SeoName,
          SuperTrim = t.SuperTrim,
          Year = t.Year.ToString(CultureInfo.CurrentCulture),
          ImagePath = t.ImagePath,
          CityMpg = t.CityMpg,
          HighwayMpg = t.HighwayMpg,
          Horsepower = t.HorsePower,
          FullDisplayName = t.FullDisplayName,
          Seating = t.Seating
        });

      var trimList = trims as IList<SuperModelViewModel.Dto.Trim> ?? trims.ToList();
      
      var viewModel = new SuperModelViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name,
          SuperModel = superModelTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name,
          model = superModelTranslator.Name,
          makeseoname = makeTranslator.SeoName,
          supermodelseoname = superModelTranslator.SeoName
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name,
          makeId = makeTranslator.Id,
          model = superModelTranslator.Name
        }),
        Make = new SuperModelViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        },
        SuperModel = new SuperModelViewModel.Dto.SuperModel
        {
          Name = superModelTranslator.Name,
          SeoName = superModelTranslator.SeoName
        },
        Trims = trimList,
        SuperModelFilters = superModelFilters,
        SuperModelYearItems = superModelYearItems
      };

      return View("SuperModel", viewModel);
    }

    [Route("{vehicleAttributeSeoName:minlength(3):vehicle_attributes}/OLD", Name = "Research_VehicleAttribute"), HttpGet]
    public ActionResult VehicleAttribute(string vehicleAttributeSeoName)
    {
      //handles choosing luxury, sport, or compact from the research category root view,
      //or a fueltype icon from the AlternativeFuel view when we were using a 2nd landing page.
      //Not going to rip it out yet until the business settles down on changes and exactly what they want :-)
 
      const string assetsPrefix = "research.vehicleattribute";

      var vehicleAttributeTranslator = UriTokenTranslators.GetVehicleAttributeTranslatorBySeoName(vehicleAttributeSeoName);
      if (vehicleAttributeTranslator == null)
        return HttpNotFound();

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var categoriesTask = VehicleSpecService.GetCategoriesByVehicleAttributeSeoNameAsync(vehicleAttributeTranslator.SeoName);

      categoriesTask.Wait();

      var categories = categoriesTask.Result
        .Select(c => new VehicleAttributeViewModel.Dto.Category
        {
          Name = c.Name,
          SeoName = c.SeoName
        });

      var vehicleAttribute = new VehicleAttributeViewModel.Dto.VehicleAttribute
      {
        Name = vehicleAttributeTranslator.Name,
        SeoName = vehicleAttributeTranslator.SeoName
      };

      var viewModel = new VehicleAttributeViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata),
        AdvertMeta = new AdvertMeta(new
        {
          vehicleattribute = vehicleAttributeTranslator.Name
        }),
        VehicleAttribute = vehicleAttribute,
        Categories = categories
      };

      return View("VehicleAttribute", viewModel);
    }

#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("{vehicleAttributeSeoName:minlength(3):vehicle_attributes}/{categorySeoName:minlength(3):categories}/", 
      Name = "Research_VehicleAttributeResult"), HttpGet]
    public ActionResult VehicleAttributeResult(string categorySeoName, string vehicleAttributeSeoName)
    {
      const string assetsPrefix = "research.vehicleattributeresult";

      var vehicleAttributeTranslator = UriTokenTranslators.GetVehicleAttributeTranslatorBySeoName(vehicleAttributeSeoName);
      var categoryTranslator = UriTokenTranslators.GetCategoryTranslatorBySeoName(categorySeoName);
      if (vehicleAttributeTranslator == null || categoryTranslator == null)
        return HttpNotFound();

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var trimsTask = VehicleSpecService.GetTrimsByCategoryAndVehicleAttributeNameAsync(categoryTranslator.SeoName, vehicleAttributeTranslator.SeoName);
      trimsTask.Wait();
      var trims = trimsTask.Result
        .OrderBy(t => t.Make)
        .ThenBy(t => t.Model)
        .ThenBy(t => Convert.ToInt32(t.Msrp))
        .ThenBy(t => t.SuperTrim)
        .Where(t => (UriTokenTranslators.GetYearTranslatorByNumber(t.Year) != null) &&
          (UriTokenTranslators.GetMakeTranslatorBySeoName(t.MakeSeoName) != null) &&
          (UriTokenTranslators.GetNewSuperModelTranslatorBySeoName(t.SuperModel) != null) &&
          (UriTokenTranslators.GetTrimTranslatorBySeoName(t.SeoName) != null))
        .Select(t => new VehicleAttributeViewModel.Dto.Trim
        {
          Id = t.Id,
          Make = t.Make,
          MakeSeoName = t.MakeSeoName,
          Model = t.Model,
          Msrp = t.Msrp,
          Name = t.Name,
          SeoName = t.SeoName,
          SuperModel = t.SuperModel,
          SuperTrim = t.SuperTrim,
          Year = t.Year.ToString(CultureInfo.CurrentCulture),
          ImagePath = t.ImagePath,
          CityMpg = t.CityMpg,
          HighwayMpg = t.HighwayMpg,
          Horsepower = t.HorsePower,
          FullDisplayName = t.FullDisplayName,
          Seating = t.Seating
        });

      var filtersTask = VehicleSpecService.GetCustomFilterGroupDataByCategoryAndVehicleAttributeNameAsync(categoryTranslator.SeoName, vehicleAttributeTranslator.SeoName);
      filtersTask.Wait();
      var categoryFilters = filtersTask.Result
        .Select(t => new VehicleAttributeViewModel.Dto.CategoryFilter
        {
          Code = t.Code.ToLower(),
          FilterGroupName = t.FilterGroupName.ToLower()
        });

      var trimList = trims as IList<VehicleAttributeViewModel.Dto.Trim> ?? trims.ToList();
      
      var vehicleAttribute = new VehicleAttributeViewModel.Dto.VehicleAttribute
      {
        Name = vehicleAttributeTranslator.Name,
        SeoName = vehicleAttributeTranslator.SeoName
      };

      var viewModel = new VehicleAttributeViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Category = categoryTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          category = categoryTranslator.Name,
          vehicleattributename = vehicleAttributeTranslator.Name,
          vehicleattributeseoname = vehicleAttributeTranslator.SeoName
        }),
        Category = new VehicleAttributeViewModel.Dto.Category
        {
          Name = categoryTranslator.Name,
          SeoName = categoryTranslator.SeoName
        },
        Trims = trimList,
        CategoryFilters = categoryFilters,
        VehicleAttribute = vehicleAttribute
      };

      return View("VehicleAttributeResult", viewModel);
    }

#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("{vehicleAttributeSeoName:minlength(3):vehicle_attributes}/", Name = "Research_VehicleAttributeNoCategoryResult"), HttpGet]
    public ActionResult VehicleAttributeNoCategoryResult(string vehicleAttributeSeoName)
    {
      const string assetsPrefix = "research.vehicleattributenocategoryresult";

      var vehicleAttributeTranslator = UriTokenTranslators.GetVehicleAttributeTranslatorBySeoName(vehicleAttributeSeoName);
      if (vehicleAttributeTranslator == null)
        return HttpNotFound();

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var trimsTask = VehicleSpecService.GetTrimsByVehicleAttributeNameAsync(vehicleAttributeTranslator.SeoName);
      trimsTask.Wait();
      var trims = trimsTask.Result
        .OrderBy(t => t.Make)
        .ThenBy(t => t.Model)
        .ThenBy(t => Convert.ToInt32(t.Msrp))
        .ThenBy(t => t.SuperTrim)
        .Where(t => (UriTokenTranslators.GetYearTranslatorByNumber(t.Year) != null) &&
          (UriTokenTranslators.GetMakeTranslatorBySeoName(t.MakeSeoName) != null) &&
          (UriTokenTranslators.GetNewSuperModelTranslatorBySeoName(t.SuperModel) != null) &&
          (UriTokenTranslators.GetTrimTranslatorBySeoName(t.SeoName) != null))
        .Select(t => new VehicleAttributeNoCategoryViewModel.Dto.Trim
        {
          Id = t.Id,
          Make = t.Make,
          MakeSeoName = t.MakeSeoName,
          Model = t.Model,
          Msrp = t.Msrp,
          Name = t.Name,
          SeoName = t.SeoName,
          SuperModel = t.SuperModel,
          SuperTrim = t.SuperTrim,
          Year = t.Year.ToString(CultureInfo.CurrentCulture),
          ImagePath = t.ImagePath,
          CityMpg = t.CityMpg,
          HighwayMpg = t.HighwayMpg,
          Horsepower = t.HorsePower,
          FullDisplayName = t.FullDisplayName,
          Seating = t.Seating
        });

      var filtersTask = VehicleSpecService.GetCustomFilterGroupDataByVehicleAttributeNameAsync(vehicleAttributeTranslator.SeoName);
      filtersTask.Wait();
      var categoryFilters = filtersTask.Result
        .Select(t => new VehicleAttributeNoCategoryViewModel.Dto.CategoryFilter
        {
          Code = t.Code.ToLower(),
          FilterGroupName = t.FilterGroupName.ToLower()
        });

      var trimList = trims as IList<VehicleAttributeNoCategoryViewModel.Dto.Trim> ?? trims.ToList();

      var vehicleAttribute = new VehicleAttributeNoCategoryViewModel.Dto.VehicleAttribute
      {
        Name = vehicleAttributeTranslator.Name,
        SeoName = vehicleAttributeTranslator.SeoName
      };

      var viewModel = new VehicleAttributeNoCategoryViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Category = vehicleAttributeTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          category = vehicleAttributeTranslator.Name,
          vehicleattributename = vehicleAttributeTranslator.Name,
          vehicleattributeseoname = vehicleAttributeTranslator.SeoName
        }),
        Category = new VehicleAttributeNoCategoryViewModel.Dto.Category
        {
          Name = vehicleAttributeTranslator.Name,
          SeoName = vehicleAttributeTranslator.SeoName
        },
        Trims = trimList,
        CategoryFilters = categoryFilters,
        VehicleAttribute = vehicleAttribute
      };

      return View("VehicleAttributeNoCategoryResult", viewModel);
    }

#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Medium)]
#endif
    [Route("{makeSeoName:minlength(3):makes}/{superModelSeoName:minlength(1):super_models}/{yearNumber:int:research_years}/",
      Name = "Research_Year"), HttpGet]
    public ActionResult Year(string makeSeoName, string superModelSeoName, int yearNumber)
    {
      const string assetsPrefix = "research.year";

      var makeTranslator = UriTokenTranslators.GetMakeTranslatorBySeoName(makeSeoName);
      var superModelTranslator = UriTokenTranslators.GetNewSuperModelTranslatorBySeoName(superModelSeoName);
      var yearTranslator = UriTokenTranslators.GetYearTranslatorByNumber(yearNumber);
      if (makeTranslator == null || superModelTranslator == null || yearTranslator == null)
        return HttpNotFound();
      
      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var trimsTask = VehicleSpecService.GetTrimsByMakeBySuperModelByYearAsync(makeTranslator.SeoName, superModelTranslator.SeoName, yearTranslator.Number);
      var filtersTask = VehicleSpecService.GetSuperModelFilterGroupDataByMakeSuperModelByYearAsync(makeTranslator.SeoName, superModelTranslator.SeoName, yearTranslator.Number);

      var newSuperModelsTask = VehicleSpecService.GetAllNewSuperModelsAsync();
      newSuperModelsTask.Wait();
      var newSuperModelYears = newSuperModelsTask.Result
        .Where(t => t.Make.ToLower() == makeSeoName && t.SeoName == superModelSeoName)
        .Select(t => new YearViewModel.Dto.YearSupermodelYearsJson
        {
          YearsJson = t.YearsJson
        });
      
      var oldSuperModelsTask = VehicleSpecService.GetAllUsedSuperModelsAsync();
      oldSuperModelsTask.Wait();
      var oldSuperModelYears = oldSuperModelsTask.Result
        .Where(t => t.Make.ToLower() == makeSeoName && t.SeoName == superModelSeoName)
        .Select(t => new YearViewModel.Dto.YearSupermodelYearsJson
        {
          YearsJson = t.YearsJson
        });

      string superModelYears = "";

      var superModelYearJsons = newSuperModelYears as YearViewModel.Dto.YearSupermodelYearsJson[] ?? newSuperModelYears.ToArray();
      var olderModelYearJsons = oldSuperModelYears as YearViewModel.Dto.YearSupermodelYearsJson[] ?? oldSuperModelYears.ToArray();
      if (superModelYearJsons.Count() == 1)
      {
        superModelYears = superModelYearJsons[0].YearsJson;
      }
      else if (olderModelYearJsons.Count() == 1)
      {
        superModelYears = olderModelYearJsons[0].YearsJson;
      }

      var js = new JavaScriptSerializer();
      var superModelYearItems = js.Deserialize<YearViewModel.Dto.YearSupermodelYearItem[]>("[" + superModelYears.TrimEnd(',') + "]");

      filtersTask.Wait();
      var yearFilters = filtersTask.Result
        .Select(t => new YearViewModel.Dto.YearFilter
        {
          Code = t.Code.ToLower(),
          FilterGroupName = t.FilterGroupName.ToLower()
        });

      trimsTask.Wait();
      var trims = trimsTask.Result
        .OrderBy(t => t.SuperTrim)
        .ThenBy(t => t.Year)
        .ThenBy(t => Convert.ToInt32(t.Msrp))
        .ThenBy(t => t.Name)
        .Where(t => (UriTokenTranslators.GetYearTranslatorByNumber(t.Year) != null) &&
          (UriTokenTranslators.GetMakeTranslatorByName(t.Make) != null) &&
          (UriTokenTranslators.GetNewSuperModelTranslatorByName(t.SuperModel) != null) &&
          (UriTokenTranslators.GetTrimTranslatorBySeoName(t.SeoName) != null))
        .Select(t => new YearViewModel.Dto.Trim
        {
          Id = t.Id,
          Model = t.Model,
          Msrp = t.Msrp,
          Name = t.Name,
          SeoName = t.SeoName,
          SuperTrim = t.SuperTrim,
          Year = t.Year.ToString(CultureInfo.CurrentCulture),
          ImagePath = t.ImagePath,
          CityMpg = t.CityMpg,
          HighwayMpg = t.HighwayMpg,
          Horsepower = t.HorsePower,
          FullDisplayName = t.FullDisplayName,
          Seating = t.Seating
        });

      var trimList = trims as IList<YearViewModel.Dto.Trim> ?? trims.ToList();

      var viewModel = new YearViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name,
          SuperModel = superModelTranslator.Name,
          Year = yearTranslator.Number.ToString()
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          makeseoname = makeTranslator.SeoName,
          supermodelseoname = superModelTranslator.SeoName
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name,
          makeId = makeTranslator.Id,
          model = superModelTranslator.Name,
          year = yearTranslator.Number
        }),
        Make = new YearViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        },
        SuperModel = new YearViewModel.Dto.SuperModel
        {
          Name = superModelTranslator.Name,
          SeoName = superModelTranslator.SeoName
        },
        Trims = trimList,
        YearFilters = yearFilters,
        YearSupermodelYearItems = superModelYearItems
      };
      
      return View("Year", viewModel);
    }

    #endregion


    #region Trim Views

    [Route("{makeSeoName:minlength(3):makes}/{superModelSeoName:minlength(1):super_models}/{yearNumber:int:research_years}/{trimSeoName:minlength(1):trims}/{trimSectionSeoName:trim_sections?}",
      Name = "Research_Trim"), HttpGet]
    public ActionResult Trim(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName, string trimSectionSeoName)
    {
      var makeTranslator = UriTokenTranslators.GetMakeTranslatorBySeoName(makeSeoName);
      var superModelTranslator = UriTokenTranslators.GetNewSuperModelTranslatorBySeoName(superModelSeoName);
      var yearTranslator = UriTokenTranslators.GetYearTranslatorByNumber(yearNumber);
      var trimTranslator = UriTokenTranslators.GetTrimTranslatorBySeoName(trimSeoName);
      if (makeTranslator == null || superModelTranslator == null || yearTranslator == null || trimTranslator == null)
        return HttpNotFound();

      switch (trimSectionSeoName)
      {
        case "specifications":
          return TrimSpecs(makeTranslator, superModelTranslator, yearTranslator, trimTranslator);
        case "warranty":
          return TrimWarranty(makeTranslator, superModelTranslator, yearTranslator, trimTranslator);
        case "pictures-videos":
          return TrimPicsnVids(makeTranslator, superModelTranslator, yearTranslator, trimTranslator);
        case "safety":
          return TrimSafety(makeTranslator, superModelTranslator, yearTranslator, trimTranslator);
        case "color":
          return TrimColor(makeTranslator, superModelTranslator, yearTranslator, trimTranslator);
        case "incentives":
          return TrimIncentives(makeTranslator, superModelTranslator, yearTranslator, trimTranslator);
        default:
          return TrimOverview(makeTranslator, superModelTranslator, yearTranslator, trimTranslator);
      }

    }

    private ActionResult TrimOverview(IMakeTranslator makeTranslator, ISuperModelTranslator superModelTranslator, IYearTranslator yearTranslator, ITrimTranslator trimTranslator)
    {
      const string assetsPrefix = "research.trimoverview";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var trimTask = VehicleSpecService
        .GetTrimOverviewByMakeBySuperModelByYearByTrimAsync(makeTranslator.SeoName, superModelTranslator.SeoName, yearTranslator.Number, trimTranslator.SeoName);

      trimTask.Wait();

      var trim = trimTask.Result;
      var trimImageTask = ImageMetaService.GetImagesByTrimIdAsync(trim.Id);

      trimImageTask.Wait();
      var trimImage = trimImageTask.Result.FirstOrDefault();

      
      var viewModel = new TrimOverviewViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name,
          SuperModel = superModelTranslator.Name,
          Year = yearTranslator.Number.ToString(),
          Trim = trimTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.Name,
          fuel = trim.FuelType,
          category = UriTokenTranslators.GetCategoryTranslatorById(trim.CategoryId).Name,
          bodystyle = trim.BodyStyle
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name,
          makeId = makeTranslator.Id,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.SeoName
        }),
        TrimSectionSeoName = "overview",
        MainImage = trimImage != null ? trimImage.UrlPrefix : "",
        Make = new TrimOverviewViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        },
        SuperModel = new TrimOverviewViewModel.Dto.SuperModel
        {
          Name = superModelTranslator.Name,
          SeoName = superModelTranslator.SeoName
        },
        Year = yearTranslator.Number,
        Trim = new TrimOverviewViewModel.Dto.Trim()
        {
          TrimId = trim.Id,
          Name = trimTranslator.Name,
          SeoName = trimTranslator.SeoName,
          Year = yearTranslator.Number,
          FullDisplayName = trim.FullDisplayName,
          Model = trim.Model,
          SuperTrim = trim.SuperTrim,
          Msrp = Convert.ToDouble(trim.Msrp),
          Invoice = Convert.ToDouble(trim.Invoice),
          CityMpg = trim.CityMpg,
          HwyMpg = trim.HighwayMpg,
          Doors = trim.Doors,
          PrimaryCategoryId = trim.CategoryId,
          Seating = trim.Seating,
          DriveTrain = trim.Drivetrain,
          EngineType = trim.EngineType,
          EngineSize = trim.EngineSize,
          BodyStyle = trim.BodyStyle,
          FuelType = trim.FuelType,
          DriveRange = trim.DriveRange,
          TowingCapacity = trim.TowingCapacity,
          CargoCapacity = trim.CargoCapacity,
          BedLength = trim.BedLength,
          HorsePower = trim.HorsePower,
          RoofType = trim.RoofType,
          Time0To60 = trim.Time0To60,
          Overview = trim.Overview
        }
      };

      if (trimImage != null)
      {
        viewModel.Images = new TrimOverviewViewModel.Dto.Image
            {
              Small = String.Format("{0}_320x.png", trimImage.UrlPrefix),
              Medium = String.Format("{0}_640x.png", trimImage.UrlPrefix),
              Large = String.Format("{0}_1024x.png", trimImage.UrlPrefix),
              ImageId = String.Format("PnVSlider_{0}", trimImage.Id)
            };
      }
      else
      {
        viewModel.Images = new TrimOverviewViewModel.Dto.Image
        {
          Small = "/assets/svg/no-image-avail.svg",
          Medium = "/assets/svg/no-image-avail.svg",
          Large = "/assets/svg/no-image-avail.svg",
          ImageId = "PnVSlider_0"
        };
      }

      #region This handles the visual data points per template.
      switch (trim.CategoryId)
      {
        case 1:
          viewModel.Trim.Template = "_TrimOverviewAltFuel";
          break;
        case 2:
          viewModel.Trim.Template = "_TrimOverviewCompact";
          break;
        case 3:
          viewModel.Trim.Template = "_TrimOverviewConvertible";
          break;
        case 4:
          viewModel.Trim.Template = "_TrimOverviewSedan";
          break;
        case 5:
          viewModel.Trim.Template = "_TrimOverviewSport";
          break;
        case 6:
          viewModel.Trim.Template = "_TrimOverviewSuv";
          break;
        case 7:
          viewModel.Trim.Template = "_TrimOverviewTruck";
          break;
        case 8:
          viewModel.Trim.Template = "_TrimOverviewVan";
          break;
        case 9:
          viewModel.Trim.Template = "_TrimOverviewWagon";
          break;
        default:
          viewModel.Trim.Template = "_TrimOverviewWagon";
          break;
      }
      #endregion

      if (trim.CanonicalSeoName.IsNotNullOrEmpty() && trim.CanonicalSeoName != trim.SeoName)
      {
        viewModel.PageMeta.Canonical = viewModel.PageMeta.Canonical.Replace(trim.SeoName, trim.CanonicalSeoName);
      }

      var leadformJsonStr = JsonConvert.SerializeObject(new
      {
        quoteButtonSelected = false, year = yearTranslator.Number, make = makeTranslator.SeoName, supermodel = superModelTranslator.SeoName, trim = trimTranslator.SeoName
      });
      viewModel.RegisterPageJson(leadformJsonStr, "ABT.pageJson.getaquote");
      var vehicleInfoJsonStr = JsonConvert.SerializeObject(new
      {
        make = makeTranslator.Name,
        model = superModelTranslator.Name,
        trimid = trim.Id
      });
      viewModel.RegisterPageJson(vehicleInfoJsonStr, "ABT.pageJson.vehicleinfo");

      return View("TrimOverview", viewModel);
    }

    private ActionResult TrimSpecs(IMakeTranslator makeTranslator, ISuperModelTranslator superModelTranslator, IYearTranslator yearTranslator, ITrimTranslator trimTranslator)
    {
      const string assetsPrefix = "research.trimspecs";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var trimTask = VehicleSpecService
        .GetTrimSpecsByMakeBySuperModelByYearByTrimAsync(makeTranslator.SeoName, superModelTranslator.SeoName, yearTranslator.Number, trimTranslator.SeoName);
      
      trimTask.Wait();
      var trim = trimTask.Result;

      var viewModel = new TrimSpecsViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name,
          SuperModel = superModelTranslator.Name,
          Year = yearTranslator.Number.ToString(),
          Trim = trimTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.Name,
          fuel = trim.FuelType,
          category = UriTokenTranslators.GetCategoryTranslatorById(trim.CategoryId).Name,
          bodystyle = trim.BodyStyle
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name,
          makeId = makeTranslator.Id,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.SeoName
        }),
        TrimSectionSeoName = "specifications",
        Make = new TrimSpecsViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        },
        SuperModel = new TrimSpecsViewModel.Dto.SuperModel
        {
          Name = superModelTranslator.Name,
          SeoName = superModelTranslator.SeoName
        },
        Year = yearTranslator.Number,
        Trim = new TrimSpecsViewModel.Dto.Trim()
        {
          Name = trimTranslator.Name,
          SeoName = trimTranslator.SeoName,
          Year = yearTranslator.Number,
          FullDisplayName = trim.FullDisplayName,
          Model = trim.Model,
          SuperTrim = trim.SuperTrim,
          Msrp = trim.Msrp
        },
        StandardSpecs = trim.Specifications
          .OrderBy(t => t.Group)
          .ThenBy(t => t.Title)
          .Select(t => new TrimSpecsViewModel.Dto.Specifications
          {
            Group = t.Group,
            Title = t.Title,
            Data = t.Data,
            Availability = t.Availability
          }),
        StandardFeatures = trim.OptionalSpecs
          .Where(t => t.Availability != "o")
          .OrderBy(t => t.Group)
          .ThenBy(t => t.Title)
          .Select(t => new TrimSpecsViewModel.Dto.Specifications
          {
            Group = t.Group,
            Title = t.Title,
            Data = t.Data,
            Availability = t.Availability
          }),
        OptionalFeatures = trim.OptionalSpecs
          .Where(t => t.Availability == "o")
          .OrderBy(t => t.Group)
          .ThenBy(t => t.Title)
          .Select(t => new TrimSpecsViewModel.Dto.Specifications
          {
            Group = t.Group,
            Title = t.Title,
            Data = t.Data,
            Availability = t.Availability
          })
      };

      if (trim.CanonicalSeoName.IsNotNullOrEmpty() && trim.CanonicalSeoName != trim.SeoName)
      {
        viewModel.PageMeta.Canonical = viewModel.PageMeta.Canonical.Replace(trim.SeoName, trim.CanonicalSeoName);
      }

      var vehicleInfoJsonStr = JsonConvert.SerializeObject(new
      {
        make = makeTranslator.Name,
        model = superModelTranslator.Name,
        trimid = trim.Id
      });
      viewModel.RegisterPageJson(vehicleInfoJsonStr, "ABT.pageJson.vehicleinfo");

      return View("TrimSpecs", viewModel);

    }

    private ActionResult TrimWarranty(IMakeTranslator makeTranslator, ISuperModelTranslator superModelTranslator, IYearTranslator yearTranslator, ITrimTranslator trimTranslator)
    {
      const string assetsPrefix = "research.trimwarranty";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var trimTask = VehicleSpecService
        .GetTrimSpecsByMakeBySuperModelByYearByTrimAsync(makeTranslator.SeoName, superModelTranslator.SeoName, yearTranslator.Number, trimTranslator.SeoName);

      trimTask.Wait();
      var trim = trimTask.Result;
      
      var viewModel = new TrimWarrantyViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name,
          SuperModel = superModelTranslator.Name,
          Year = yearTranslator.Number.ToString(),
          Trim = trimTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.Name,
          fuel = trim.FuelType,
          category = UriTokenTranslators.GetCategoryTranslatorById(trim.CategoryId).Name,
          bodystyle = trim.BodyStyle
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name,
          makeId = makeTranslator.Id,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.SeoName
        }),
        TrimSectionSeoName = "warranty",
        Make = new TrimWarrantyViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        },
        SuperModel = new TrimWarrantyViewModel.Dto.SuperModel
        {
          Name = superModelTranslator.Name,
          SeoName = superModelTranslator.SeoName
        },
        Year = yearTranslator.Number,
        Trim = new TrimWarrantyViewModel.Dto.Trim()
        {
          Name = trimTranslator.Name,
          SeoName = trimTranslator.SeoName,
          Year = yearTranslator.Number,
          FullDisplayName = trim.FullDisplayName,
          Model = trim.Model,
          SuperTrim = trim.SuperTrim,
          Msrp = trim.Msrp,
        },
        WarrantyItems = trim.OptionalSpecs
          .Where(t => t.Group == "Warranty")
          .Where(t => t.Availability != "o")
          .OrderBy(t => t.Group)
          .ThenBy(t => t.Title)
          .Select(t => new TrimWarrantyViewModel.Dto.Specifications
          {
            Group = t.Group,
            Title = t.Title,
            Data = t.Data,
            Availability = t.Availability
          })
      };

      if (trim.CanonicalSeoName.IsNotNullOrEmpty() && trim.CanonicalSeoName != trim.SeoName)
      {
        viewModel.PageMeta.Canonical = viewModel.PageMeta.Canonical.Replace(trim.SeoName, trim.CanonicalSeoName);
      }

      var vehicleInfoJsonStr = JsonConvert.SerializeObject(new
      {
        make = makeTranslator.Name,
        model = superModelTranslator.Name,
        trimid = trim.Id
      });
      viewModel.RegisterPageJson(vehicleInfoJsonStr, "ABT.pageJson.vehicleinfo");

      return View("TrimWarranty", viewModel);

    }

    private ActionResult TrimPicsnVids(IMakeTranslator makeTranslator, ISuperModelTranslator superModelTranslator, IYearTranslator yearTranslator, ITrimTranslator trimTranslator)
    {
      const string assetsPrefix = "research.trimpicsnvids";
      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var trimTask = VehicleSpecService
        .GetTrimOverviewByMakeBySuperModelByYearByTrimAsync(makeTranslator.SeoName, superModelTranslator.SeoName, yearTranslator.Number, trimTranslator.SeoName);

      trimTask.Wait();

      var trim = trimTask.Result;
      var trimImageTask = ImageMetaService.GetImagesByTrimIdAsync(trim.Id);

      trimImageTask.Wait();
      var trimImage = trimImageTask.Result;

      var viewModel = new PicsnVidsViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name,
          SuperModel = superModelTranslator.Name,
          Year = yearTranslator.Number.ToString(),
          Trim = trimTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.Name,
          fuel = trim.FuelType,
          category = UriTokenTranslators.GetCategoryTranslatorById(trim.CategoryId).Name,
          bodystyle = trim.BodyStyle
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name,
          makeId = makeTranslator.Id,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.SeoName
        }),
        TrimSectionSeoName = "picturesvideos",
        MainImage = trimImage != null && trimImage.Any() ? String.Format("{0}_320x.png", trimImage.FirstOrDefault().UrlPrefix) : "",
        Make = new PicsnVidsViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        },
        SuperModel = new PicsnVidsViewModel.Dto.SuperModel
        {
          Name = superModelTranslator.Name,
          SeoName = superModelTranslator.SeoName
        },
        Year = yearTranslator.Number,
        Trim = new PicsnVidsViewModel.Dto.Trim()
        {
          Name = trimTranslator.Name,
          SeoName = trimTranslator.SeoName,
          Year = yearTranslator.Number,
          FullDisplayName = trim.FullDisplayName,
          Model = trim.Model,
          SuperTrim = trim.SuperTrim
        },
      };

      if (trimImage != null && trimImage.Any())
      {
        viewModel.Images = 
            trimImage.Select(t => new PicsnVidsViewModel.Dto.Image
              {
                Small = String.Format("{0}_320x.png", t.UrlPrefix),
                Medium = String.Format("{0}_640x.png", t.UrlPrefix),
                Large = String.Format("{0}_1024x.png", t.UrlPrefix),
                ImageId = String.Format("PnVSlider_{0}", t.Id)
              });
      }
      else
      {
        var noImageList = new List<PicsnVidsViewModel.Dto.Image>();
        noImageList.Add(new PicsnVidsViewModel.Dto.Image
              {
                Small = "/assets/svg/no-image-avail.svg",
                Medium = "/assets/svg/no-image-avail.svg",
                Large = "/assets/svg/no-image-avail.svg",
                ImageId = "PnVSlider_0"
              });
        viewModel.Images = noImageList;
      }

      if (trim.CanonicalSeoName.IsNotNullOrEmpty() && trim.CanonicalSeoName != trim.SeoName)
      {
        viewModel.PageMeta.Canonical = viewModel.PageMeta.Canonical.Replace(trim.SeoName, trim.CanonicalSeoName);
      }

      var vehicleInfoJsonStr = JsonConvert.SerializeObject(new
      {
        make = makeTranslator.Name,
        model = superModelTranslator.Name,
        trimid = trim.Id
      });
      viewModel.RegisterPageJson(vehicleInfoJsonStr, "ABT.pageJson.vehicleinfo");

      return View("TrimPicsnVids", viewModel);

    }

    private ActionResult TrimSafety(IMakeTranslator makeTranslator, ISuperModelTranslator superModelTranslator, IYearTranslator yearTranslator, ITrimTranslator trimTranslator)
    {
      const string assetsPrefix = "research.trimsafety";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var trimTask = VehicleSpecService
        .GetTrimSafetyByMakeBySuperModelByYearByTrimAsync(makeTranslator.SeoName, superModelTranslator.SeoName, yearTranslator.Number, trimTranslator.SeoName);

      trimTask.Wait();
      var trim = trimTask.Result;

      var viewModel = new TrimSafetyViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name,
          SuperModel = superModelTranslator.Name,
          Year = yearTranslator.Number.ToString(),
          Trim = trimTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.Name,
          fuel = trim.FuelType,
          category = UriTokenTranslators.GetCategoryTranslatorById(trim.CategoryId).Name,
          bodystyle = trim.BodyStyle
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name,
          makeId = makeTranslator.Id,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.SeoName
        }),
        TrimSectionSeoName = "safety",
        Make = new TrimSafetyViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        },
        SuperModel = new TrimSafetyViewModel.Dto.SuperModel
        {
          Name = superModelTranslator.Name,
          SeoName = superModelTranslator.SeoName
        },
        Year = yearTranslator.Number,
        Trim = new TrimSafetyViewModel.Dto.Trim()
        {
          Name = trimTranslator.Name,
          SeoName = trimTranslator.SeoName,
          Year = yearTranslator.Number,
          FullDisplayName = trim.FullDisplayName,
          Model = trim.Model,
          SuperTrim = trim.SuperTrim,
          Msrp = trim.Msrp,
        },
        SafetyItems = trim.SafetyItems
          .Select(t => new TrimSafetyViewModel.Dto.Specifications
          {
            Group = t.Group,
            Title = t.Title,
            Data = t.Data,
            Availability = t.Availability
          })
      };

      if (trim.CanonicalSeoName.IsNotNullOrEmpty() && trim.CanonicalSeoName != trim.SeoName)
      {
        viewModel.PageMeta.Canonical = viewModel.PageMeta.Canonical.Replace(trim.SeoName, trim.CanonicalSeoName);
      }

      var vehicleInfoJsonStr = JsonConvert.SerializeObject(new
      {
        make = makeTranslator.Name,
        model = superModelTranslator.Name,
        trimid = trim.Id
      });
      viewModel.RegisterPageJson(vehicleInfoJsonStr, "ABT.pageJson.vehicleinfo");

      return View("TrimSafety", viewModel);

    }

    private ActionResult TrimColor(IMakeTranslator makeTranslator, ISuperModelTranslator superModelTranslator, IYearTranslator yearTranslator, ITrimTranslator trimTranslator)
    {
      const string assetsPrefix = "research.trimcolor";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var trimTask = VehicleSpecService
        .GetTrimColorByMakeBySuperModelByYearByTrimAsync(makeTranslator.SeoName, superModelTranslator.SeoName, yearTranslator.Number, trimTranslator.SeoName);

      trimTask.Wait();
      var trim = trimTask.Result;

      var viewModel = new TrimColorViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name,
          SuperModel = superModelTranslator.Name,
          Year = yearTranslator.Number.ToString(),
          Trim = trimTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.Name,
          fuel = trim.FuelType,
          category = UriTokenTranslators.GetCategoryTranslatorById(trim.CategoryId).Name,
          bodystyle = trim.BodyStyle
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name,
          makeId = makeTranslator.Id,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.SeoName
        }),
        TrimSectionSeoName = "color",
        Make = new TrimColorViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        },
        SuperModel = new TrimColorViewModel.Dto.SuperModel
        {
          Name = superModelTranslator.Name,
          SeoName = superModelTranslator.SeoName
        },
        Year = yearTranslator.Number,
        Trim = new TrimColorViewModel.Dto.Trim()
        {
          Name = trimTranslator.Name,
          SeoName = trimTranslator.SeoName,
          Year = yearTranslator.Number,
          FullDisplayName = trim.FullDisplayName,
          Model = trim.Model,
          SuperTrim = trim.SuperTrim,
          Msrp = trim.Msrp,
        },
        IntColorItems = trim.Colors
          .Where(t => t.ColorType == "Int")
          .Select(t => new TrimColorViewModel.Dto.Colors
          {
            ColorCode = t.ColorCode,
            ColorType = t.ColorType,
            ColorDesc = t.ColorDesc,
            ExtImage = t.ExtImage,
            RGB = t.RGB
          }),
        ExtColorItems = trim.Colors
          .Where(t => t.ColorType == "Pri")
          .Select(t => new TrimColorViewModel.Dto.Colors
          {
            ColorCode = t.ColorCode,
            ColorType = t.ColorType,
            ColorDesc = t.ColorDesc,
            ExtImage = t.ExtImage,
            RGB = t.RGB
          })
      };

      if (trim.CanonicalSeoName.IsNotNullOrEmpty() && trim.CanonicalSeoName != trim.SeoName)
      {
        viewModel.PageMeta.Canonical = viewModel.PageMeta.Canonical.Replace(trim.SeoName, trim.CanonicalSeoName);
      }

      var vehicleInfoJsonStr = JsonConvert.SerializeObject(new
      {
        make = makeTranslator.Name,
        model = superModelTranslator.Name,
        trimid = trim.Id
      });
      viewModel.RegisterPageJson(vehicleInfoJsonStr, "ABT.pageJson.vehicleinfo");

      return View("TrimColor", viewModel);

    }

    private ActionResult TrimIncentives(IMakeTranslator makeTranslator, ISuperModelTranslator superModelTranslator, IYearTranslator yearTranslator, ITrimTranslator trimTranslator)
    {
      const string assetsPrefix = "research.trimincentives";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var trimTask = VehicleSpecService
        .GetTrimIncentivesByMakeBySuperModelByYearByTrimByZipAsync(makeTranslator.SeoName, superModelTranslator.SeoName, yearTranslator.Number, trimTranslator.SeoName, "0");

      trimTask.Wait();
      var trim = trimTask.Result;

      var viewModel = new TrimIncentivesViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeTranslator.Name,
          SuperModel = superModelTranslator.Name,
          Year = yearTranslator.Number.ToString(),
          Trim = trimTranslator.Name
        },
        AdvertMeta = new AdvertMeta(new
        {
          make = makeTranslator.Name,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.Name,
          fuel = trim.FuelType,
          category = UriTokenTranslators.GetCategoryTranslatorById(trim.CategoryId).Name,
          bodystyle = trim.BodyStyle
        }),
        OfferMeta = new OfferMeta(new
        {
          make = makeTranslator.Name,
          makeId = makeTranslator.Id,
          model = superModelTranslator.Name,
          year = yearTranslator.Number,
          trim = trimTranslator.SeoName
        }),
        TrimSectionSeoName = "incentives",
        Make = new TrimIncentivesViewModel.Dto.Make
        {
          Name = makeTranslator.Name,
          SeoName = makeTranslator.SeoName
        },
        SuperModel = new TrimIncentivesViewModel.Dto.SuperModel
        {
          Name = superModelTranslator.Name,
          SeoName = superModelTranslator.SeoName
        },
        Year = yearTranslator.Number,
        Trim = new TrimIncentivesViewModel.Dto.Trim()
        {
          TrimId = trim.Id,
          Name = trimTranslator.Name,
          SeoName = trimTranslator.SeoName,
          Year = yearTranslator.Number,
          FullDisplayName = trim.FullDisplayName,
          Model = trim.Model,
          SuperTrim = trim.SuperTrim,
          Msrp = trim.Msrp,
        },
        Public = trim.Incentives
          .Where(t => t.GroupDesc == "Public")
          .Select(t => new TrimIncentivesViewModel.Dto.Incentives
          {
            Expires = String.Format("{0:MM/dd/yyyy}", t.Expires),
            GroupDesc = t.GroupDesc,
            CatDesc = t.CatDesc.Replace("Stand Alone Retail", "").Replace("OEM Finance", ""),
            MasterDesc = t.MasterDesc,
            Amount = t.Amount,
            APR_24 = t.APR_24,
            APR_36 = t.APR_36,
            APR_48 = t.APR_48,
            APR_60 = t.APR_60,
            APR_72 = t.APR_72,
            APR_84 = t.APR_84
          }),
        Retiree = trim.Incentives
          .Where(t => t.GroupDesc == "Retiree")
          .Select(t => new TrimIncentivesViewModel.Dto.Incentives
          {
            Expires = String.Format("{0:MM/dd/yyyy}", t.Expires),
            GroupDesc = t.GroupDesc,
            CatDesc = t.CatDesc.Replace("Stand Alone Retail", "").Replace("OEM Finance", ""),
            MasterDesc = t.MasterDesc,
            Amount = t.Amount,
            APR_24 = t.APR_24,
            APR_36 = t.APR_36,
            APR_48 = t.APR_48,
            APR_60 = t.APR_60,
            APR_72 = t.APR_72,
            APR_84 = t.APR_84
          }),
        Military = trim.Incentives
          .Where(t => t.GroupDesc == "Military")
          .Select(t => new TrimIncentivesViewModel.Dto.Incentives
          {
            Expires = String.Format("{0:MM/dd/yyyy}", t.Expires),
            GroupDesc = t.GroupDesc,
            CatDesc = t.CatDesc.Replace("Stand Alone Retail", "").Replace("OEM Finance", ""),
            MasterDesc = t.MasterDesc,
            Amount = t.Amount,
            APR_24 = t.APR_24,
            APR_36 = t.APR_36,
            APR_48 = t.APR_48,
            APR_60 = t.APR_60,
            APR_72 = t.APR_72,
            APR_84 = t.APR_84
          }),
        College = trim.Incentives
          .Where(t => t.GroupDesc == "College")
          .Select(t => new TrimIncentivesViewModel.Dto.Incentives
          {
            Expires = String.Format("{0:MM/dd/yyyy}", t.Expires),
            GroupDesc = t.GroupDesc,
            CatDesc = t.CatDesc.Replace("Stand Alone Retail", "").Replace("OEM Finance", ""),
            MasterDesc = t.MasterDesc,
            Amount = t.Amount,
            APR_24 = t.APR_24,
            APR_36 = t.APR_36,
            APR_48 = t.APR_48,
            APR_60 = t.APR_60,
            APR_72 = t.APR_72,
            APR_84 = t.APR_84
          })
      };

      if (trim.CanonicalSeoName.IsNotNullOrEmpty() && trim.CanonicalSeoName != trim.SeoName)
      {
        viewModel.PageMeta.Canonical = viewModel.PageMeta.Canonical.Replace(trim.SeoName, trim.CanonicalSeoName);
      }

      var vehicleInfoJsonStr = JsonConvert.SerializeObject(new
      {
        make = makeTranslator.Name,
        model = superModelTranslator.Name,
        trimid = trim.Id
      });
      viewModel.RegisterPageJson(vehicleInfoJsonStr, "ABT.pageJson.vehicleinfo");

      return View("TrimIncentives", viewModel);

    }

    #endregion
     

    #region Services

    private static IVehicleSpecService VehicleSpecService
    {
      get { return ServiceLocator.Get<IVehicleSpecService>(); }
    }

    private static IImageMetaService ImageMetaService
    {
      get { return ServiceLocator.Get<IImageMetaService>(); }
    }

    #endregion
  }
}
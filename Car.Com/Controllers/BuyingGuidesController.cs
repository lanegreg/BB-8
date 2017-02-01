using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Car.Com.Common;
using Car.Com.Common.SiteMeta;
using Car.Com.Domain.Models.Content;
using Car.Com.Domain.Models.SiteMeta;
using Car.Com.Domain.Services;
using Car.Com.Models.BuyingGuides;
using Car.Com.Models.Shared;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using Newtonsoft.Json;


namespace Car.Com.Controllers
{
  [RoutePrefix("buying-guides")]
  public class BuyingGuidesController : BaseController
  {
    [Route("{page?}", Name = "BuyingGuides_Index"), HttpGet]
    public ActionResult Index(int? page)
    {
      const string assetsPrefix = "buyingguides.index";
      const int articlesPerPage = 6;

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var pageNum = page ?? 1;
      // *** should be using skip/take collection semantic here ...all the way through to the service ***
      var startRow = articlesPerPage*(pageNum - 1) + 1;

      var financeTask = VehicleContentService.GetArticlesByTopicAsync("finance", startRow, articlesPerPage);

      var mostRecentGroupTask = VehicleContentService.GetArticlesGroupMostRecentAsync();

      var categoriesTask = VehicleContentService.GetArticlesByVehiclesCategoryAsync();

      mostRecentGroupTask.Wait();
      var mostRecentGroup = mostRecentGroupTask.Result;

      financeTask.Wait();
      var financeArticles = financeTask.Result;

      categoriesTask.Wait();
      var categoriesArticles = categoriesTask.Result;

      var viewModel = new IndexViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata),
        MostRecentGroup = mostRecentGroup.Articles,
        FinanceArticles = financeArticles.Articles,
        CategoriesArticles = categoriesArticles.Articles,
      };

      return View("Index", viewModel);
    }


    [Route("{categorySeoName:minlength(3):categories}/", Name = "BuyingGuides_ByCategory"), HttpGet]
    public ActionResult BuyingGuidesByCategory(string categorySeoName)
    {
      const string assetsPrefix = "buyingguides.articlecategory";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var categoriesSectionTask = VehicleContentService.GetArticlesByCategoryAttributeSeoNameAsync(categorySeoName);

      categoriesSectionTask.Wait();
      var categoriesSectionArticles = categoriesSectionTask.Result;

      var viewModel = new CategoryViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        CategoriesSectionArticles = categoriesSectionArticles.Articles,
        VehicleCategoryName = categoriesSectionArticles.Category
      };

      return View("ArticleCategory", viewModel);
    }


    [Route("{vehicleAttributeSeoName:minlength(3):vehicle_attributes}/", Name = "BuyingGuides_ByVehicleAttribute"),
     HttpGet]
    public ActionResult BuyingGuidesByVehicleAttribute(string vehicleAttributeSeoName)
    {
      const string assetsPrefix = "buyingguides.articlecategory";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var categoriesSectionTask =
        VehicleContentService.GetArticlesByCategoryAttributeSeoNameAsync(vehicleAttributeSeoName);

      categoriesSectionTask.Wait();
      var categoriesSectionArticles = categoriesSectionTask.Result;

      var viewModel = new CategoryViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        CategoriesSectionArticles = categoriesSectionArticles.Articles,
        VehicleCategoryName = categoriesSectionArticles.Category
      };

      return View("ArticleCategory", viewModel);
    }


    [Route("alternative-fuel-vehicles/", Name = "BuyingGuides_AlternativeFuelVehicles"), HttpGet]
    public ActionResult BuyingGuidesAlternativeFuelVehicles()
    {
      const string assetsPrefix = "buyingguides.articlecategory";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var categoriesSectionTask =
        VehicleContentService.GetArticlesByCategoryAttributeSeoNameAsync("alternative-fuel-vehicles");

      categoriesSectionTask.Wait();
      var categoriesSectionArticles = categoriesSectionTask.Result;

      var viewModel = new CategoryViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        CategoriesSectionArticles = categoriesSectionArticles.Articles,
        VehicleCategoryName = categoriesSectionArticles.Category
      };

      return View("Articlecategory", viewModel);
    }


    [Route("{title:minlength(1)}-{contentId:int}", Name = "BuyingGuide_Article"), HttpGet]
    public ActionResult Article_BuyingGuide(string title, int contentId)
    {
      const string assetsPrefix = "buyingguides.article";
      const int numAdUnits = 4;

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var articleTask = VehicleContentService.GetArticleByIdAsync(contentId);

      articleTask.Wait();
      var article = articleTask.Result;


      var makeTranslator = UriTokenTranslators.GetMakeTranslatorByName(article.Make ?? String.Empty);
      var makeName = makeTranslator != null ? makeTranslator.Name : String.Empty;

      var modelTranslator = UriTokenTranslators.GetModelTranslatorByName(article.Model ?? String.Empty);
      var modelName = modelTranslator != null ? modelTranslator.Name : String.Empty;

      var year = article.Year.ToString();

      var categoryTranslator = UriTokenTranslators.GetCategoryTranslatorByName(article.VehicleCategory ?? String.Empty);
      var categoryName = categoryTranslator != null ? categoryTranslator.Name : String.Empty;



      // throws a 404 exception if the current URL doesn't match the URL of the article.
      if (article.Url.IsNullOrEmpty() || HttpContext.Request.Url.AbsoluteUri.IndexOf(article.Url) == -1)
      {
        throw new HttpException(404, "HTTP/1.1 404 Not Found");
      }


      var articleAds = new List<List<ArticleAd>>();
      var articleVehicles = new List<TrackerVehicle>();

      // set article page ads
      foreach (var articlePage in article.ArticlePages)
      {
        articleAds.Add((List<ArticleAd>)articlePage.AdUnits);

        articleVehicles.Add(new TrackerVehicle
        {
          Make = articlePage.Make ?? makeName,
          Model = articlePage.Model ?? modelName,
          SuperModel = articlePage.Model ?? modelName,
          Year = articlePage.Year > 1900 ? articlePage.Year : Int32.Parse(year)
        });
      }

      
      // set article page ads for endcap
      var adsEndcap = new List<ArticleAd>();

      for (var adUnit = 1; adUnit <= numAdUnits; adUnit++)
      {
        var articleAdUnit = article.AdUnits.FirstOrDefault(x => x.AdUnitId == adUnit);

        adsEndcap.Add(new ArticleAd
        {
          AdMake = articleAdUnit.AdMake ?? String.Empty,
          AdModel = articleAdUnit.AdModel ?? String.Empty,
          AdVehicleYear = articleAdUnit.AdVehicleYear,
          AdCategory = articleAdUnit.AdCategory ?? String.Empty,
          AdUnitId = articleAdUnit.AdUnitId
        });
      }

      articleAds.Add(adsEndcap);

      var articleVehiclesJsonStr = JsonConvert.SerializeObject(articleVehicles);
      var articleAdsJsonStr = JsonConvert.SerializeObject(articleAds);



      var viewModel = new ArticleViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        AdvertMeta = new AdvertMeta(new
        {
          ads = articleAds.First()
        }),
        PageMeta = new PageMeta
        {
          Title = article.Title,
          Canonical = "http://www.car.com" + article.Url,
          Keywords = article.MetaKeywords,
          Description = article.MetaDescription
        },
        TrackMeta = new TrackMeta(metadata)
        {
          Make = makeName,
          SuperModel = modelName,
          Year = year,
          Category = categoryName
        },
        OpenGraphMeta = new OpenGraphMeta
        {
          Title = article.Title,
          Description = article.MetaDescription,
          Type = "article",
          Url = "http://www.car.com" + article.Url,
          Images = article.ArticlePages.Select(x => x.PageImage).Where(x => x.IsNotNullOrEmpty()).ToList()
        },
        Article = article
      };


      viewModel.RegisterPageJson(articleVehiclesJsonStr, "ABT.pageJson.articleVehicles");
      viewModel.RegisterPageJson(articleAdsJsonStr, "ABT.pageJson.articleAds");

      var articleGallery = new ArticleGalleryViewModel();
      {
        articleGallery.Article = viewModel.Article;
      }

      viewModel.ArticleGallery = articleGallery;

      return View("Article", viewModel);
    }

    #region Services

    private static IVehicleContentService VehicleContentService
    {
      get { return ServiceLocator.Get<IVehicleContentService>(); }
    }

    #endregion
  }
}
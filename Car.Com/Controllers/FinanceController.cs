using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Car.Com.Common;
using Car.Com.Common.Pagination;
using Car.Com.Common.SiteMeta;
using Car.Com.Domain.Models.Content;
using Car.Com.Domain.Models.SiteMeta;
using Car.Com.Domain.Services;
using Car.Com.Models.Finance;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using Newtonsoft.Json;
using  Car.Com.Models.Shared;
using Microsoft.Ajax.Utilities;


namespace Car.Com.Controllers
{
  [RoutePrefix("finance")]
  public class FinanceController : BaseController
  {
    [Route("{page?}", Name = "Finance_Index"), HttpGet]
    public ActionResult Index(int? page)
    {
      const string assetsPrefix = "finance.index";
      const int articlesPerPage = 6;

      var pageNum = page ?? 1;
      // *** should be using skip/take collection symantic here ...all the way through to the service ***
      var startRow = articlesPerPage * (pageNum - 1) + 1;

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var financeTask = VehicleContentService.GetArticlesByTopicAsync("finance", startRow, articlesPerPage);

      financeTask.Wait();
      var financeArticles = financeTask.Result;

      var totalNumOfPages = financeArticles.TotalRecords > articlesPerPage
        ? (int)Math.Ceiling(((decimal)financeArticles.TotalRecords / articlesPerPage))
        : 1;

      var viewModel = new IndexViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        FinanceArticles = financeArticles.Articles,
        Paginator = new Paginator("/finance/", totalNumOfPages, pageNum)
      };
      
      return View("Index", viewModel);
    }

    [Route("{title:minlength(1)}-{contentId:int}", Name = "Finance_Article"), HttpGet]
    public ActionResult Article(string title, int contentId)
    {
      const string assetsPrefix = "finance.article";
      const int numAdUnits = 4;

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var articleTask = VehicleContentService.GetArticleByIdAsync(contentId);

      articleTask.Wait();

      var article = articleTask.Result;

      var makeTranslator = String.Empty;
      var modelTranslator = String.Empty;
      var yearTranslator = String.Empty;
      var categoryTranslator = String.Empty;

      if (article.Make.IsNotNullOrEmpty())
      {
        makeTranslator = UriTokenTranslators.GetMakeTranslatorByName(article.Make).Name;
      }

      if (article.Model.IsNotNullOrEmpty())
      {
        modelTranslator = UriTokenTranslators.GetModelTranslatorByName(article.Model).Name;
      }

      if (article.Year.ToString().IsNotNullOrEmpty())
      {
        yearTranslator = article.Year.ToString();
      }

      if (article.VehicleCategory.IsNotNullOrEmpty())
      {
        var tempcategoryTranslator = UriTokenTranslators.GetCategoryTranslatorByName(article.VehicleCategory);

        if (tempcategoryTranslator != null)
          categoryTranslator = tempcategoryTranslator.Name;
      }

      var currentUrl = HttpContext.Request.Url.AbsoluteUri;

      // throws a 404 exception if the current URL doesn't match the URL of the article.
      if (article.Url.IsNullOrEmpty() || currentUrl.IndexOf(article.Url) == -1)
      {
        throw new HttpException(404, "HTTP/1.1 404 Not Found");
      }

      ICollection<ArticleAd> ads = new Collection<ArticleAd>();
      IList<ICollection<ArticleAd>> articleAds = new List<ICollection<ArticleAd>>();
      IList<TrackerVehicle> articleVehicles = new List<TrackerVehicle>();

      TrackerVehicle vehicle;

      // set article page ads
      foreach (var articlePage in article.ArticlePages)
      {
        ads = articlePage.AdUnits;

        articleAds.Add(ads);

        if (articlePage.Make != null)
        {
          vehicle = new TrackerVehicle
          {
            Make = articlePage.Make,
            Model = articlePage.Model,
            SuperModel = articlePage.Model,
            Year = articlePage.Year
          };
        }
        else
        {
          vehicle = new TrackerVehicle
          {
            Make = makeTranslator,
            Model = modelTranslator,
            SuperModel = modelTranslator,
            Year = Int32.Parse(yearTranslator)
          };
        }

        articleVehicles.Add(vehicle);

      }

      // set article page ads for endcap

      ICollection<ArticleAd> adsEndcap = new Collection<ArticleAd>();

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
          ads = articleAds[0]
        })
        ,
        PageMeta = new PageMeta
        {
          Title = article.Title,
          Canonical = "http://www.car.com" + article.Url,
          Keywords = article.MetaKeywords,
          Description = article.MetaDescription
        },
        TrackMeta = new TrackMeta
        {
          Make = makeTranslator,
          SuperModel = modelTranslator,
          Year = yearTranslator,
          Category = categoryTranslator
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

      ArticleGalleryViewModel articleGallery = new ArticleGalleryViewModel();
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
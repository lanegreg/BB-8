using Car.Com.Common;
using Car.Com.Common.Cache;
using Car.Com.Domain.Models.Sitemap;
using Car.Com.Domain.Services;
using Car.Com.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Car.Com.Service.Data.Impl
{
  public sealed class SitemapService : ICacheable, ISitemapService
  {
    #region Declarations

    private static readonly int RefreshIntervalInMins = WebConfig.Get<int>("SitemapService:ReCacheInterval_mins");
    private static readonly bool IsLocalDev = WebConfig.Get<bool>("Environment:IsLocalDev");
    private static readonly object Mutex = new object();
    private static readonly MemoryCache Cache = MemoryCache.Default;

    private const string Host = "www.car.com";
    private static readonly IEnumerable<string> SectionNames;

    private static IEnumerable<Section> _sections = new List<Section>();
    private static IDictionary<string, IEnumerable<Page>> _pagesBySectionDict = new Dictionary<string, IEnumerable<Page>>();

    #endregion
    

    static SitemapService()
    {
      SectionNames = new List<string>
      {
        "general",
        "car-research",
        "buying-tools",
        "content"
      };
    }

    public void Warm()
    {
      if (IsLocalDev)
        return;

      CacheAllSiteMapData();
      Cache.Set("Sitemap_Service", String.Empty, GetCachePolicy());
    }

    public IEnumerable<string> GetSitemapSectionNames()
    {
      return SectionNames;
    }

    public IEnumerable<ISection> GetSections(HttpRequestBase request)
    {
      var sections = _sections.ToList();
      sections.ForEach(s => s.IsSecureConnection = request.ConnectionIsSecureOrTerminatedSecure());

      return sections;
    }

    public IEnumerable<IPage> GetPagesBySectionName(HttpRequestBase request, string sectionName)
    {
      if (!_pagesBySectionDict.ContainsKey(sectionName))
        return new List<IPage>();
      
      var pages = _pagesBySectionDict[sectionName].ToList();
      pages.ForEach(p => p.IsSecureConnection = request.ConnectionIsSecureOrTerminatedSecure());

      return pages;
    }


    #region Private Static Methods

    private static void CacheUpdateHandler(CacheEntryUpdateArguments args)
    {
      CacheAllSiteMapData();
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

    private static void CacheAllSiteMapData()
    {
      try
      {
        var pagesBySectionDict = new Dictionary<string, IEnumerable<Page>>();
        var lastModified = DateTime.Now.ToString("yyyy-MM-dd");

        #region **! Here, we are caching all the sitemap sections for the sitemap-index page.

        _sections = SectionNames
          .Select(sectionName => new Section
          {
            DomainAndPath = String.Format("{0}/sitemap/{1}", Host, sectionName),
            LastModified = lastModified
          });

        #endregion


        #region **! Here, we are caching all the pages for each sitemap section.

        #region Cache all pages for the *general* sitemap section.
        const string generalSection = "general";

        pagesBySectionDict.Add(generalSection, new List<Page>
        {
          new Page
          {
            DomainAndPath = Host,
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Top
          },
          new Page
          {
            DomainAndPath = String.Format("{0}/{1}", Host, "cars-for-sale"),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Medium
          }
        });

        #endregion


        #region Cache all pages for the *car-research* sitemap section.

        // NOTE: We are only going after all the *NEW* cars in this section.
        const string researchSection = "car-research";

        // landing page
        var researchPages = new List<Page>
        {
          new Page
          {
            DomainAndPath = String.Format("{0}/{1}", Host, researchSection),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Top
          }
        };

        // make pages
        var researchMakes = UriTokenTranslators.GetAllMakeTranslators()
          .Where(make => make.IsActive)
          .Select(make => new Dto.CarResearchUrl
          {
            Make = make.SeoName
          }).ToList().OrderBy(s => s.Make);

        researchPages.AddRange(researchMakes
          .Select(carResearchUrl => new Page
          {
            DomainAndPath = String.Format("{0}/{1}", Host, carResearchUrl.Make),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Medium
          }).ToList());


        // make/super-model pages
        var researchMakeSuperModels = VehicleSpecService.GetAllNewSuperModels()
          .Select(superModel => new Dto.CarResearchUrl
          {
            Make = UriTokenTranslators.GetMakeTranslatorByName(superModel.Make).SeoName,
            SuperModel = superModel.SeoName
          }).ToList().OrderBy(s => s.Make).ThenBy(s => s.SuperModel);

        researchPages.AddRange(researchMakeSuperModels
          .Select(carResearchUrl => new Page
          {
            DomainAndPath = String.Format("{0}/{1}/{2}", Host, carResearchUrl.Make, carResearchUrl.SuperModel),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Medium
          }).ToList());


        // make/super-model/year pages
        var makeSuperModelYears =
          (from superModel in VehicleSpecService.GetAllNewSuperModels()
            from year in superModel.Years
            where year.IsNew
            select new Dto.CarResearchUrl
            {
              Make = UriTokenTranslators.GetMakeTranslatorByName(superModel.Make).SeoName,
              SuperModel = superModel.SeoName,
              Year = year.Number
            }).ToList().OrderBy(s => s.Make).ThenBy(s => s.SuperModel).ThenBy(s => s.Year);

        researchPages.AddRange(makeSuperModelYears
          .Select(carResearchUrl => new Page
          {
            DomainAndPath =
              String.Format("{0}/{1}/{2}/{3}",
                Host, carResearchUrl.Make, carResearchUrl.SuperModel, carResearchUrl.Year),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Medium
          }).ToList());


        // make/super-model/year/trim pages
        var makeSuperModelYearTrims =
          (from msmy in makeSuperModelYears
            from trim in VehicleSpecService.GetTrimsByMakeBySuperModelByYear(msmy.Make, msmy.SuperModel, msmy.Year)
            select new Dto.CarResearchUrl
            {
              Make = msmy.Make,
              SuperModel = msmy.SuperModel,
              Year = msmy.Year,
              Trim = trim.CanonicalSeoName
            }).ToList().OrderBy(s => s.Make).ThenBy(s => s.SuperModel).ThenBy(s => s.Trim).ThenBy(s => s.Year);

        var makeSuperModelYearTrimsDistinct = makeSuperModelYearTrims.GroupBy(x => x.Trim).Select(g => g.First());

        researchPages.AddRange(makeSuperModelYearTrimsDistinct
          .Select(carResearchUrl => new Page
          {
            DomainAndPath =
              String.Format("{0}/{1}/{2}/{3}/{4}",
                Host, carResearchUrl.Make, carResearchUrl.SuperModel, carResearchUrl.Year, carResearchUrl.Trim),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Medium
          }).ToList());
        

        // make/super-model/year/trim/section pages
        var trimSectionNames = new List<string>
        {
          "specifications",
          "color",
          "warranty",
          "pictures-videos",
          "incentives",
          "safety"
        };

        var makeSuperModelYearTrimSections =
          (from msmyt in makeSuperModelYearTrimsDistinct
            from sectionName in trimSectionNames
            select new Dto.CarResearchUrl
            {
              Make = msmyt.Make,
              SuperModel = msmyt.SuperModel,
              Year = msmyt.Year,
              Trim = msmyt.Trim,
              Section = sectionName
            }).ToList().OrderBy(s => s.Make).ThenBy(s => s.SuperModel).ThenBy(s => s.Trim).ThenBy(s => s.Year).ThenBy(s => s.Section);

        researchPages.AddRange(makeSuperModelYearTrimSections
          .Select(carResearchUrl => new Page
          {
            DomainAndPath =
              String.Format("{0}/{1}/{2}/{3}/{4}/{5}",
                Host, carResearchUrl.Make, carResearchUrl.SuperModel, carResearchUrl.Year, carResearchUrl.Trim, carResearchUrl.Section),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Low
          }).ToList());


        // Add them all here!
        pagesBySectionDict.Add(researchSection, researchPages);

        #endregion


        #region Cache all pages for the *buying-tools* sitemap section.
        const string toolsSection = "buying-tools";

        var toolNames = new List<string>
        {
          "tools/car-comparison",
          "tools/calculators",
          "tools/calculators/payment-calculator",
          "tools/calculators/affordability-calculator",
          "tools/calculators/fuel-savings-calculator",
          "tools/calculators/lease-vs-buy-calculator",
          "tools/calculators/early-payoff-calculator",
          "tools/calculators/rebate-vs-financing-calculator",
          "tools/used-car-values"
        };

        var toolPages = new List<Page>
        {
          new Page
          {
            DomainAndPath = String.Format("{0}/{1}", Host, "tools"),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Monthly,
            Priority = Priority.Top
          }
        };

        toolPages.AddRange(toolNames
          .Select(toolName => new Page
          {
            DomainAndPath = String.Format("{0}/{1}", Host, toolName),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Monthly,
            Priority = Priority.Medium
          }));


        // Add them all here!
        pagesBySectionDict.Add(toolsSection, toolPages);

        #endregion


        #region Cache all pages for the *content* sitemap section.

        const string contentSection = "content";

        var contentPages = new List<Page>
        {
          new Page
          {
            DomainAndPath = String.Format("{0}/{1}", Host, "buying-guides"),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Top
          },
          new Page
          {
            DomainAndPath = String.Format("{0}/{1}", Host, "finance"),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Medium
          }
        };

        // article urls
        var articleUrls = VehicleContentService.GetAllArticleUrlsForSitemap();

        contentPages.AddRange(articleUrls
          .Select(articleUrl => new Page
          {
            DomainAndPath = String.Format("{0}{1}", Host, articleUrl.TrimEnd(new [] { '/' })),
            LastModified = lastModified,
            ChangeFrequency = ChangeFrequency.Weekly,
            Priority = Priority.Low
          }).ToList());

        pagesBySectionDict.Add(contentSection, contentPages);

        #endregion

        lock (Mutex)
        {
          _pagesBySectionDict = pagesBySectionDict;
        }
        
        #endregion
      }
      catch (Exception ex)
      {
        Log.Fatal(String.Format(
          "The SitemapService Failed during the caching process. It's likely an SeoName problem exists in the data./r/n{0}",
          ex.Message));
      }
    }

    #endregion


    public static class Dto
    {
      public class CarResearchUrl
      {
        public string Make { get; set; }
        public string SuperModel { get; set; }
        public string Trim { get; set; }
        public int Year { get; set; }
        public string Section { get; set; }
      }
    }
    

    #region Local Memory Cache

    private static ILocalCache LocalCache
    {
      get { return ServiceLocator.Get<ILocalCache>(); }
    }

    #endregion


    #region Services

    private static IVehicleSpecService VehicleSpecService
    {
      get { return ServiceLocator.Get<IVehicleSpecService>(); }
    }

    private static IVehicleContentService VehicleContentService
    {
      get { return ServiceLocator.Get<IVehicleContentService>(); }
    }

    #endregion
  }
}

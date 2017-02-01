using Car.Com.Common.Cacheability;
using Car.Com.Common.SiteMeta;
using Car.Com.Controllers.Filters;
using Car.Com.Domain.Services;
using Car.Com.Models.Site;
using Car.Com.Service;
using System.Linq;
using System.Web.Mvc;


namespace Car.Com.Controllers
{
  /** 
   * This Controller's sole purpose is to handle the Site urls (i.e. /, /about-us/, /contact-us/, etc.), nothing more! 
   **/

  public class SiteController : BaseController
	{

#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("", Name = "Site_Index")]
		public ActionResult Index()
		{
		  const string assetsPrefix = "site.index";

      const int startRow = 1;
      const int take = 9;

      var metadata = MetadataService.GetMetadataForPage(HttpContext);
      var buyingGuides = VehicleContentService.GetArticlesByTopic("buying guides", startRow, take);

      var viewModel = new IndexViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata),
        BuyingGuides = buyingGuides.Articles
      };
			
      return View("Index", viewModel);
		}


#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("tools", Name = "Site_Tools"), HttpGet]
    public ActionResult Tools()
    {
      const string assetsPrefix = "site.tools";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new ToolsViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("Tools", viewModel);
    }



    #region - Footer Route Handlers

    [Route("about-us", Name = "Site_About")]
    public ActionResult About()
    {
      const string assetsPrefix = "site.about";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new AboutViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("About", viewModel);
    }


    [Route("contact-us", Name = "Site_Contact")]
    public ActionResult Contact()
    {
      const string assetsPrefix = "site.contact";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new ContactViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("Contact", viewModel);
    }


    [Route("terms-of-use", Name = "Site_Terms")]
    public ActionResult Terms()
    {
      const string assetsPrefix = "site.terms";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new TermsViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("Terms", viewModel);
    }


    [Route("privacy-policy", Name = "Site_Privacy")]
    public ActionResult Privacy()
    {
      const string assetsPrefix = "site.privacy";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new PrivacyViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("Privacy", viewModel);
    }


    [Route("fraud-awareness", Name = "Site_Fraud")]
    public ActionResult Fraud()
    {
      const string assetsPrefix = "site.fraud";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new FraudViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("Fraud", viewModel);
    }

    #endregion



    #region - Sitemap Route Handlers

    [Route("sitemap-index", Name = "Site_XmlSitemapIndex")]
    public ActionResult XmlSitemapIndex()
    {
      var sections = SitemapService.GetSections(HttpContext.Request)
        .Select(s => new XmlSitemapIndexViewModel.Dto.Section
        {
          Url = s.Url,
          LastModified = s.LastModified
        });

      var viewModel = new XmlSitemapIndexViewModel()
      {
        Sections = sections
      };

      return View("XmlSitemapIndex", viewModel);
    }


    [Route("sitemap/{sectionName:minlength(3):sitemap_section_names}", Name = "Site_XmlSitemapSection")]
    public ActionResult XmlSitemapSection(string sectionName)
    {
      var pages = SitemapService.GetPagesBySectionName(HttpContext.Request, sectionName)
        .Select(p => new XmlSitemapSectionViewModel.Dto.Page
        {
          Url = p.Url,
          LastModified = p.LastModified,
          Priority = p.Priority,
          ChangeFrequency = p.ChangeFrequency
        });

      var viewModel = new XmlSitemapSectionViewModel()
      {
        Pages = pages
      };

      return View("XmlSitemapSection", viewModel);
    }

    #endregion



    #region - Services

    private static IVehicleContentService VehicleContentService
    {
      get { return ServiceLocator.Get<IVehicleContentService>(); }
    }

    private static ISitemapService SitemapService
    {
      get { return ServiceLocator.Get<ISitemapService>(); }
    }

    #endregion
	}
}
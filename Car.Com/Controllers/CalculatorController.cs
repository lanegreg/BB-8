using Car.Com.Common.SiteMeta;
using Car.Com.Models.Calculator;
using System.Web.Mvc;

namespace Car.Com.Controllers
{
  [RoutePrefix("tools/calculators")]
  public class CalculatorController : BaseController
  {
    /** 
     * This Controller's sole purpose is to handle all urls that belong to calculator pages, nothing more!
     **/


    [Route("", Name = "Calculator_Index"), HttpGet]
    public ActionResult Index()
    {
      const string assetsPrefix = "calculator.index";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new IndexViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("Index", viewModel);
    }


    [Route("lease-vs-buy-calculator/", Name = "Calculator_LeaseOrPurchase"), HttpGet]
    public ActionResult LeaseOrPurchase()
    {
      const string assetsPrefix = "calculator.leaseorpurchase";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new LeaseOrPurchaseViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("LeaseOrPurchase", viewModel);
    }


    [Route("rebate-vs-financing-calculator/", Name = "Calculator_LoanVsFinancing"), HttpGet]
    public ActionResult LoanVsFinancing()
    {
      const string assetsPrefix = "calculator.loanvsfinancing";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new LoanVsFinancingViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("LoanVsFinancing", viewModel);
    }


    [Route("payment-calculator/", Name = "Calculator_PaymentEstimate"), HttpGet]
    public ActionResult PaymentEstimate()
    {
      const string assetsPrefix = "calculator.paymentestimate";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new PaymentEstimateViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("PaymentEstimate", viewModel);
    }


    [Route("fuel-savings-calculator/", Name = "Calculator_FuelEfficiency"), HttpGet]
    public ActionResult FuelEfficiency()
    {
      const string assetsPrefix = "calculator.fuelefficiency";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new FuelEfficiencyViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("FuelEfficiency", viewModel);
    }


    [Route("early-payoff-calculator/", Name = "Calculator_AcceleratedPayoff"), HttpGet]
    public ActionResult AcceleratedPayoff()
    {
      const string assetsPrefix = "calculator.acceleratedpayoff";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new AcceleratedPayoffViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("AcceleratedPayoff", viewModel);
    }


    [Route("affordability-calculator/", Name = "Calculator_Affordability"), HttpGet]
    public ActionResult Affordability()
    {
      const string assetsPrefix = "calculator.affordability";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var viewModel = new AffordabilityViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata)
      };

      return View("Affordability", viewModel);
    }

  }
}
using Car.Com.Common.Cacheability;
using Car.Com.Common.SiteMeta;
using Car.Com.Controllers.Api;
using Car.Com.Controllers.Filters;
using Car.Com.Domain.Models.Evaluation;
using Car.Com.Domain.Services;
using Car.Com.Models.CarValue;
using Car.Com.Service;
using System.Linq;
using System.Web.Mvc;

namespace Car.Com.Controllers
{
  [RoutePrefix("tools")]
  public class CarValueController : BaseController
  {
#if !DEBUG
    [Cacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("used-car-values/", Name = "CarValue_Index"), HttpGet]
    public ActionResult Index()
    {
      const string assetsPrefix = "carvalue.index";

      var metadata = MetadataService.GetMetadataForPage(HttpContext);

      var evalTypeKVs = EvaluationService.EvaluationTypes
        .Select(t => new IndexViewModel.Dto.EvaluationType
        {
          Key = t.Key,
          Value = t.Value
        });

      var yearKVs = EvaluationService.GetYears()
        .Select(t => new IndexViewModel.Dto.Year
        {
          Key = t.Key,
          Value = t.Value
        });

      var viewModel = new IndexViewModel(assetsPrefix, metadata)
      {
        InlineHeadScript = AssetService.GetInlineHeadScript(),
        InlineHeadStyles = AssetService.GetInlineHeadStyles(assetsPrefix),
        TrackMeta = new TrackMeta(metadata),
        EvaluationTypes = evalTypeKVs,
        Years = yearKVs
      };

      #region *** for testing only (can delete) ***

      var types = EvaluationService.EvaluationTypes;
      var years = EvaluationService.GetYears();
      //var makes = EvaluationService.GetMakesByYear(years.First());
      //var trims = EvaluationService.GetTrimsByYearByMakeByEvaluationType(years.First(), makes.First(), types.First());

      //var drives = EvaluationService.GetFeaturesByTrimByFeatureType(trims.First(), FeatureType.Drives);
      //var engines = EvaluationService.GetFeaturesByTrimByFeatureType(trims.First(), FeatureType.Engines);
      //var trannies = EvaluationService.GetFeaturesByTrimByFeatureType(trims.First(), FeatureType.Transmissions);
      //var options = EvaluationService.GetFeaturesByTrimByFeatureType(trims.First(), FeatureType.Options);

      #endregion

      return View("Index", viewModel);
    }



    #region Services

    private static IEvaluationService EvaluationService
    {
      get { return ServiceLocator.Get<IEvaluationService>(); }
    }

    #endregion
  }
}
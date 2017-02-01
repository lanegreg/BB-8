using Car.Com.Common;
using Car.Com.Common.Api;
using Car.Com.Common.Cacheability;
using Car.Com.Controllers.Filters;
using Car.Com.Domain.Models.CarsForSale.Api;
using Car.Com.Domain.Models.CarsForSale.Filters;
using Car.Com.Domain.Models.Image;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api/article")]
  public class ArticleController : BaseApiController
  {
#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.Short)]
#endif
    [Route("relatedarticles/{contentId:int}", Name = "GetRelatedArticles"), HttpGet]
    public DataWrapper GetRelatedArticles(int contentId, int start = 1, int numRecords = 5, bool fillWithLatest = true)
    {

      var articles = ContentService.GetRelatedArticles(contentId, start, numRecords, fillWithLatest);

      return DataWrapper(new
      {
        relatedArticles = articles
      });
    }

    #region Services

    private static IVehicleContentService ContentService
    {
      get { return ServiceLocator.Get<IVehicleContentService>(); }
    }

    #endregion

  }
}

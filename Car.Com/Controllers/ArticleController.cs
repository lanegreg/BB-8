using Car.Com.Common;
using Car.Com.Common.Pagination;
using Car.Com.Common.SiteMeta;
using Car.Com.Domain.Models.Content;
using Car.Com.Domain.Models.SiteMeta;
using Car.Com.Domain.Services;
using Car.Com.Models.Article;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace Car.Com.Controllers
{
  public class TrackerVehicle
  {
    public string Make { get; set; }
    public string Model { get; set; }
    public string SuperModel { get; set; }
    public int Year { get; set; }
  }

  public class ArticleController : BaseController
  {
    /** 
     * This Controller's sole purpose is to handle all urls that belong to inventory car pages, nothing more!
     **/






    [Route("api/article/{id:minlength(1)}/{page:minlength(1)}", Name = "Article_Page"), HttpGet]
    public ActionResult ArticlePage(int contentId, int page)
    {
      var articlePageTask = VehicleContentService.GetArticlePageAsync(contentId, page);

      articlePageTask.Wait();

      var articlePage = articlePageTask.Result;

      return null;
    }






    


    #region Services

    private static IVehicleContentService VehicleContentService
    {
      get { return ServiceLocator.Get<IVehicleContentService>(); }
    }


    #endregion
  }
}
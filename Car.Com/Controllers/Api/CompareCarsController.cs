using Car.Com.Common.Api;
using Car.Com.Domain.Models.VehicleSpec;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api")]
  public class CompareCarsController : BaseApiController
  {

    [Route("compare/index/trimidlist/{trimIdList:minlength(1)}", Name = "GetCompareCarsIndexDataByTrimIdList"), HttpGet]
    public DataWrapper GetCompareCarsIndexDataByTrimIdList(string trimIdList)
    {

      try
      {
        if (trimIdList.Length < 1)
          return DataWrapper();

        var compareTrimData = VehicleSpecService.GetCompareCarsByTrimIdListAsync(trimIdList).Result;
        if (compareTrimData != null)
        {
          //var queryResults = compareTrimData;

          var response = compareTrimData.Select(t => new CompareTrim
          {
            Id = t.Id,
            Name = t.Name,
            SeoName = t.SeoName,
            Make = t.Make,
            MakeSeoName = t.MakeSeoName,
            SuperModel = t.SuperModel,
            SuperModelSeoName = t.SuperModelSeoName,
            Year = t.Year,
            Model = t.Model,
            IsNew = t.IsNew,
            Images = ImageMetaService.GetImagesByTrimIdAsync(t.Id).Result
              .Select(im => new Image
              {
                Small = String.Format("{0}_320x.png", im.UrlPrefix),
                Medium = String.Format("{0}_640x.png", im.UrlPrefix),
                Large = String.Format("{0}_1024x.png", im.UrlPrefix)
              })
          });

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("compare/results/trimidlist/{trimIdList:minlength(1)}", Name = "GetCompareCarsDataByTrimIdList"), HttpGet]
    public DataWrapper GetCompareCarsDataByTrimIdList(string trimIdList)
    {
 
      try
      {
        if (trimIdList.Length < 1)
        return DataWrapper();

        var compareTrimData = VehicleSpecService.GetCompareCarsByTrimIdListAsync(trimIdList).Result;
        if (compareTrimData != null)
        {
          //var queryResults = compareTrimData;

          var response = compareTrimData.Select(t => new CompareTrim
          {
            Id = t.Id,
            CategoryId = t.CategoryId,
            Name = t.Name,
            SeoName = t.SeoName,
            Make = t.Make,
            MakeSeoName = t.MakeSeoName,
            SuperModel = t.SuperModel,
            SuperModelSeoName = t.SuperModelSeoName,
            Year = t.Year,
            Model = t.Model,
            IsNew = t.IsNew,
            Msrp = String.Format("${0:0,0}", Convert.ToDouble(t.Msrp)),
            Invoice = String.Format("${0:0,0}", Convert.ToDouble(t.Invoice)),
            CityMpg = t.CityMpg,
            HighwayMpg = t.HighwayMpg,
            EngineType = t.EngineType,
            EngineSize = t.EngineSize,
            HorsePower = t.HorsePower,
            Specifications = t.Specifications,
            Images = ImageMetaService.GetImagesByTrimIdAsync(t.Id).Result
              .Select(im => new Image
              {
                Small = String.Format("{0}_320x.png", im.UrlPrefix),
                Medium = String.Format("{0}_640x.png", im.UrlPrefix),
                Large = String.Format("{0}_1024x.png", im.UrlPrefix)
             })
          });

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }
      
    }

    [Route("compare/makes", Name = "GetCompareCarMakes"), HttpGet]
    public DataWrapper GetCompareCarMakes()
    {

      try
      {
        var compareMakeList = VehicleSpecService.GetAllActiveMakesAsync().Result;
        if (compareMakeList != null)
        {

          var response = compareMakeList.Select(t => new CompareMakes
          {
            Name = t.Name,
            SeoName = t.SeoName,
          });

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("compare/make/{makeSeoName:minlength(3)}/super-models", Name = "GetCompareCarSuperModelByMakes"), HttpGet]
    public DataWrapper GetCompareCarSuperModelByMakes(string makeSeoName)
    {
      var makeTranslator = UriTokenTranslators.GetMakeTranslatorBySeoName(makeSeoName);
      if (makeTranslator == null)
        return DataWrapper();

      try
      {
        var compareSuperModelList = VehicleSpecService.GetNewSuperModelsByMakeAsync(makeSeoName).Result;
        if (compareSuperModelList != null)
        {

          var response = compareSuperModelList.Select(t => new CompareSuperModels
          {
            Make = t.Make,
            Name = t.Name,
            SeoName = t.SeoName,
          });

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("compare/make/{makeSeoName:minlength(3)}/super-model/{superModelSeoName:minlength(1)}/trims/", Name = "GetCompareCarTrimsByMakeBySuperModel"), HttpGet]
    public DataWrapper GetCompareCarTrimsByMakeBySuperModel(string makeSeoName, string superModelSeoName)
    {
      var makeTranslator = UriTokenTranslators.GetMakeTranslatorBySeoName(makeSeoName);
      var superModelTranslator = UriTokenTranslators.GetSuperModelTranslatorBySeoName(superModelSeoName);
      
      if (makeTranslator == null || superModelTranslator == null)
        return DataWrapper();

      try
      {
        var compareTrimList = VehicleSpecService.GetNewTrimsByMakeBySuperModelAsync(makeSeoName, superModelSeoName).Result;
        if (compareTrimList != null)
        {

          var response = compareTrimList
            .OrderByDescending(t => t.Year)
            .ThenBy(t => t.Make)
            .ThenBy(t => t.Model)
            .ThenBy(t => t.Name)
            .Select(t => new CompareListTrim
              {
                Name = t.Name,
                SeoName = t.SeoName,
                Year = t.Year,
                Id = t.Id,
                Msrp = t.Msrp
              });

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    #region Input / Output Models

    //output
    protected class CompareTrim
    {
      [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
      public int Id { get; set; }

      [JsonProperty("category_id", NullValueHandling = NullValueHandling.Ignore)]
      public int CategoryId { get; set; }

      [JsonProperty("name")]
      public String Name { get; set; }

      [JsonProperty("seo_name")]
      public string SeoName { get; set; }

      [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
      public string Make { get; set; }

      [JsonProperty("makeseoname", NullValueHandling = NullValueHandling.Ignore)]
      public string MakeSeoName { get; set; }

      [JsonProperty("super_model", NullValueHandling = NullValueHandling.Ignore)]
      public string SuperModel { get; set; }

      [JsonProperty("super_model_seoname", NullValueHandling = NullValueHandling.Ignore)]
      public string SuperModelSeoName { get; set; }

      [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
      public int Year { get; set; }

      [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
      public string Model { get; set; }

      [JsonProperty("is_new", NullValueHandling = NullValueHandling.Ignore)]
      public bool IsNew { get; set; }

      [JsonProperty("msrp", NullValueHandling = NullValueHandling.Ignore)]
      public string Msrp { get; set; }

      [JsonProperty("invoice", NullValueHandling = NullValueHandling.Ignore)]
      public string Invoice { get; set; }

      [JsonProperty("city_mpg", NullValueHandling = NullValueHandling.Ignore)]
      public string CityMpg { get; set; }

      [JsonProperty("highway_mpg", NullValueHandling = NullValueHandling.Ignore)]
      public string HighwayMpg { get; set; }

      [JsonProperty("engine_type", NullValueHandling = NullValueHandling.Ignore)]
      public string EngineType { get; set; }

      [JsonProperty("engine_size", NullValueHandling = NullValueHandling.Ignore)]
      public string EngineSize { get; set; }

      [JsonProperty("horsepower", NullValueHandling = NullValueHandling.Ignore)]
      public string HorsePower { get; set; }

      [JsonProperty("specifications", NullValueHandling = NullValueHandling.Ignore)]
      public IEnumerable<Specification> Specifications { get; set; }

      [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
      public IEnumerable<Image> Images { get; set; }
    }

    protected class Image
    {
      [JsonProperty("small", NullValueHandling = NullValueHandling.Ignore)]
      public string Small { get; set; }

      [JsonProperty("medium", NullValueHandling = NullValueHandling.Ignore)]
      public string Medium { get; set; }

      [JsonProperty("large", NullValueHandling = NullValueHandling.Ignore)]
      public string Large { get; set; }
    }

    protected class CompareMakes
    {
      [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
      public string Name { get; set; }

      [JsonProperty("seo_name", NullValueHandling = NullValueHandling.Ignore)]
      public string SeoName { get; set; }
    }

    protected class CompareSuperModels
    {
      [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
      public string Make { get; set; }

      [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
      public string Name { get; set; }

      [JsonProperty("seo_name", NullValueHandling = NullValueHandling.Ignore)]
      public string SeoName { get; set; }
    }

    protected class CompareListTrim
    {
      [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
      public string Name { get; set; }

      [JsonProperty("seo_name", NullValueHandling = NullValueHandling.Ignore)]
      public string SeoName { get; set; }

      [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
      public int Year { get; set; }

      [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
      public int Id { get; set; }

      [JsonProperty("msrp", NullValueHandling = NullValueHandling.Ignore)]
      public string Msrp { get; set; }
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

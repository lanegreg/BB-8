using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Car.Com.Common.Api;
using System.Web.Http;
using Car.Com.Domain.Models.VehicleSpec;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using Newtonsoft.Json;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api")]
  public class ResearchFilterController : BaseApiController
  {

    #region Category (filters)

    /** CLIENT_SIDE EXAMPLE:
     * 
     * $.post('/api/research-filter/categoryfiltergroups', 
     *    {'': "{categoryname: 'sedan'}"}, 
     *    function(data) {
     *      console.log(data);
     * });
     * 
     * REF: http://encosia.com/using-jquery-to-post-frombody-parameters-to-web-api/
     **/

    [Route("research-filter/categoryfiltergroups", Name = "GetCategoryFilterGroupDataByCategory"), HttpPost]
    public DataWrapper GetCategoryFilterGroupDataByCategory([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<CategoryFilterQuery>(queryStr);
      var categoryName = query.CategoryName;

      var categoryTranslator = UriTokenTranslators.GetCategoryTranslatorByName(categoryName);
      var categorySeoName = categoryTranslator.SeoName;

      try
      {
        var categoryFilterGroups = VehicleSpecService.GetCategoryFilterGroupDataByCategoryAsync(categorySeoName).Result;
        if (categoryFilterGroups != null)
        {
          var queryResults = categoryFilterGroups;

          var response = new CategoryFilterGroupResponse
          {
            CategoryFilterGroups = queryResults
          };

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    [Route("research-filter/categorytrimfilterattributes", Name = "GetCategoryTrimFilterAttributesByCategory"), HttpPost]
    public DataWrapper GetCategoryTrimFilterAttributesByCategory([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<CategoryFilterQuery>(queryStr);
      var categoryName = query.CategoryName;

      var categoryTranslator = UriTokenTranslators.GetCategoryTranslatorByName(categoryName);
      var categorySeoName = categoryTranslator.SeoName;

      try
      {
        var categoryTrimFilterAttributes = VehicleSpecService.GetCategoryTrimFilterAttributesByCategoryAsync(categorySeoName).Result;
        if (categoryTrimFilterAttributes != null)
        {
          var queryResults = categoryTrimFilterAttributes;

          var response = new CategoryTrimFilterAttributeResponse
          {
            CategoryTrimFilterAttributes = queryResults
          };

          return DataWrapper(response);
        }
        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    [Route("research-filter/vehicleattributeonly/categoryfiltergroups", Name = "GetCustomFilterGroupDataByVehicleAttributeName"), HttpPost]
    public DataWrapper GetCustomFilterGroupDataByVehicleAttributeName([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<VehicleAttributeFilterQuery>(queryStr);
      var vehicleAttributeName = query.VehicleAttributeSeoName;

      var vehicleAttributeTranslator = UriTokenTranslators.GetVehicleAttributeTranslatorBySeoName(vehicleAttributeName);
      var vehicleAttributeSeoName = vehicleAttributeTranslator.SeoName;

      try
      {
        var categoryFilterGroups = VehicleSpecService.GetCustomFilterGroupDataByVehicleAttributeNameAsync(vehicleAttributeSeoName).Result;
        if (categoryFilterGroups != null)
        {
          var queryResults = categoryFilterGroups;

          var response = new CategoryFilterGroupResponse
          {
            CategoryFilterGroups = queryResults
          };

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    [Route("research-filter/vehicleattributeonly/categorytrimfilterattributes", Name = "GetCustomTrimFilterAttributesByVehicleAttributeName"), HttpPost]
    public DataWrapper GetCustomTrimFilterAttributesByVehicleAttributeName([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<VehicleAttributeFilterQuery>(queryStr);
      var vehicleAttributeName = query.VehicleAttributeSeoName;

      var vehicleAttributeTranslator = UriTokenTranslators.GetVehicleAttributeTranslatorBySeoName(vehicleAttributeName);
      var vehicleAttributeSeoName = vehicleAttributeTranslator.SeoName;

      try
      {
        var categoryTrimFilterAttributes = VehicleSpecService.GetCustomTrimFilterAttributesByVehicleAttributeNameAsync(vehicleAttributeSeoName).Result;
        if (categoryTrimFilterAttributes != null)
        {
          var queryResults = categoryTrimFilterAttributes;

          var response = new CategoryTrimFilterAttributeResponse
          {
            CategoryTrimFilterAttributes = queryResults
          };

          return DataWrapper(response);
        }
        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    [Route("research-filter/vehicleattribute/categoryfiltergroups", Name = "GetCustomFilterGroupDataByCategoryAndVehicleAttributeName"), HttpPost]
    public DataWrapper GetCustomFilterGroupDataByCategoryAndVehicleAttributeName([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<VehicleAttributeFilterQuery>(queryStr);
      var categoryName = query.CategoryName;
      var vehicleAttributeName = query.VehicleAttributeSeoName;

      var categoryTranslator = UriTokenTranslators.GetCategoryTranslatorByName(categoryName);
      var categorySeoName = categoryTranslator.SeoName;
      var vehicleAttributeTranslator = UriTokenTranslators.GetVehicleAttributeTranslatorBySeoName(vehicleAttributeName);
      var vehicleAttributeSeoName = vehicleAttributeTranslator.SeoName;

      try
      {
        var categoryFilterGroups = VehicleSpecService.GetCustomFilterGroupDataByCategoryAndVehicleAttributeNameAsync(categorySeoName, vehicleAttributeSeoName).Result;
        if (categoryFilterGroups != null)
        {
          var queryResults = categoryFilterGroups;

          var response = new CategoryFilterGroupResponse
          {
            CategoryFilterGroups = queryResults
          };

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    [Route("research-filter/vehicleattribute/categorytrimfilterattributes", Name = "GetCustomTrimFilterAttributesByCategoryAndVehicleAttributeName"), HttpPost]
    public DataWrapper GetCustomTrimFilterAttributesByCategoryAndVehicleAttributeName([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<VehicleAttributeFilterQuery>(queryStr);
      var categoryName = query.CategoryName;
      var vehicleAttributeName = query.VehicleAttributeSeoName;

      var categoryTranslator = UriTokenTranslators.GetCategoryTranslatorByName(categoryName);
      var categorySeoName = categoryTranslator.SeoName;
      var vehicleAttributeTranslator = UriTokenTranslators.GetVehicleAttributeTranslatorBySeoName(vehicleAttributeName);
      var vehicleAttributeSeoName = vehicleAttributeTranslator.SeoName;

      try
      {
        var categoryTrimFilterAttributes = VehicleSpecService.GetCustomTrimFilterAttributesByCategoryAndVehicleAttributeNameAsync(categorySeoName, vehicleAttributeSeoName).Result;
        if (categoryTrimFilterAttributes != null)
        {
          var queryResults = categoryTrimFilterAttributes;

          var response = new CategoryTrimFilterAttributeResponse
          {
            CategoryTrimFilterAttributes = queryResults
          };

          return DataWrapper(response);
        }
        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    #endregion


    #region SuperModel (filters)

    /** CLIENT_SIDE EXAMPLE:
     * 
     * $.post('/api/research-filter/supermodelfiltergroups', 
     *    {'': "{makename: 'honda', supermodelname: 'fit'}"}, 
     *    function(data) {
     *      console.log(data);
     * });
     * 
     * REF: http://encosia.com/using-jquery-to-post-frombody-parameters-to-web-api/
     **/

    [Route("research-filter/supermodelfiltergroups", Name = "GetSuperModelFilterGroupDataByMakeSuperModel"), HttpPost]
    public DataWrapper GetSuperModelFilterGroupDataByMakeSuperModel([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<SuperModelFilterQuery>(queryStr);
      var makeName = query.MakeName;
      var superModelName = query.SuperModelName;

      try
      {
        var superModelFilterGroups = VehicleSpecService.GetSuperModelFilterGroupDataByMakeSuperModelAsync(makeName, superModelName).Result;
        if (superModelFilterGroups != null)
        {
          var queryResults = superModelFilterGroups;

          var response = new SuperModelFilterGroupResponse
          {
            SuperModelFilterGroups = queryResults
          };

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    [Route("research-filter/supermodeltrimfilterattributes", Name = "GetSuperModelTrimFilterAttributesByMakeSuperModel"), HttpPost]
    public DataWrapper GetSuperModelTrimFilterAttributesByMakeSuperModel([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<SuperModelFilterQuery>(queryStr);
      var makeName = query.MakeName;
      var superModelName = query.SuperModelName;

      try
      {
        var superModelTrimFilterAttributes = VehicleSpecService.GetSuperModelTrimFilterAttributesByMakeSuperModelAsync(makeName, superModelName).Result;
        if (superModelTrimFilterAttributes != null)
        {
          var queryResults = superModelTrimFilterAttributes;

          var response = new SuperModelTrimFilterAttributeResponse
          {
            SuperModelTrimFilterAttributes = queryResults
          };

          return DataWrapper(response);
        }
        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    [Route("research-filter/supermodelfiltergroupsbyyear", Name = "GetSuperModelFilterGroupDataByMakeSuperModelByYear"), HttpPost]
    public DataWrapper GetSuperModelFilterGroupDataByMakeSuperModelByYear([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<SuperModelFilterQueryByYear>(queryStr);
      var makeName = query.MakeName;
      var superModelName = query.SuperModelName;
      var year = query.Year;

      try
      {
        var superModelFilterGroups = VehicleSpecService.GetSuperModelFilterGroupDataByMakeSuperModelByYearAsync(makeName, superModelName, year).Result;
        if (superModelFilterGroups != null)
        {
          var queryResults = superModelFilterGroups;

          var response = new SuperModelFilterGroupResponse
          {
            SuperModelFilterGroups = queryResults
          };

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    [Route("research-filter/supermodeltrimfilterattributesbyyear", Name = "GetSuperModelTrimFilterAttributesByMakeSuperModelByYear"), HttpPost]
    public DataWrapper GetSuperModelTrimFilterAttributesByMakeSuperModelByYear([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<SuperModelFilterQueryByYear>(queryStr);
      var makeName = query.MakeName;
      var superModelName = query.SuperModelName;
      var year = query.Year;

      try
      {
        var superModelTrimFilterAttributes = VehicleSpecService.GetSuperModelTrimFilterAttributesByMakeSuperModelByYearAsync(makeName, superModelName, year).Result;
        if (superModelTrimFilterAttributes != null)
        {
          var queryResults = superModelTrimFilterAttributes;

          var response = new SuperModelTrimFilterAttributeResponse
          {
            SuperModelTrimFilterAttributes = queryResults
          };

          return DataWrapper(response);
        }
        return DataWrapper();
      }
      catch (Exception)
      {
        return DataWrapper();
      }

    }

    #endregion

    
    #region Input / Output Models

    //input
    protected class SuperModelFilterQuery
    {
      [JsonProperty("makename")]
      public string MakeName { get; set; }

      [JsonProperty("supermodelname")]
      public string SuperModelName { get; set; }
    }

    protected class SuperModelFilterQueryByYear
    {
      [JsonProperty("makename")]
      public string MakeName { get; set; }

      [JsonProperty("supermodelname")]
      public string SuperModelName { get; set; }

      [JsonProperty("year")]
      public int Year { get; set; }
    }

    protected class CategoryFilterQuery
    {
      [JsonProperty("categoryname")]
      public string CategoryName { get; set; }
    }

    protected class VehicleAttributeFilterQuery
    {
      [JsonProperty("categoryname")]
      public string CategoryName { get; set; }

      [JsonProperty("vehicleattributeseoname")]
      public string VehicleAttributeSeoName { get; set; }
    }

    //output
    protected class SuperModelFilterGroupResponse
    {
      [JsonProperty("supermodelfiltergroups")]
      public IEnumerable<ISuperModelFilterGroup> SuperModelFilterGroups { get; set; }
    }

    protected class SuperModelTrimFilterAttributeResponse
    {
      [JsonProperty("supermodeltrimfilterattributes")]
      public IEnumerable<ISuperModelTrimFilterAttribute> SuperModelTrimFilterAttributes { get; set; }
    }

    protected class CategoryFilterGroupResponse
    {
      [JsonProperty("categoryfiltergroups")]
      public IEnumerable<ICategoryFilterGroup> CategoryFilterGroups { get; set; }
    }

    protected class CategoryTrimFilterAttributeResponse
    {
      [JsonProperty("categorytrimfilterattributes")]
      public IEnumerable<ICategoryTrimFilterAttribute> CategoryTrimFilterAttributes { get; set; }
    }

    #endregion


    #region Services

    private static IVehicleSpecService VehicleSpecService
    {
      get { return ServiceLocator.Get<IVehicleSpecService>(); }
    }

    #endregion

  }
}

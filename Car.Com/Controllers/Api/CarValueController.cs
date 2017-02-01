using Car.Com.Common.Api;
using Car.Com.Domain.Models.Evaluation;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api/car-value-param")]
  public class CarValueController : BaseApiController
  {
    /** CLIENT_SIDE EXAMPLE:
     * 
        $.post('/api/car-value-param/makes/', {
            '': JSON.stringify({
              "eval_type": "PP",
              "year": 2013
            })
          },
          function(data) {
            console.log(data);
          });
     * 
     * REF: http://encosia.com/using-jquery-to-post-frombody-parameters-to-web-api/
     **/
     
    [Route("makes", Name = "GetCarValueMakesByEvalTypeByYear"), HttpPost]
    public DataWrapper GetCarValueMakesByEvalTypeByYear([FromBody] string jsonString)
    {
      Dto.Query query;
      try { query = JsonConvert.DeserializeObject<Dto.Query>(jsonString); }
      catch { throw new HttpResponseException(HttpStatusCode.NotFound); }

      int year;
      if (!Int32.TryParse(query.Year, out year) || year < 1990 || year > DateTime.Now.AddMonths(6).Year)
        throw new HttpResponseException(HttpStatusCode.NotFound);


      var makes = EvaluationService.GetMakesByYear(year)
        .Select(m => new Dto.Makes
        {
          Key = m.Key,
          Value = m.Value
        }).ToList();

      return DataWrapper(new {makes}, makes.Count());
    }


    /** CLIENT_SIDE EXAMPLE:
     * 
        $.post('/api/car-value-param/model-trims/', {
            '': JSON.stringify({
              "eval_type": "PP",
              "year": 2013,
              "make": "Ford"
            })
          },
          function(data) {
            console.log(data);
          });
     * 
     * REF: http://encosia.com/using-jquery-to-post-frombody-parameters-to-web-api/
     **/

    [Route("model-trims", Name = "GetCarValueTrimsByEvalTypeByYearByMake"), HttpPost]
    public DataWrapper GetCarValueTrimsByEvalTypeByYearByMake([FromBody] string jsonString)
    {
      Dto.Query query;
      try { query = JsonConvert.DeserializeObject<Dto.Query>(jsonString); }
      catch { throw new HttpResponseException(HttpStatusCode.NotFound); }

      int year;
      if (!Int32.TryParse(query.Year, out year) || year < 1990 || year > DateTime.Now.AddMonths(6).Year)
        throw new HttpResponseException(HttpStatusCode.NotFound);


      var modelTrims =
        EvaluationService.GetTrimsByYearByMakeByEvaluationType(year, query.Make, query.EvaluationType).ToList()
          .Select(m => new Dto.ModelTrims
          {
            Key = m.Key,
            Value = m.Value
          }).ToList();

      return DataWrapper(new { model_trims = modelTrims }, modelTrims.Count());
    }

    [Route("features", Name = "GetFeaturesByTrimByFeatureType"), HttpPost]
    public DataWrapper GetFeaturesByTrimByFeatureType([FromBody] string jsonString)
    {
      Dto.Query query;
      try { query = JsonConvert.DeserializeObject<Dto.Query>(jsonString); }
      catch { throw new HttpResponseException(HttpStatusCode.NotFound); }

      var drives = EvaluationService.GetFeaturesByTrimByFeatureType(query.Trim, FeatureType.Drives)
                  .Select(x => new Dto.Drives
                  {
                    Key = x.Key,
                    Value = x.Value,
                    PreSelect = x.PreSelect
                  }).ToList();
      var engines = EvaluationService.GetFeaturesByTrimByFeatureType(query.Trim, FeatureType.Engines)
                .Select(x => new Dto.Drives
                {
                  Key = x.Key,
                  Value = x.Value,
                  PreSelect = x.PreSelect
                }).ToList();
      var transmissions = EvaluationService.GetFeaturesByTrimByFeatureType(query.Trim, FeatureType.Transmissions)
                .Select(x => new Dto.Transmissions()
                {
                  Key = x.Key,
                  Value = x.Value,
                  PreSelect = x.PreSelect
                }).ToList();
      var vehicleoptions = EvaluationService.GetFeaturesByTrimByFeatureType(query.Trim, FeatureType.Options)
                .Select(x => new Dto.Options()
                {
                  Key = x.Key,
                  Value = x.Value,
                  PreSelect = x.PreSelect
                }).ToList();

      return DataWrapper(new { drives, engines, transmissions, vehicleoptions }, drives.Count());
      
    }

    [Route("vehiclevalue", Name = "GetVehicleValue"), HttpPost]
    public DataWrapper GetVehicleValue([FromBody] string jsonString)
    {
      Dto.Query query;
      try { query = JsonConvert.DeserializeObject<Dto.Query>(jsonString); }
      catch { throw new HttpResponseException(HttpStatusCode.NotFound); }

      var vehicleValue = EvaluationService.GetVehicleValue(query.Trim, query.EvaluationType, query.ConditionType,
        query.Mileage, query.PostalCode, query.EquipmentList);

      return DataWrapper(new { vehicleValue }, null);
      
    }

    public static class Dto
    {
      public abstract class KeyValuePairBase<T>
      {
        [JsonProperty("key")]
        public T Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("preselect")]
        public bool PreSelect { get; set; }
      }

      public class Query
      {
        [JsonProperty("eval_type")]
        public string EvaluationType { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("trim")]
        public string Trim { get; set; }

        [JsonProperty("condition_type")]
        public string ConditionType { get; set; }

        [JsonProperty("mileage")]
        public string Mileage { get; set; }

        [JsonProperty("postalcode")]
        public string PostalCode { get; set; }

        [JsonProperty("equipment_list")]
        public string EquipmentList { get; set; }
      }

      public class Makes : KeyValuePairBase<string> { }

      public class ModelTrims : KeyValuePairBase<string> { }

      public class Drives : KeyValuePairBase<string> { }

      public class Engines : KeyValuePairBase<string> { }

      public class Transmissions : KeyValuePairBase<string> { }

      public class Options : KeyValuePairBase<string> { }
    }



    #region Services

    private static IEvaluationService EvaluationService
    {
      get { return ServiceLocator.Get<IEvaluationService>(); }
    }

    #endregion
  }
}

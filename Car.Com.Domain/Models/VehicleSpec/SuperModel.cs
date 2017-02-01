using Car.Com.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class SuperModel : ISuperModel
  {
    [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
    public int Year { get; set; }

    [JsonProperty("years_json", NullValueHandling = NullValueHandling.Ignore)]
    public string YearsJson { get; set; }

    [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)] 
    public string Make { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("image_acode", NullValueHandling = NullValueHandling.Ignore)]
    public string ImageAcode { get; set; }

    [JsonProperty("starting_msrp", NullValueHandling = NullValueHandling.Ignore)]
    public string StartingMsrp { get; set; }

    [JsonProperty("seo_name", NullValueHandling = NullValueHandling.Ignore)]
    public string SeoName { get; set; }

    public string FormattedStartingMsrp
    {
      get
      {
        int msrp;
        
        if (StartingMsrp.IsNotNullOrEmpty() && Int32.TryParse(StartingMsrp, out msrp))
          return String.Format("{0:C}", msrp).Replace(".00", "");

        return String.Empty;
      }
    }

    public IEnumerable<Dto.Year> Years
    {
      get
      {
        var jsonArr = String.Format("[{0}]", YearsJson);
        return JsonConvert.DeserializeObject<IEnumerable<Dto.Year>>(jsonArr);
      }
    }

    public static class Dto
    {
      public class Year
      {
        [JsonProperty("is_new")]
        public bool IsNew { get; set; }

        [JsonProperty("year")]
        public int Number { get; set; }
      }
    }
  }
}

using Car.Com.Domain.Common;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class Trim : Entity, ITrim
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("seo_name")]
    public string SeoName { get; set; }

    [JsonProperty("image_path")]
    public string ImagePath { get; set; }
    

    [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
    public int Year { get; set; }

    [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
    public string Make { get; set; }

    [JsonProperty("makeseoname", NullValueHandling = NullValueHandling.Ignore)]
    public string MakeSeoName { get; set; }

    [JsonProperty("super_model", NullValueHandling = NullValueHandling.Ignore)]
    public string SuperModel { get; set; }

    [JsonProperty("super_model_seoname", NullValueHandling = NullValueHandling.Ignore)]
    public string SuperModelSeoName { get; set; }

    [JsonProperty("super_trim", NullValueHandling = NullValueHandling.Ignore)]
    public string SuperTrim { get; set; }

    [JsonProperty("msrp", NullValueHandling = NullValueHandling.Ignore)]
    public string Msrp { get; set; }




    [JsonProperty("is_new", NullValueHandling = NullValueHandling.Ignore)]
    public bool IsNew { get; set; }

    [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
    public string Model { get; set; }

    [JsonProperty("category_id", NullValueHandling = NullValueHandling.Ignore)]
    public int CategoryId { get; set; }

    [JsonProperty("canonical_seo_name", NullValueHandling = NullValueHandling.Ignore)]
    public string CanonicalSeoName { get; set; }

    [JsonProperty("acode", NullValueHandling = NullValueHandling.Ignore)]
    public string Acode { get; set; }

    [JsonProperty("abt_name", NullValueHandling = NullValueHandling.Ignore)]
    public string AbtName { get; set; }

    [JsonProperty("full_display_name", NullValueHandling = NullValueHandling.Ignore)]
    public string FullDisplayName { get; set; }


    #region Additional Detail Page Models

    #region Specification Related Properties

    [JsonProperty("specifications", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Specification> Specifications { get; set; }

    [JsonProperty("optionalspecs", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Specification> OptionalSpecs { get; set; }

    [JsonProperty("safetyitems", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Specification> SafetyItems { get; set; }

    [JsonProperty("colors", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<TrimColor> Colors { get; set; }

    [JsonProperty("incentives", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<TrimIncentive> Incentives { get; set; }

    #endregion

    #region Overview Related Properties
    [JsonProperty("invoice", NullValueHandling = NullValueHandling.Ignore)]
    public string Invoice { get; set; }

    [JsonProperty("city_mpg", NullValueHandling = NullValueHandling.Ignore)]
    public string CityMpg { get; set; }

    [JsonProperty("highway_mpg", NullValueHandling = NullValueHandling.Ignore)]
    public string HighwayMpg { get; set; }

    [JsonProperty("doors", NullValueHandling = NullValueHandling.Ignore)]
    public string Doors { get; set; }

    [JsonProperty("seating", NullValueHandling = NullValueHandling.Ignore)]
    public string Seating { get; set; }

    [JsonProperty("drivetrain", NullValueHandling = NullValueHandling.Ignore)]
    public string Drivetrain { get; set; }

    [JsonProperty("engine_type", NullValueHandling = NullValueHandling.Ignore)]
    public string EngineType { get; set; }

    [JsonProperty("engine_size", NullValueHandling = NullValueHandling.Ignore)]
    public string EngineSize { get; set; }

    [JsonProperty("body_style", NullValueHandling = NullValueHandling.Ignore)]
    public string BodyStyle { get; set; }

    [JsonProperty("fuel_type", NullValueHandling = NullValueHandling.Ignore)]
    public string FuelType { get; set; }

    [JsonProperty("drive_range", NullValueHandling = NullValueHandling.Ignore)]
    public string DriveRange { get; set; }

    [JsonProperty("towing_capacity", NullValueHandling = NullValueHandling.Ignore)]
    public string TowingCapacity { get; set; }

    [JsonProperty("cargo_capacity", NullValueHandling = NullValueHandling.Ignore)]
    public string CargoCapacity { get; set; }

    [JsonProperty("bedlength", NullValueHandling = NullValueHandling.Ignore)]
    public string BedLength { get; set; }

    [JsonProperty("horsepower", NullValueHandling = NullValueHandling.Ignore)]
    public string HorsePower { get; set; }

    [JsonProperty("roof_type", NullValueHandling = NullValueHandling.Ignore)]
    public string RoofType { get; set; }

    [JsonProperty("time_0_to_60", NullValueHandling = NullValueHandling.Ignore)]
    public string Time0To60 { get; set; }

    [JsonProperty("overview", NullValueHandling = NullValueHandling.Ignore)]
    public string Overview { get; set; }

    #endregion

    #endregion
  }
}

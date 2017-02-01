using System;
using System.Collections.Generic;
using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class CategoryTrimFilterAttribute : Entity, ICategoryTrimFilterAttribute
  {

    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public new int Id { get; set; }

    [JsonProperty("bodystyle", NullValueHandling = NullValueHandling.Ignore)]
    public string BodyStyle { get; set; }

    [JsonProperty("drivetrain", NullValueHandling = NullValueHandling.Ignore)]
    public string Drivetrain { get; set; }

    [JsonProperty("enginetype", NullValueHandling = NullValueHandling.Ignore)]
    public string EngineType { get; set; }

    [JsonProperty("enginesize", NullValueHandling = NullValueHandling.Ignore)]
    public string EngineSize { get; set; }

    [JsonProperty("fueltype", NullValueHandling = NullValueHandling.Ignore)]
    public string FuelType { get; set; }

    [JsonProperty("transmission", NullValueHandling = NullValueHandling.Ignore)]
    public string Transmission { get; set; }

    [JsonProperty("bedlength", NullValueHandling = NullValueHandling.Ignore)]
    public string BedLength { get; set; }

    [JsonProperty("towingcapacity", NullValueHandling = NullValueHandling.Ignore)]
    public string TowingCapacity { get; set; }

    [JsonProperty("seating", NullValueHandling = NullValueHandling.Ignore)]
    public string Seating { get; set; }

    [JsonProperty("cargocapacity", NullValueHandling = NullValueHandling.Ignore)]
    public string CargoCapacity { get; set; }

    [JsonProperty("navigation", NullValueHandling = NullValueHandling.Ignore)]
    public string Navigation { get; set; }

    [JsonProperty("driverange", NullValueHandling = NullValueHandling.Ignore)]
    public string DriveRange { get; set; }

    [JsonProperty("vehiclecategory", NullValueHandling = NullValueHandling.Ignore)]
    public string VehicleCategory { get; set; }

  }
}



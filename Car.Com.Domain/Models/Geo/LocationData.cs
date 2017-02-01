using System;
using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.Geo
{
  public class LocationData : Entity, ILocationData
  {
    [JsonProperty("ipaddress", NullValueHandling = NullValueHandling.Ignore)]
    public string IpAddress { get; set; }

    [JsonProperty("areacode", NullValueHandling = NullValueHandling.Ignore)]
    public int AreaCode { get; set; }

    [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
    public string City { get; set; }

    [JsonProperty("countrycode", NullValueHandling = NullValueHandling.Ignore)]
    public string CountryCode { get; set; }

    [JsonProperty("countryname", NullValueHandling = NullValueHandling.Ignore)]
    public string CountryName { get; set; }

    [JsonProperty("dmacode", NullValueHandling = NullValueHandling.Ignore)]
    public int DmaCode { get; set; }

    [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
    public double Latitude { get; set; }

    [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
    public double Longitude { get; set; }

    [JsonProperty("metrocode", NullValueHandling = NullValueHandling.Ignore)]
    public int MetroCode { get; set; }

    [JsonProperty("postalcode", NullValueHandling = NullValueHandling.Ignore)]
    public string PostalCode { get; set; }

    [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
    public string Region { get; set; }

    [JsonProperty("regionname", NullValueHandling = NullValueHandling.Ignore)]
    public string RegionName { get; set; }
  }
}

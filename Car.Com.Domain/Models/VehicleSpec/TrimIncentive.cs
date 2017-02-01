using System;
using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class TrimIncentive : Entity, ITrimIncentive
  {
    [JsonProperty("expires", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime Expires { get; set; }

    [JsonProperty("groupdesc", NullValueHandling = NullValueHandling.Ignore)]
    public string GroupDesc { get; set; }

    [JsonProperty("catdesc", NullValueHandling = NullValueHandling.Ignore)]
    public string CatDesc { get; set; }

    [JsonProperty("masterdesc", NullValueHandling = NullValueHandling.Ignore)]
    public string MasterDesc { get; set; }

    [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal Amount { get; set; }

    [JsonProperty("apr_24", NullValueHandling = NullValueHandling.Ignore)]
    public double APR_24 { get; set; }

    [JsonProperty("apr_36", NullValueHandling = NullValueHandling.Ignore)]
    public double APR_36 { get; set; }

    [JsonProperty("apr_48", NullValueHandling = NullValueHandling.Ignore)]
    public double APR_48 { get; set; }

    [JsonProperty("apr_60", NullValueHandling = NullValueHandling.Ignore)]
    public double APR_60 { get; set; }

    [JsonProperty("apr_72", NullValueHandling = NullValueHandling.Ignore)]
    public double APR_72 { get; set; }

    [JsonProperty("apr_84", NullValueHandling = NullValueHandling.Ignore)]
    public double APR_84 { get; set; }
  }
}

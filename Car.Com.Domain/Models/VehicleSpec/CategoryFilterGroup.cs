using System;
using System.Collections.Generic;
using Car.Com.Domain.Common;
using Newtonsoft.Json;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public class CategoryFilterGroup : Entity, ICategoryFilterGroup
  {
    [JsonProperty("filtergroupname", NullValueHandling = NullValueHandling.Ignore)]
    public string FilterGroupName { get; set; }

    [JsonProperty("svgid", NullValueHandling = NullValueHandling.Ignore)]
    public string SvgId { get; set; }

    [JsonProperty("displayname", NullValueHandling = NullValueHandling.Ignore)]
    public string DisplayName { get; set; }

    [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
    public string Code { get; set; }

  }
}

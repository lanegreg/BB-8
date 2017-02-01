using Newtonsoft.Json;
using System;

namespace Car.Com.Common.Environment
{
  public class CacheBustedAsset
  {
    [JsonProperty("page_name")]
    public string PageName { get; set; }

    [JsonProperty("hash")]
    public string Hash { get; set; }

    [JsonProperty("file_type")]
    public string FileType { get; set; }
    
    [JsonProperty("device_type")]
    public string DeviceType { get; set; }

    [JsonIgnore]
    public string CacheBustedAssetName
    {
      get { return String.Format("{0}.{1}-{2}.min.{3}", PageName, DeviceType, Hash, FileType); }
    }
  }
}
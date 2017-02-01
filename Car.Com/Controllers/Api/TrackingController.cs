using System;
using System.Web;
using Car.Com.Common.Api;
using Car.Com.Common.Cacheability;
using Car.Com.Common.Tracking;
using Car.Com.Controllers.Filters;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Newtonsoft.Json;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api/track")]
  public class TrackingController : BaseApiController
  {
    /** CLIENT_SIDE EXAMPLE:
     * 
        $.post('/api/track/meta', {
          '': JSON.stringify({
            affiliate_id: 24093,
            referrer: 'https://www.google.com/'
          })
        }, 
        function(data) {
          console.log(data);
        });
     * 
     **/

#if !DEBUG
    [ApiCacheability(TimeToLiveLevel = TimeToLiveLevel.None)]
#endif
    [Route("meta", Name = "GetTrackMeta"), HttpPost]
    public DataWrapper GetTrackMeta([FromBody] string jsonString)
    {
      var query = JsonConvert.DeserializeObject<Dto.Query>(jsonString);
      var thisHost = HttpContext.Current.Request.Url.Host;
      var affiliate = AffiliateService.GetAffiliateById(query.AffiliateId) ?? AffiliateService.GetAffiliateById(AffiliateService.CarDotComId);

      return DataWrapper(new Dto.TrackMeta
      {
        TrafficChannel = TrafficChannel.Resolve(affiliate, new Uri(query.Referrer).Host, thisHost),
        Affiliate = new Dto.Affiliate
        {
          Id = affiliate.Id,
          Name = affiliate.Name,
          GroupName = affiliate.GroupName
        }
      }, 1);
    }



    public static class Dto
    {
      public class Query
      {
        [JsonProperty("affiliate_id")]
        public int AffiliateId { get; set; }

        [JsonProperty("referrer")]
        public string Referrer { get; set; }
      }


      public class TrackMeta
      {
        [JsonProperty("traffic_channel")]
        public string TrafficChannel { get; set; }

        [JsonProperty("affiliate")]
        public Affiliate Affiliate { get; set; }
      }
      
      public class Affiliate
      {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("group_name")]
        public string GroupName { get; set; }
      }
    }
    

    #region Services

    private static IAffiliateService AffiliateService
    {
      get { return ServiceLocator.Get<IAffiliateService>(); }
    }

    #endregion
  }
}
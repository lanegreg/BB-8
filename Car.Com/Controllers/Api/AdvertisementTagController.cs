using Car.Com.Common.Advertisement;
using Car.Com.Common.Api;
using System.Linq;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api")]
  public class AdvertisementTagController : BaseApiController
  {
    [Route("ad-route/{routeName}/tags", Name = "GetAdvertisementTags"), HttpPost]
    public DataWrapper GetAdvertisementTags(string routeName, [FromBody] string pageCtx)
    {
      var generator = Router.GetGeneratorForRoute(routeName);
      var tags = generator.ProduceAdTags(pageCtx);
      var criteria = tags.First().Criteria;

      return DataWrapper(new
      {
        adSpots = tags,
        ctx = new
        {
          make = criteria.Make,
          model = criteria.Model,
          year = criteria.Year,
          type = criteria.Category,
          fuel = criteria.Fuel
        }
      });
    }

    /** CLIENT_SIDE EXAMPLE:
     * 
     * $.post('/api/ad-route/research.index/tags', 
     *    {'': "{'is_mobi': true,'make': 'ford', 'model': 'mustang', 'trim': 'trim_name', 'year': '2014'}"}, 
     *    function(data) {
     *      console.log(data);
     * });
     * 
     * REF: http://encosia.com/using-jquery-to-post-frombody-parameters-to-web-api/
     **/
  }
}

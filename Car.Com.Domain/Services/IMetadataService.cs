using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;
using System.Web;

namespace Car.Com.Domain.Services
{
  public interface IMetadataService
  {
    PageMeta GetPageMetaForUrl(string urlPath, IDictionary<string, string> tokenMaps);
    IMetadata GetMetadataForPage(HttpContextBase httpContext);
    IMetadata GetMetadataForPage(HttpContextBase httpContext, IDictionary<string, string> additionalTokenMaps);
  }
}

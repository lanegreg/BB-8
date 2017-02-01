using Car.Com.Domain.Models.Sitemap;
using System.Collections.Generic;
using System.Web;

namespace Car.Com.Domain.Services
{
  public interface ISitemapService
  {
    IEnumerable<string> GetSitemapSectionNames();
    IEnumerable<ISection> GetSections(HttpRequestBase request);
    IEnumerable<IPage> GetPagesBySectionName(HttpRequestBase request, string sectionName);
  }
}

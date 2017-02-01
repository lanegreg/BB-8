using System.Collections.Generic;

namespace Car.Com.Domain.Models.SiteMeta
{
  public class PageMeta
  {
    public bool WasMappedWithSeoMetadata { get; set; }
    public string Title { get; set; }
    public string Canonical { get; set; }
    public string Description { get; set; }
    public string H1 { get; set; }
    public string H2 { get; set; }
    public string LinkbackAnchorText { get; set; }
    public string LinkbackAnchorTitle { get; set; }
    public string Keywords { get; set; }

    public string SiteSection { get; set; }
    public string ContentSection { get; set; }
    public string SubSection { get; set; }
    public string PageName { get; set; }

    public IEnumerable<Breadcrumb> Breadcrumbs { get; set; }
  }
}
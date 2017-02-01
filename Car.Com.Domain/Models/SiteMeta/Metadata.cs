using Newtonsoft.Json;
using System.Collections.Generic;

namespace Car.Com.Domain.Models.SiteMeta
{
  public class Metadata : IMetadata
  {
    #region Interface Implementation

    public PageMeta PageMeta { get; set; }

    #endregion Interface Implementation

    #region Serialization Properties

    [JsonProperty]
    public string UriPattern { get; set; }

    private string _htmlTitle;

    [JsonProperty]
    public string HtmlTitle
    {
      get { return _htmlTitle.Replace("|_|", "\""); }
      set { _htmlTitle = value; }
    }

    private string _metaDescription;

    [JsonProperty]
    public string MetaDescription
    {
      get { return _metaDescription.Replace("|_|", "\""); }
      set { _metaDescription = value; }
    }

    [JsonProperty]
    public string Canonical { get; set; }

    private string _h1;

    [JsonProperty]
    public string H1
    {
      get { return _h1.Replace("|_|", "\""); }
      set { _h1 = value; }
    }

    private string _h2;

    [JsonProperty]
    public string H2
    {
      get { return _h2.Replace("|_|", "\""); }
      set { _h2 = value; }
    }

    [JsonProperty]
    public List<Keyword> Keywords { get; set; }

    [JsonProperty]
    public List<Keyword> SupportingKeywords { get; set; }

    private string _linkBackAnchorText;

    [JsonProperty]
    public string LinkBackAnchorText
    {
      get { return _linkBackAnchorText.Replace("|_|", "\""); }
      set { _linkBackAnchorText = value; }
    }

    private string _linkBackBreadcrumbText;

    [JsonProperty]
    public string LinkBackBreadcrumbText
    {
      get { return _linkBackBreadcrumbText.Replace("|_|", "\""); }
      set { _linkBackBreadcrumbText = value; }
    }

    private string _linkBackTitleAttr;

    [JsonProperty]
    public string LinkBackTitleAttr
    {
      get { return _linkBackTitleAttr.Replace("|_|", "\""); }
      set { _linkBackTitleAttr = value; }
    }

    [JsonProperty]
    public string SiteSection { get; set; }

    [JsonProperty]
    public string ContentSection { get; set; }

    [JsonProperty]
    public string SubSection { get; set; }

    [JsonProperty]
    public string PageName { get; set; }

    [JsonProperty]
    public List<Breadcrumb> Breadcrumbs { get; set; }

    #endregion Serialization Properties
  }
}

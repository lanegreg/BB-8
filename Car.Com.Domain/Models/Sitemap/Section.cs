using System;

namespace Car.Com.Domain.Models.Sitemap
{
  public class Section : ISection
  {
    public bool IsSecureConnection { get; set; }
    public string DomainAndPath { get; set; }

    public string Url
    {
      get { return String.Format("{0}://{1}/", IsSecureConnection ? "https" : "http", DomainAndPath); }
    }

    public string LastModified { get; set; }
  }
}

using System;

namespace Car.Com.Domain.Models.Sitemap
{
  public class Page : IPage
  {
    public bool IsSecureConnection { get; set; }
    public string DomainAndPath { get; set; }

    public string Url
    {
      get { return String.Format("{0}://{1}/", IsSecureConnection ? "https" : "http", DomainAndPath); }
    }
    
    public string LastModified { get; set; }
    public string Priority { get; set; }
    public string ChangeFrequency { get; set; }
  }
}

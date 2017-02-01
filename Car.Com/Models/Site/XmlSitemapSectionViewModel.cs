using System.Collections.Generic;

namespace Car.Com.Models.Site
{
  public class XmlSitemapSectionViewModel
  {
    public IEnumerable<Dto.Page> Pages { get; set; }


    public static class Dto
    {
      public class Page
      {
        public string Url { get; set; }
        public string LastModified { get; set; }
        public string Priority { get; set; }
        public string ChangeFrequency { get; set; }
      }
    }
  }
}
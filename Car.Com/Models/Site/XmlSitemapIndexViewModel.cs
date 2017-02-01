using System.Collections.Generic;

namespace Car.Com.Models.Site
{
  public class XmlSitemapIndexViewModel
  {
    public IEnumerable<Dto.Section> Sections { get; set; }


    public static class Dto
    {
      public class Section
      {
        public string Url { get; set; }
        public string LastModified { get; set; }
      }
    }
  }
}
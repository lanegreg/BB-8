using System.Collections.Generic;
using Car.Com.Domain.Models.Content;
using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Shared
{
  public class ArticleGalleryViewModel
  {
    public Car.Com.Domain.Models.Content.Article Article { get; set; }
    public ICollection<ArticleAd> ArticleAds { get; set; }
  }
}
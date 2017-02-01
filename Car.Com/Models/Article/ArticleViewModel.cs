using Car.Com.Domain.Models.Content;
using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;

namespace Car.Com.Models.Article
{
  public class ArticleViewModel : ViewModelBase
  {
    public ArticleViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public ArticleViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public Car.Com.Domain.Models.Content.Article Article { get; set; }
    public ICollection<ArticleAd> ArticleAds { get; set; }
  }
}
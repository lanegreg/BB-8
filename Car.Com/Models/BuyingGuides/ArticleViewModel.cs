using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;
using Car.Com.Domain.Models.Content;
using Car.Com.Models.Shared;

namespace Car.Com.Models.BuyingGuides
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

    public ArticleGalleryViewModel ArticleGallery { get; set; }

    public Car.Com.Domain.Models.Content.Article Article { get; set; }
    public IList<Domain.Models.Content.Article> CategoriesSectionArticles { get; set; }
    public ICollection<ArticleAd> ArticleAds { get; set; }
    public string VehicleCategoryName { get; set; }
    public string CategoryPluralName { get; set; }
  }
}
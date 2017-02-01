using System.Collections.Generic;
using Car.Com.Common.Pagination;
using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.BuyingGuides
{
  public class CategoryViewModel : ViewModelBase
  {
    public CategoryViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {}

    public CategoryViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {}

    public IList<Domain.Models.Content.Article> CategoriesSectionArticles { get; set; }
    //public Paginator Paginator { get; set; }
    public string VehicleCategoryName { get; set; }
    //public string CategoryPluralName { get; set; }
  }
}
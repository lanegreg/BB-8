﻿using System.Collections.Generic;
using Car.Com.Common.Pagination;
using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.BuyingGuides
{
  public class IndexViewModel : ViewModelBase
  {
    public IndexViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public IndexViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public IList<Domain.Models.Content.Article> BuyingGuides { get; set; }
    public IList<Domain.Models.Content.Article> MostRecentGroup { get; set; }
    public IList<Domain.Models.Content.Article> FinanceArticles { get; set; }
    public IList<Domain.Models.Content.Article> CategoriesArticles { get; set; }
    public IList<Domain.Models.Content.Article> CategoriesSectionArticles { get; set; }

    public Paginator Paginator { get; set; }

    //public string VehicleCategoryName { get; set; }
    //public string CategoryPluralName { get; set; }
  }
}
using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;

namespace Car.Com.Models.Site
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

    public IList<Car.Com.Domain.Models.Content.Article> BuyingGuides { get; set; }

  }
}
using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.CarsForSale
{
  public class SelectCategoryMakesViewModel : ViewModelBase
  {
    public SelectCategoryMakesViewModel(string assetsPrefix)
      : base(assetsPrefix)
    { }

    public SelectCategoryMakesViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    { }

  }
}
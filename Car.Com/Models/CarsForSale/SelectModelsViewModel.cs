using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.CarsForSale
{
  public class SelectModelsViewModel : ViewModelBase
  {
    public SelectModelsViewModel(string assetsPrefix)
      : base(assetsPrefix)
    { }

    public SelectModelsViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    { }

  }
}
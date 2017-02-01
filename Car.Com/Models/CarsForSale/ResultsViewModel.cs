using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.CarsForSale
{
  public class ResultsViewModel : ViewModelBase
  {
    public ResultsViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {}

    public ResultsViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {}

    public bool FirstLoad { get; set; }

  }
}
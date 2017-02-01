using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Site
{
  public class FraudViewModel : ViewModelBase
  {
    public FraudViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public FraudViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
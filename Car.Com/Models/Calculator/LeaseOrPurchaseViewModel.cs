using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Calculator
{
  public class LeaseOrPurchaseViewModel : ViewModelBase
  {
    public LeaseOrPurchaseViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public LeaseOrPurchaseViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
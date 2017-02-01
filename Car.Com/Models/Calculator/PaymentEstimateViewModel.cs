using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Calculator
{
  public class PaymentEstimateViewModel : ViewModelBase
  {
    public PaymentEstimateViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public PaymentEstimateViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
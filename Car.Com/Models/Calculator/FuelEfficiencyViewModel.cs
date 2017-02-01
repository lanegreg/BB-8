using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Calculator
{
  public class FuelEfficiencyViewModel : ViewModelBase
  {
    public FuelEfficiencyViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public FuelEfficiencyViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
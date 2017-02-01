using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Calculator
{
  public class AffordabilityViewModel : ViewModelBase
  {
    public AffordabilityViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public AffordabilityViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
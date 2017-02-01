using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Calculator
{
  public class AcceleratedPayoffViewModel : ViewModelBase
  {
    public AcceleratedPayoffViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public AcceleratedPayoffViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
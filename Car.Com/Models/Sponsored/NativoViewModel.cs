using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Sponsored
{
  public class NativoViewModel : ViewModelBase
  {
    public NativoViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public NativoViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
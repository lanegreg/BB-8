using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Site
{
  public class CopyrightViewModel : ViewModelBase
  {
    public CopyrightViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public CopyrightViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
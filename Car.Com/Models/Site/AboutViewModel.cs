using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Site
{
  public class AboutViewModel : ViewModelBase
  {
    public AboutViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public AboutViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
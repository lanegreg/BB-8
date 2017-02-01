using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Site
{
  public class PrivacyViewModel : ViewModelBase
  {
    public PrivacyViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public PrivacyViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
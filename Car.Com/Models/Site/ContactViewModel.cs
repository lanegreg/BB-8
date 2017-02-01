using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Site
{
  public class ContactViewModel : ViewModelBase
  {
    public ContactViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public ContactViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
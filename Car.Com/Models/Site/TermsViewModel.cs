using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Site
{
  public class TermsViewModel : ViewModelBase
  {
    public TermsViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public TermsViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
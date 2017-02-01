using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Site
{
  public class ToolsViewModel : ViewModelBase
  {
    public ToolsViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public ToolsViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
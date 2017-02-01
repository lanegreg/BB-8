using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.CompareCars
{
  public class ResultsViewModel : ViewModelBase
  {
    public ResultsViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public ResultsViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
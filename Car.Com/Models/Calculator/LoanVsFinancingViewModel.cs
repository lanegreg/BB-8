using Car.Com.Domain.Models.SiteMeta;

namespace Car.Com.Models.Calculator
{
  public class LoanVsFinancingViewModel : ViewModelBase
  {
    public LoanVsFinancingViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public LoanVsFinancingViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

  }
}
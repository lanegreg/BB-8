using Car.Com.Domain.Models.SiteMeta;
using Car.Com.Domain.Models.VehicleSpec;
using System.Collections.Generic;

namespace Car.Com.Models.HttpError
{
  public class ErrorViewModel : ViewModelBase
  {
    public ErrorViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public ErrorViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public IEnumerable<IMake> Makes { get; set; }
    public IEnumerable<ICategory> Categories { get; set; }
  }
}
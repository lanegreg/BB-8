using Car.Com.Domain.Models.SiteMeta;
using Car.Com.Domain.Models.VehicleSpec;
using System.Collections.Generic;

namespace Car.Com.Models.Research
{
  public class IndexViewModel : ViewModelBase
  {
    public IndexViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public IndexViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public IEnumerable<IMake> Makes { get; set; }
    public IEnumerable<ICategory> Categories { get; set; }
  }
}
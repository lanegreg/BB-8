using System.Collections.Generic;
using Car.Com.Domain.Models.SiteMeta;
namespace Car.Com.Models.Research
{
  public class AlternativeFuelViewModel : ViewModelBase
  {
    public AlternativeFuelViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public AlternativeFuelViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }
    
    public Dto.VehicleAttribute VehicleAttribute { get; set; }
    public IEnumerable<Dto.VehicleAttribute> VehicleAttributes { get; set; }
    
    public static class Dto
    {
      public class VehicleAttribute
      {
        public string Name { get; set; }
        public string SeoName { get; set; }
      }

    }
  }
}
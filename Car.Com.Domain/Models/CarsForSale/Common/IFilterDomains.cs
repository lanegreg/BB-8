using System.Collections.Generic;

namespace Car.Com.Domain.Models.CarsForSale.Common
{
  public interface IFilterDomains
  {
    IEnumerable<IFeature> Cylinders { get; }
    IEnumerable<IFeature> CityMpgs { get; }
    IEnumerable<IFeature> HighwayMpgs { get; }
    IEnumerable<IFeature> DriveTypes { get; }
    IEnumerable<IFeature> TrannyTypes { get; }
    IEnumerable<IFeature> Options { get; }
    IEnumerable<IFeature> FuelTypes { get; }
    IEnumerable<IFeature> Categories { get; }
    IEnumerable<IFeature> Years { get; }
    IEnumerable<IFeature> Makes { get; }
    IEnumerable<IFeature> GetModelsDomainByMakeId(int makeId);
    IEnumerable<IFeature> GetMakesDomainByCategoryId(int categoryId);
  }
}

namespace Car.Com.Domain.Models.VehicleSpec
{
  public interface ICategory
  {
    int Id { get; }
    string Name { get; }
    string PluralName { get; }
    string SeoName { get; }
  }
}
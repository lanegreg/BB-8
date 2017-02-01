
namespace Car.Com.Domain.Models.VehicleSpec
{
  public interface IMake
  {
    int Id { get; }
    string Name { get; }
    string SeoName { get; }
    string PluralName { get; }
    int AbtMakeId { get; }
    bool IsActive { get; }
  }
}
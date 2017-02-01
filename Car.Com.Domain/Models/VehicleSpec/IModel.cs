
namespace Car.Com.Domain.Models.VehicleSpec
{
  public interface IModel
  {
    int Id { get; }
    string Name { get; }
    string SeoName { get; }
    int MakeId { get; }
    int AbtMakeId { get; }
  }
}
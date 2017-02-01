
namespace Car.Com.Domain.Models.VehicleSpec
{
  public interface ISpecification
  {
    string Group { get; }
    string Title { get; }
    string Data { get; }
    string Availability { get; }
  }
}

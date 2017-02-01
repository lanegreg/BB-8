
namespace Car.Com.Domain.Models.Geo
{
  public interface ICity
  {
    string Name { get; }
    IState State { get; }
    ILatLngCombination Centroid { get; }
  }
}
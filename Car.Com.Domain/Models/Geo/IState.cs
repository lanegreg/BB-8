
namespace Car.Com.Domain.Models.Geo
{
  public interface IState
  {
    string Name { get; }
    string Abbreviation { get; }
    ILatLngCombination Centroid { get; }
  }
}
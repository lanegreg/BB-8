
namespace Car.Com.Domain.Models.Geo
{
  public class City : ICity
  {
    public string Name { get; set; }
    public IState State { get; set; }
    public ILatLngCombination Centroid { get; set; }
  }
}

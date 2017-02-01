
namespace Car.Com.Domain.Models.Geo
{
  public class State : IState
  {
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public ILatLngCombination Centroid { get; set; }
  }
}

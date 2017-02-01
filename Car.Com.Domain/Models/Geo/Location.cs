
namespace Car.Com.Domain.Models.Geo
{
  public class Location : ILocation
  {
    public string Address { get; set; }
    public ICity City { get; set; }
    public IZipcode Zipcode { get; set; }
    public ILatLngCombination Centroid { get; set; }
  }
}

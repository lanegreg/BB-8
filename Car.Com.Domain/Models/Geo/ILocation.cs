
namespace Car.Com.Domain.Models.Geo
{
  public interface ILocation
  {
    string Address { get; }
    ICity City { get; set; }
    IZipcode Zipcode { get; set; }
    ILatLngCombination Centroid { get; }
  }
}
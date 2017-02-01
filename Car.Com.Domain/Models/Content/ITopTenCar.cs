
namespace Car.Com.Domain.Models.Content
{
  public interface ITopTenCar
  {
    string Make { get; }
    string Model { get; }
    string Year { get; }
    string Blurb { get; }
    string ImageUrl { get; }
    int Ordinal { get; }
  }
}
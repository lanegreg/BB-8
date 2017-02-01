
namespace Car.Com.Domain.Models.Content
{
  public class TopTenCar : ITopTenCar
  {
    public string Make { get; set; }
    public string Model { get; set; }
    public string Year { get; set; }
    public string Blurb { get; set; }
    public string ImageUrl { get; set; }
    public int Ordinal { get; set; }
  }
}

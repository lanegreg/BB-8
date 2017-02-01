
namespace Car.Com.Domain.Models.CarsForSale
{
  public interface IDealer
  {
    int Id { get; }
    int ProgramId { get; }
    string Name { get; }
    string Phone { get; }
    string City { get; }
    string State { get; }
    int RevenueScore { get; }
    int AutoCheckId { get; }
    bool HasAutoCheckId { get; }
    bool IsPremiumPlacement { get; }
    string Message { get; set; }
    string Latitude { get; set; }
    string Longitude { get; set; }
  }
}
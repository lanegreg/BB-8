using Car.Com.Domain.Common;

namespace Car.Com.Domain.Models.CarsForSale
{
  public sealed class Dealer : Entity, IDealer
  {
    public int ProgramId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public int RevenueScore { get; set; }
    public int AutoCheckId { get; set; }
    public bool HasAutoCheckId { get { return AutoCheckId > 0; }}
    public bool IsPremiumPlacement { get; set; }
    public string Message { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
  }
}

using System.Collections.Generic;

namespace Car.Com.Domain.Models.CarsForSale
{
  public interface ICarForSale
  {
    int Id { get; }
    bool IsNew { get; }
    string Vin { get; }
    string SquishVin { get; }
    int MakeId { get; }
    string Make { get; }
    string Model { get; }
    int Year { get; }
    int TrimId { get; }
    string Trim { get; }
    string DisplayName { get; }

    int CategoryId { get; }
    string Category { get; }
    string DriveType { get; }
    int FuelTypeId { get; }
    int Mileage { get; }
    int AskingPrice { get; }
    string ExteriorColor { get; }
    string InteriorColor { get; }
    int Cylinders { get; }
    string NumOfDoors { get; }
    string TransmissionType { get; }
    string CityMpg { get; }
    string HighwayMpg { get; }
    string VehicleDetails { get; }
    string SellerNotes { get; }
    int DistanceInMiles { get; }

    IDealer Dealer { get; }
    IEnumerable<string> ImageUrls { get; }
    string PrimaryImage { get; set; }

    bool HasValidAskingPrice { get; }

    int IsNewStatus { get; }
    bool HasPrimaryImage { get; }
  }
}
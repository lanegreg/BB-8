using Car.Com.Common;
using Car.Com.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale
{
  /**
   * Return the CarsForSale recordset
   *
   * First Recordset Contract:
   *    { Id, ProgramId, DealerId, Vin, Year, MakeId, Make, ModelId, Model, Trim, DriveType, BodyStyleId, FuelTypeId, Mileage, 
   *      AskingPrice, ExteriorColor, Cylinders, TransmissionType, CityMpg, HwyMpg, FeatureBits, PipeSeparatedImageUrls }
   *
   **/
  public class CarForSale : Entity, ICarForSale
  {
    #region Interface Implementation

    public string Vin { get; set; }
    public string SquishVin { get; set; }
    public int MakeId { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public int TrimId { get; set; }
    public string Trim { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; }
    public string DisplayName { get; set; }
    public string DriveType { get; set; }
    public int FuelTypeId { get; set; }
    public int Mileage { get; set; }
    public int AskingPrice { get; set; }
    public string ExteriorColor { get; set; }
    public string InteriorColor { get; set; }
    public int Cylinders { get; set; }
    public string NumOfDoors { get; set; }
    public string TransmissionType { get; set; }
    public string CityMpg { get; set; }
    public string HighwayMpg { get; set; }
    public string VehicleDetails { get; set; }
    public string SellerNotes { get; set; }
    public int DistanceInMiles { get; set; }

    public IDealer Dealer { get; set; }

    public bool IsNew { get { return IsNewStatus == 1; } }

    // "PipeSeparatedImageUrls" is used to keep the hasImage flag properly interpreted based on original data.
    public bool HasPrimaryImage { get { return PipeSeparatedImageUrls.IsNotNullOrEmpty(); } }

    public IEnumerable<string> ImageUrls
    {
      get
      {
        return PipeSeparatedImageUrls.IsNotNullOrEmpty()
          ? PipeSeparatedImageUrls.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries).ToList()
          : new List<string>() { "/assets/svg/no-image-avail.svg" };
      }
      set { new List<string>().AddRange(value); }
    }

    private string _primaryImage;
    public string PrimaryImage
    {
      get { return _primaryImage ?? (_primaryImage = ImageUrls.Any() ? ImageUrls.First() : String.Empty); }
      set { _primaryImage = value; }
    }
    
    public bool HasValidAskingPrice
    {
      get { return AskingPrice > 1000; }
    }

    #endregion


    #region Service Visible Properties

    public int DealerId { get; set; }
    public int ProgramId { get; set; }
    public long OptionBits { get; set; }
    public string PipeSeparatedImageUrls { get; set; }
    public int ModelId { get; set; }
    public int IsNewStatus { get; set; }

    #endregion
  }
}

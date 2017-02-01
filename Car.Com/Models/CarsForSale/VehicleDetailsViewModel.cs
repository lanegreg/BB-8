using Car.Com.Common;
using Car.Com.Domain.Models.SiteMeta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Models.CarsForSale
{
  public class VehicleDetailsViewModel : ViewModelBase
  {
    public VehicleDetailsViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {}

    public VehicleDetailsViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {}


    public Dto.CarForSale CarForSale { get; set; }


    public static class Dto
    {
      public class CarForSale
      {
        public int InventoryId { get; set; }
        public int MakeId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Trim { get; set; }
        public string Vin { get; set; }
        public string Category { get; set; }
        public string DriveType { get; set; }
        public int FuelTypeId { get; set; }
        public int Mileage { get; set; }
        public bool HasMissingMileage { get; set; }
        public int AskingPrice { get; set; }
        public bool HasMissingPrice { get; set; }
        public string ExteriorColor { get; set; }
        public string InteriorColor { get; set; }
        public string Cylinders { get; set; }
        public string NumOfDoors { get; set; }
        public string TransmissionType { get; set; }
        public string CityMpg { get; set; }
        public string HighwayMpg { get; set; }
        public string VehicleDetails { get; set; }
        public string SellerNotes { get; set; }

        public Dealer Dealer { get; set; }
        public DealerHours DealerHours { get; set; }

        public IEnumerable<string> ImageUrls { get; set; }

        public string PrimaryImage
        {
          get { return ImageUrls.Any() ? ImageUrls.First() : String.Empty; }
        }

        public bool HasValidAskingPrice
        {
          get { return AskingPrice > 1000; }
        }

        public bool HasSellerNotes
        {
          get { return SellerNotes.Length > 0; }
        }

        public bool HasVehicleDetails
        {
          get { return VehicleDetails.Length > 0; }
        }

        public string DisplayTransmissionType
        {
          get
          {
            switch (TransmissionType)
            {
             case "AUTO":
                return "Automatic";

             case "MAN":
                return "Manual";

             default:
                return String.Empty;
            }
          }
        }
      }

      public class Dealer
      {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public bool IsTrusted { get; set; }
        public string Message { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string PhoneFormatted
        {
          get
          {
            return HasValidPhoneNumber
              ? String.Format("{0:(###) ###-####}", ConvertPhoneToLong(Phone))
              : String.Empty;
          }
        }

        public bool HasValidPhoneNumber
        {
          get
          {
            var hasValue = Phone.IsNotNullOrEmpty();
            return (hasValue && ConvertPhoneToLong(Phone) > 0);
          }
        }

        private static long ConvertPhoneToLong(string phone)
        {
          long phoneNum;
          var hasValidNumber = Int64.TryParse(phone, out phoneNum);
          return hasValidNumber ? phoneNum : -1;
        }

        public bool HasMessage { get { return Message.IsNotNullOrEmpty(); } }
        public bool AutonationDealer { get; set; }

      }

      public class DealerHours
      {
        public string SalesMonOpen { get; set; }
        public string SalesMonClose { get; set; }
        public string SalesTueOpen { get; set; }
        public string SalesTueClose { get; set; }
        public string SalesWedOpen { get; set; }
        public string SalesWedClose { get; set; }
        public string SalesThrOpen { get; set; }
        public string SalesThrClose { get; set; }
        public string SalesFriOpen { get; set; }
        public string SalesFriClose { get; set; }
        public string SalesSatOpen { get; set; }
        public string SalesSatClose { get; set; }
        public string SalesSunOpen { get; set; }
        public string SalesSunClose { get; set; }
      }
    }
  }
}
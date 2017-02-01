using System;

namespace Car.Com.Domain.Models.Geo
{
  public interface ILocationData
  {
    string IpAddress { get; }
    int AreaCode { get; }
    string City { get; }
    string CountryCode { get; }
    string CountryName { get; }
    int DmaCode { get; }
    double Latitude { get; }
    double Longitude { get; }
    int MetroCode { get; }
    string PostalCode { get; }
    string Region { get; }
    string RegionName { get; }
  }
}

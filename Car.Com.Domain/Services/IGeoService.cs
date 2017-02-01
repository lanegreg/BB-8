using System.Collections.Generic;
using Car.Com.Domain.Models.Geo;
using System.Threading.Tasks;

namespace Car.Com.Domain.Services
{
  public interface IGeoService
  {

    Task<ILocationData> GetLocationDataByIpAddressAsync(string ipaddress);
    ILocationData GetLocationDataByIpAddress(string ipaddress);
    Task<IEnumerable<ILocation>> GetLocationByZipcodeAsync(string zipcode);
    IEnumerable<ILocation> GetLocationByZipcode(string zipcode);
  }
}

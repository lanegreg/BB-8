using Car.Com.Common;
using Car.Com.Common.Cache;
using Car.Com.Domain.Models.Geo;
using Car.Com.Domain.Services;
using Car.Com.Service.Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Car.Com.Service.Rest.Common;


namespace Car.Com.Service.Data.Impl
{
  public sealed class GeoService : RestServiceBase<ServiceJsonEnvelope>, IGeoService
  {
    #region Declarations

    private static readonly string AbtProdConnString = WebConfig.Get<string>("ConnectionString:Abt_Prod");

    private const double LocalCacheTimeToLiveInSecs = 3600; // 1 Hour

    private static readonly Uri Endpoint;
    private static readonly string PathPrefix;
    #endregion

    #region ctors

    static GeoService()
    {
      PathPrefix = String.Format("/api/v{0}/geo/locationzip", WebConfig.Get<string>("GeoIpService:ApiVersion"));
      Endpoint = new Uri(String.Format("http://{0}", WebConfig.Get<string>("GeoIpService:Endpoint")));
    }

    public GeoService()
      : base(WebConfig.Get<int>("VehicleSpecService:Timeout_ms"))
    {}

    #endregion

    public async Task<ILocationData> GetLocationDataByIpAddressAsync
      (string ipaddress)
    {
      var localCacheKey =
        String.Format("location_data:by_ip[{0}]", ipaddress);

      // First, check LocalCache.
      var locationData = LocalCache.Get<LocationData>(localCacheKey);
      if (locationData != null)
        return locationData;

      var path =
        String.Format("/ip/{0}/", ipaddress);

      locationData = await FetchResource<LocationData>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);
      locationData.IpAddress = ipaddress;

      LocalCache.Put(localCacheKey, locationData, LocalCacheTimeToLiveInSecs);

      return locationData;
    }

    public ILocationData GetLocationDataByIpAddress
      (string ipaddress)
    {
      return GetLocationDataByIpAddressAsync(ipaddress).Result;
    }

     

    public async Task<IEnumerable<ILocation>> GetLocationByZipcodeAsync(string zipcode)
    {
      var localCacheKey =
        String.Format("lead_city_state:by_zipcode[{0}]", zipcode);

      // First, check LocalCache.
      var cityList = LocalCache.Get<ICollection<ILocation>>(localCacheKey);
      if (cityList != null)
        return cityList;


      // If not found in cache, get data and store in cache.
      cityList = GetCityListByZipcode(zipcode);
      LocalCache.Put(localCacheKey, cityList, LocalCacheTimeToLiveInSecs);
      return cityList;
    }

    public IEnumerable<ILocation> GetLocationByZipcode(string zipcode)
    {
      return GetLocationByZipcodeAsync(zipcode).Result;
    }


    #region Private Static Methods

    private static ICollection<ILocation> GetCityListByZipcode(string zipcode)
    {
      using (var conn = AbtProdDbConn())
      {
        // This gets us the city list for a given zipcode.
        var cityList = (ICollection<Dto.CityStateAbbrev>)conn.Query<Dto.CityStateAbbrev>("CCWeb.GetCityListByZipcode",
            new { Zipcode = zipcode },
            commandType: CommandType.StoredProcedure);

        var response = new List<ILocation>();

        foreach (var cItem in cityList)
        {
          response.Add(new Location()
          {
            Address = cItem.City,
            City = new City()
            {
              Name = cItem.City,
              State = new State(){Abbreviation = cItem.StateAbbreviation}
            }
          });
        }
        
        return response;
      }
    }

    #endregion



    public static class Dto
    {
      public class CityStateAbbrev
      {
        public string City { get; set; }
        public string StateAbbreviation { get; set; }
      }
    }

    #region Database Connections

    private static IDbConnection AbtProdDbConn()
    {
      return ServiceLocator.Get<IDbConnectionFactory>().CreateConnection(AbtProdConnString);
    }

    #endregion


    #region Local Memory Cache

    private static ILocalCache LocalCache
    {
      get { return ServiceLocator.Get<ILocalCache>(); }
    }

    #endregion

  }
}

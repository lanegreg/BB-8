using System.Collections.Specialized;
using System.Linq;
using Car.Com.Common;
using Car.Com.Common.Api;
using Car.Com.Domain.Models.VehicleSpec;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Car.Com.Domain.Services;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api")]
  public class ResearchController : BaseApiController
  {
    /** CLIENT_SIDE EXAMPLE:
     * 
     * $.post('/api/research-filter/supermodelfiltergroups', 
     *    {'': "{makename: 'honda', supermodelname: 'fit'}"}, 
     *    function(data) {
     *      console.log(data);
     * });
     * 
     * REF: http://encosia.com/using-jquery-to-post-frombody-parameters-to-web-api/
     **/

    [Route("research/trimincentives", Name = "GetTrimIncentiveDataByTrimIdZip"), HttpPost]
    public DataWrapper GetTrimIncentiveDataByTrimIdZip([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<TrimIncentivesQuery>(queryStr);
      var trimId = query.TrimId;
      var zip = query.Zip;

      try
      {
        var superModelFilterGroups = VehicleSpecService.GetTrimIncentivesByTrimIdByZipAsync(int.Parse(trimId), zip).Result;
        if (superModelFilterGroups != null)
        {
          var queryResults = superModelFilterGroups;

          var response = new TrimIncentiveResponse
          {
            TrimIncentive = queryResults
          };

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }
      
    }
     
    [Route("research/carsforsale", Name = "GetCarsForSaleByTrimId"), HttpPost]
    public DataWrapper GetCarsForSaleVehicles([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<SimilarVehicleQuery>(queryStr);
      var trimid = query.TrimId;

      try
      {
        var resultTrimList = new List<TrimListItem>();

        var trimslist = VehicleSpecService.GetSimilarTrimsByTrimIdAsync(trimid).Result;

        if (trimslist != null)
        {
          foreach (var t in trimslist)
          {
            int msrp;
            string msrpVar;

            msrpVar = (t.Msrp.IsNotNullOrEmpty() && Int32.TryParse(t.Msrp, out msrp))
              ? String.Format("{0:C}", msrp).Replace(".00", "")
              : String.Empty;

            resultTrimList.Add(new TrimListItem()
            {
              MakeSeo = t.MakeSeoName,
              SuperModelSeo = t.SuperModel,
              Year = t.Year.ToString(),
              StartingMsrp = msrpVar,
              FullDisplayName = t.FullDisplayName,
              TrimSeoName = t.SeoName,
              ImagePath = String.Format("{0}_320x.png", t.ImagePath)
            });
          }
          return DataWrapper(resultTrimList);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("research/similarvehicles", Name = "GetSimilarVehiclesByTrimId"), HttpPost]
    public DataWrapper GetSimilarVehiclesByTrimId([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<SimilarVehicleQuery>(queryStr);
      var trimid = query.TrimId;

      try
      {
        var resultTrimList = new List<TrimListItem>();

        var trimslist = VehicleSpecService.GetSimilarTrimsByTrimIdAsync(trimid).Result;

        if (trimslist != null)
        {
          foreach (var t in trimslist)
          {
            int msrp;
            string msrpVar;

            msrpVar = (t.Msrp.IsNotNullOrEmpty() && Int32.TryParse(t.Msrp, out msrp))
              ? String.Format("{0:C}", msrp).Replace(".00", "")
              : String.Empty;

            resultTrimList.Add(new TrimListItem()
            {
              Id = t.Id,
              Make = t.Make,
              MakeSeo = t.MakeSeoName,
              SuperModelSeo = t.SuperModel,
              Model = t.Model,
              Year = t.Year.ToString(),
              StartingMsrp = msrpVar,
              FullDisplayName = t.FullDisplayName,
              TrimSeoName = t.SeoName,
              Name = t.Name,
              ImagePath = String.Format("{0}_320x.png", t.ImagePath)
            });
          }
          return DataWrapper(resultTrimList);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("research/similarvehiclesbyprice", Name = "GetSimilarVehiclesByPrice"), HttpPost]
    public DataWrapper GetSimilarVehiclesByPrice([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<SimilarVehiclePriceQuery>(queryStr);
      var price = query.Price;

      try
      {
        var resultTrimList = new List<TrimListItem>();

        var trimslist = VehicleSpecService.GetSimilarTrimsByPriceAsync(price).Result;

        if (trimslist != null)
        {
          foreach (var t in trimslist)
          {
            int msrp;
            string msrpVar;

            msrpVar = (t.Msrp.IsNotNullOrEmpty() && Int32.TryParse(t.Msrp, out msrp))
              ? String.Format("{0:C}", msrp).Replace(".00", "")
              : String.Empty;

            resultTrimList.Add(new TrimListItem()
            {
              Id = t.Id,
              Make = t.Make,
              MakeSeo = t.MakeSeoName,
              SuperModelSeo = t.SuperModel,
              Model = t.Model,
              Year = t.Year.ToString(),
              StartingMsrp = msrpVar,
              FullDisplayName = t.FullDisplayName,
              TrimSeoName = t.SeoName,
              Name = t.Name,
              ImagePath = String.Format("{0}_320x.png", t.ImagePath)
            });
          }
          return DataWrapper(resultTrimList);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("research/viewedrecently/trimidlist/{trimIdList:minlength(1)}", Name = "GetViewedRecentlyByTrimIdList"), HttpGet]
    public DataWrapper GetViewedRecentlyByTrimIdList(string trimIdList)
    {

      try
      {
        if (trimIdList.Length < 1)
          return DataWrapper();

        var compareTrimData = VehicleSpecService.GetCompareCarsByTrimIdListAsync(trimIdList).Result;
        if (compareTrimData != null)
        {
          //var queryResults = compareTrimData;

          var response = compareTrimData.Select(t => new ViewedRecentlyTrim
          {
            Id = t.Id,
            Name = t.Name,
            SeoName = t.SeoName,
            Make = t.Make,
            MakeSeoName = t.MakeSeoName,
            SuperModel = t.SuperModel,
            SuperModelSeoName = t.SuperModelSeoName,
            Year = t.Year,
            Model = t.Model,
            IsNew = t.IsNew,
            Images = ImageMetaService.GetImagesByTrimIdAsync(t.Id).Result
              .Select(im => new Image
              {
                Small = String.Format("{0}_320x.png", im.UrlPrefix),
                Medium = String.Format("{0}_640x.png", im.UrlPrefix),
                Large = String.Format("{0}_1024x.png", im.UrlPrefix)
              })
          });

          return DataWrapper(response);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("research/supertrim/trimlist", Name = "GetTrimsByMakeBySuperModelByYear"), HttpPost]
    public DataWrapper GetTrimsByMakeBySuperModelByYear([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<TrimListQuery>(queryStr);
      var make = query.Make;
      var supermodel = query.Supermodel;
      var year = query.Year;
      var supertrim = query.Supertrim;

      try
      {
        var resultTrimList = new List<TrimListItem>();
        
        var trimslist = VehicleSpecService.GetTrimsByMakeBySuperModelByYearAsync(make, supermodel, year).Result
          .Where(t => t.SuperTrim == supertrim)
          .OrderBy(t => t.Msrp).ThenBy(t => t.FullDisplayName);

        if (trimslist != null)
        {
          foreach (var t in trimslist)
          {
            int msrp;
            string msrpVar;

            msrpVar = (t.Msrp.IsNotNullOrEmpty() && Int32.TryParse(t.Msrp, out msrp))
              ? String.Format("{0:C}", msrp).Replace(".00", "")
              : String.Empty;
            
            resultTrimList.Add(new TrimListItem() { MakeSeo = make, 
                                                    SuperModelSeo = supermodel, 
                                                    Year = year.ToString(),
                                                    StartingMsrp = msrpVar,
                                                    FullDisplayName = t.FullDisplayName,
                                                    TrimSeoName = t.SeoName});
          }
          return DataWrapper(resultTrimList);
        }
        
        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("research/getautowebads", Name = "GetAutowebAds"), HttpPost]
    public DataWrapper GetAutowebAds([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<AutowebAdsQuery>(queryStr);

      var make = query.Make;
      var model = query.Model;
      var zip = query.Zip;
      var campaignId = query.PublisherCampaignId;
      var userIpAddress = HttpContext.Current.Request.UserHostAddress;
      var userAgent = HttpContext.Current.Request.UserAgent;
      var urlReferer = HttpContext.Current.Request.UrlReferrer != null ? HttpContext.Current.Request.UrlReferrer.ToString() : "";

      if (string.IsNullOrEmpty(userAgent))
      {
        userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.101 Safari/537.36";
      }

      if (string.IsNullOrEmpty(urlReferer))
      {
        urlReferer = "http://www.car.com/";
      }

      if (!ValidIpAddress(userIpAddress))
      {
        userIpAddress = "207.8.110.64";
      }

      var urlPrefix = WebConfig.Get<string>("AdService:Autoweb:Endpoint") + "?format=json";
      var privateKey = WebConfig.Get<string>("AdService:Autoweb:PrivateKey");
      var sharedSecret = WebConfig.Get<string>("AdService:Autoweb:SharedSecret");

      string publisherInput = "{Make:" + make.ToLower() + 
        ",Model:" + model.ToLower() + ",ZipCode:" + zip + 
        ",DisplayCount:5,PublisherCampaignId:" + campaignId + 
        ",ProductCategoryID:1,NoTracking:1}";

      if (WebConfig.Get<string>("Environment:IsProduction") == "true")
      {
        publisherInput = "{Make:" + make.ToLower() + 
          ",Model:" + model.ToLower() + ",ZipCode:" + zip + 
          ",DisplayCount:5,PublisherCampaignId:" + campaignId + 
          ",ProductCategoryID:1}";
      }
      
      var url = urlPrefix + "&PublisherInput=" + publisherInput + "&UserIpAddress=" + userIpAddress + 
        "&UserAgent=" + HttpUtility.UrlEncode(userAgent) + "&UrlReferer=" + HttpUtility.UrlEncode(urlReferer);

      var awDate = DateTime.UtcNow.ToString("yyyy-MM-dd/HH\\:mm\\:ss");
      var awSignature = CreateHashedValue(awDate, privateKey);
      var awSharedSecret = sharedSecret;

      var request = (HttpWebRequest)WebRequest.Create(url);
      request.Method = "GET";
      request.UserAgent = userAgent;
      request.Headers.Add("awDate", awDate);
      request.Headers.Add("awSignature", awSignature);
      request.Headers.Add("awSharedSecret", awSharedSecret);

      string result = string.Empty;

      try
      {
        var response = request.GetResponse() as HttpWebResponse;
        if (response != null)
        {
          using (Stream stream = response.GetResponseStream())
          {
            if (stream != null)
            {
              var reader = new StreamReader(stream, Encoding.UTF8);
              result = reader.ReadToEnd();
            }
            else
            {
              result = "none";
            }
          }
        }
      }
      catch (Exception ex)
      {
        result = "none";
      }

      if (result == "none")
      {
        return DataWrapper();
      }

      var autowebAdResponse = new AutowebAdResponse {AutowebAdJson = result};
      return DataWrapper(autowebAdResponse);

    }

    [Route("research/freeautocheck", Name = "GetFreeAutocheck"), HttpPost]
    public DataWrapper FreeAutoCheckReport([FromBody] string queryStr)
		{
      var query = JsonConvert.DeserializeObject<AutoCheckQuery>(queryStr);

      var carForSale = CarsForSaleService.GetCarForSaleByDealerIdByInventoryId(query.DealerId, query.InventoryId);
      
			WebClient client = new WebClient();
			var values = new NameValueCollection
			{
				{ "VIN", carForSale.Vin },
				{ "CID", "7005279" },
				{ "PWD", "64ox9JFm" },
				{ "SID", carForSale.Dealer.AutoCheckId.ToString() }
			};
			byte[] responseArray = client.UploadValues("https://www.autocheck.com/DealerWebLink.jsp", values);
      var autoCheckResponse = new AutoCheckResponse { AutoCheckData = Encoding.ASCII.GetString(responseArray) };
      return DataWrapper(autoCheckResponse);
		}

    #region Input / Output Models

    //input
    protected class SimilarVehicleQuery
    {
      [JsonProperty("trimid")]
      public int TrimId { get; set; }
    }

    protected class SimilarVehiclePriceQuery
    {
      [JsonProperty("price")]
      public int Price { get; set; }
    }

    protected class TrimListQuery
    {
      [JsonProperty("make")]
      public string Make { get; set; }

      [JsonProperty("supermodel")]
      public string Supermodel { get; set; }

      [JsonProperty("year")]
      public int Year { get; set; }

      [JsonProperty("supertrim")]
      public string Supertrim { get; set; }
    }

    protected class TrimIncentivesQuery
    {
      [JsonProperty("trimid")]
      public string TrimId { get; set; }

      [JsonProperty("zip")]
      public string Zip { get; set; }
    }

    protected class AutowebAdsQuery
    {
      [JsonProperty("make")]
      public string Make { get; set; }

      [JsonProperty("model")]
      public string Model { get; set; }

      [JsonProperty("zip")]
      public string Zip { get; set; }

      [JsonProperty("useripaddress")]
      public string UserIpAddress { get; set; }

      [JsonProperty("publishercampaignid")]
      public string PublisherCampaignId { get; set; }

      [JsonProperty("useragent")]
      public string UserAgent { get; set; }

      [JsonProperty("urlreferer")]
      public string UrlReferer { get; set; }
    }

    protected class AutoCheckQuery
    {
      [JsonProperty("dealerId")]
      public int DealerId { get; set; }

      [JsonProperty("inventoryId")]
      public int InventoryId { get; set; }

    }

    //output
    protected class TrimListResponse
    {
      [JsonProperty("trimlist")]
      public IEnumerable<ITrim> Trim { get; set; }
    }

    protected class TrimListItem
    {
      [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
      public int Id { get; set; }

      [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
      public string Make { get; set; }

      [JsonProperty("make_seo", NullValueHandling = NullValueHandling.Ignore)]
      public string MakeSeo { get; set; }

      [JsonProperty("super_model_seo", NullValueHandling = NullValueHandling.Ignore)]
      public string SuperModelSeo { get; set; }

      [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
      public string Model { get; set; }

      [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
      public string Year { get; set; }

      [JsonProperty("starting_msrp", NullValueHandling = NullValueHandling.Ignore)]
      public string StartingMsrp { get; set; }

      [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
      public string Name { get; set; }

      [JsonProperty("full_display_name", NullValueHandling = NullValueHandling.Ignore)]
      public string FullDisplayName { get; set; }

      [JsonProperty("trim_seo_name", NullValueHandling = NullValueHandling.Ignore)]
      public string TrimSeoName { get; set; }

      [JsonProperty("image_path", NullValueHandling = NullValueHandling.Ignore)]
      public string ImagePath { get; set; }
    }

    protected class TrimIncentiveResponse
    {
      [JsonProperty("trimincentives")]
      public IEnumerable<TrimIncentive> TrimIncentive { get; set; }
    }

    protected class AutowebAdResponse
    {
      [JsonProperty("autowebadjson")]
      public string AutowebAdJson { get; set; }
    }

    protected class AutoCheckResponse
    {
      [JsonProperty("autocheckdata")]
      public string AutoCheckData { get; set; }
    }

    protected class ViewedRecentlyTrim
    {
      [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
      public int Id { get; set; }

      [JsonProperty("name")]
      public String Name { get; set; }

      [JsonProperty("seo_name")]
      public string SeoName { get; set; }

      [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
      public string Make { get; set; }

      [JsonProperty("makeseoname", NullValueHandling = NullValueHandling.Ignore)]
      public string MakeSeoName { get; set; }

      [JsonProperty("super_model", NullValueHandling = NullValueHandling.Ignore)]
      public string SuperModel { get; set; }

      [JsonProperty("super_model_seoname", NullValueHandling = NullValueHandling.Ignore)]
      public string SuperModelSeoName { get; set; }

      [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
      public int Year { get; set; }

      [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
      public string Model { get; set; }

      [JsonProperty("is_new", NullValueHandling = NullValueHandling.Ignore)]
      public bool IsNew { get; set; }

      [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
      public IEnumerable<Image> Images { get; set; }
    }

    protected class Image
    {
      [JsonProperty("small", NullValueHandling = NullValueHandling.Ignore)]
      public string Small { get; set; }

      [JsonProperty("medium", NullValueHandling = NullValueHandling.Ignore)]
      public string Medium { get; set; }

      [JsonProperty("large", NullValueHandling = NullValueHandling.Ignore)]
      public string Large { get; set; }
    }

    #endregion

    #region Services

    private static IVehicleSpecService VehicleSpecService
    {
      get { return ServiceLocator.Get<IVehicleSpecService>(); }
    }

    #endregion

    #region PrivateMethods

    private static string CreateHashedValue(string message, string privateKey)
    {
      privateKey = privateKey ?? "";
      var encoding = new System.Text.ASCIIEncoding();
      byte[] keyBytes = encoding.GetBytes(privateKey);
      byte[] messageBytes = encoding.GetBytes(message);
      using (var hmacsha256 = new HMACSHA256(keyBytes))
      {
        byte[] hashedMessageBytes = hmacsha256.ComputeHash(messageBytes);
        return Convert.ToBase64String(hashedMessageBytes);
      }
    }

    private static bool ValidIpAddress(string userIpAddress)
    {
 
      if (userIpAddress != null)
      {
        IPAddress testIpAddress;
        if (IPAddress.TryParse(userIpAddress, out testIpAddress))
        {
          var ipPartsArr = userIpAddress.Split('.');
          for (var x = 0; x < ipPartsArr.Count(); x++)
          {
            if (ipPartsArr[x].Length < 1)
              return false;
          }
          if (ipPartsArr.Count() == 4)
            return true;
        }
      }
      return false;
    }

    #endregion

    #region Services

    private static ICarsForSaleService CarsForSaleService
    {
      get { return ServiceLocator.Get<ICarsForSaleService>(); }
    }

    private static IImageMetaService ImageMetaService
    {
      get { return ServiceLocator.Get<IImageMetaService>(); }
    }

    #endregion

  }
}

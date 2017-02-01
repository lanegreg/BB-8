using System.Globalization;
using System.Web;
using Car.Com.Common;
using Car.Com.Common.Api;
using Car.Com.Domain.LeadEngine;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Car.Com.Service.Data.Impl;
using Car.Com.Service.Rest.Impl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  [RoutePrefix("api")]
  public class LeadEngineController : BaseApiController
  {
    [Route("leadengine/autonationusedcardealers", Name = "GetLeadEngineAutonationUsedCarDealers"), HttpGet]
    public DataWrapper GetLeadEngineAutonationUsedCarDealers()
    {
      try
      {
        var prDealerTask = LeadService.GetLeadAutonationUsedCarDealersAsync();
        prDealerTask.Wait();
        var prDealers = prDealerTask.Result;

        if (prDealers != null)
        {
          return DataWrapper(prDealers);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("leadengine/makes", Name = "GetLeadEngineMakes"), HttpGet]
    public DataWrapper GetLeadEngineMakes()
    {

      try
      {
        var newSuperTask = VehicleSpecService.GetAllNewSuperModelsAsync();
        var newMakeTask = VehicleSpecService.GetAllActiveMakesAsync();

        newSuperTask.Wait();
        newMakeTask.Wait();
        
        var resultMakeList = new List<LeadEngineMakes>();
        var newSuperModelList = newSuperTask.Result;
        var newMakeList = newMakeTask.Result;

        if (newMakeList != null && newSuperModelList != null)
        {
          foreach (var m in newMakeList)
          {
            if (newSuperModelList.FirstOrDefault(sm => sm.Make == m.Name) != null)
            {
              resultMakeList.Add(new LeadEngineMakes() { Name = m.Name, SeoName = m.SeoName });
            }
          }
          return DataWrapper(resultMakeList);
        }

        return DataWrapper();
      }
      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("leadengine/make/{makeSeoName:minlength(3)}/super-models", Name = "GetLeadEngineSuperModelByMakes"), HttpGet]
    public DataWrapper GetLeadEngineSuperModelByMakes(string makeSeoName)
    {
      var makeTranslator = UriTokenTranslators.GetMakeTranslatorBySeoName(makeSeoName);
      if (makeTranslator == null)
        return DataWrapper();

      try
      {
        var leadengineSuperModelList = VehicleSpecService.GetNewSuperModelsByMakeAsync(makeSeoName).Result;
        if (leadengineSuperModelList != null)
        {

          var response = leadengineSuperModelList.Select(t => new LeadEngineSuperModels
          {
            Make = t.Make,
            Name = t.Name,
            SeoName = t.SeoName,
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

    [Route("leadengine/make/{makeSeoName:minlength(3)}/super-model/{superModelSeoName:minlength(1)}/trims/", Name = "GetLeadEngineTrimsByMakeBySuperModel"), HttpGet]
    public DataWrapper GetLeadEngineTrimsByMakeBySuperModel(string makeSeoName, string superModelSeoName)
    {
      var makeTranslator = UriTokenTranslators.GetMakeTranslatorBySeoName(makeSeoName);
      var superModelTranslator = UriTokenTranslators.GetSuperModelTranslatorBySeoName(superModelSeoName);

      if (makeTranslator == null || superModelTranslator == null)
        return DataWrapper();

      try
      {
        var leadengineTrimList = VehicleSpecService.GetNewTrimsByMakeBySuperModelAsync(makeSeoName, superModelSeoName).Result;
        if (leadengineTrimList != null)
        {

          var response = leadengineTrimList
            .OrderByDescending(t => t.Year)
            .ThenBy(t => t.Make)
            .ThenBy(t => t.Model)
            .ThenBy(t => t.Name)
            .Select(t => new LeadEngineListTrim
              {
                Name = t.Name,
                SeoName = t.SeoName,
                LeadTrimName = (t.Name.IndexOf(t.Model) < 0) ? t.Name : t.Name.Remove(t.Name.IndexOf(t.Model), t.Model.Length).Trim(),
                FullDisplayName = t.FullDisplayName,
                Model = t.Model,
                Year = t.Year,
                Msrp = Convert.ToDouble(t.Msrp) > 0 ? String.Format("${0:0,0}", Convert.ToDouble(t.Msrp)) : "",
                Id = t.Id,
                Acode = t.Acode
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

    [Route("leadengine/citystate/{zipcode:minlength(5)}", Name = "GetLeadEnginecityStateByZipcode"), HttpGet]
    public DataWrapper GetLeadEnginecityStateByZipcode(string zipcode)
    {
      if (zipcode == null || zipcode.Length != 5)
        return DataWrapper();

      try
      {
        var cityStateList = GeoService.GetLocationByZipcodeAsync(zipcode).Result;
        if (cityStateList != null)
        {

          var response = cityStateList.Select(t => new LeadEngineCityStates
          {
            City = t.City.Name,
            StateAbbreviation = t.City.State.Abbreviation,
            StateName = t.City.State.Name
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

    [Route("leadengine/leadprinfo/{make:minlength(3)}/{model:minlength(1)}/{yearNumber:int}/{prnumber:int}/{fname}", Name = "GetLeadEnginePrInfoByPrNumber"), HttpGet]
    public DataWrapper GetLeadEnginePrInfoByPrNumber(string make, string model, int yearNumber, int prnumber, string fname = "")
    {
      var makeTranslator = UriTokenTranslators.GetMakeTranslatorByName(make);
      var yearTranslator = UriTokenTranslators.GetYearTranslatorByNumber(yearNumber);

      if (makeTranslator == null || yearTranslator == null || prnumber < 0)
        return DataWrapper();

      var prDealerTask = LeadService.GetLeadDealersByPrNumberAsync(prnumber);
      prDealerTask.Wait();
      var prDealers = prDealerTask.Result;

      List<int> autonationNewCarDealers = LeadService.GetLeadAutonationNewCarDealers();
      List<int> autonationUsedCarDealers = LeadService.GetLeadAutonationUsedCarDealers();
      
      try
      {
        var response = new LeadEnginePrInfo
          {
            FirstName = fname,
            Make = makeTranslator.Name,
            Model = model,
            Year = yearTranslator.Number,
            PrNumber = prnumber,
            Dealers = prDealers.Select(d => new LeadEnginePrDealers
            {
              Id = d.Id,
              Name = d.Name,
              Address = d.Address,
              City = d.City,
              State = d.State,
              Zip = d.Zip,
              Message = d.Message,
              Phone = d.Phone,
              LogoUrl = d.LogoUrl,
              LogoWidth = d.LogoWidth,
              LogoHeight = d.LogoHeight,
              AutonationNewCarDealer = autonationNewCarDealers.Contains(d.Id),
              AutonationUsedCarDealer = autonationUsedCarDealers.Contains(d.Id)
            })
          };

          return DataWrapper(response);
        }

      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("leadengine/leadmultiprinfo/{prnumberList:minlength(1)}/{fname}", Name = "GetLeadEngineMultiPrInfoByPrNumber"), HttpGet]
    public DataWrapper GetLeadEngineMultiPrInfoByPrNumber(string prnumberList, string fname = "")
    {
      if (prnumberList.Length < 1)
        return DataWrapper();

      var prDealerTask = LeadService.GetLeadDealersByPrNumberListAsync(prnumberList);
      prDealerTask.Wait();
      var prDealers = prDealerTask.Result;

      List<int> autonationNewCarDealers = LeadService.GetLeadAutonationNewCarDealers();
      List<int> autonationUsedCarDealers = LeadService.GetLeadAutonationUsedCarDealers();

      try
      {
        var response = new LeadEnginePrInfo
        {
          FirstName = fname,
          Make = "",
          Model = "",
          Year = 0,
          PrNumber = 0,
          Dealers = prDealers.Select(d => new LeadEnginePrDealers
          {
            Id = d.Id,
            Name = d.Name,
            Address = d.Address,
            City = d.City,
            State = d.State,
            Zip = d.Zip,
            Message = d.Message,
            Phone = d.Phone,
            LogoUrl = d.LogoUrl,
            LogoWidth = d.LogoWidth,
            LogoHeight = d.LogoHeight,
            ConfirmationNum = d.ConfirmationNum,
            AutonationNewCarDealer = autonationNewCarDealers.Contains(d.Id),
            AutonationUsedCarDealer = autonationUsedCarDealers.Contains(d.Id)
          })
        };

        return DataWrapper(response);
      }

      catch (Exception)
      {
        //if service returns no filter data...
        return DataWrapper();
      }

    }

    [Route("leadengine/selectedtrimimage/{trimid:int}", Name = "GetLeadEngineSelectedTrimImage"), HttpGet]
    public DataWrapper GetLeadEngineSelectedTrimImage(int trimid)
    {
      if (trimid < 1)
        return DataWrapper();

      try
      {
        var trimImageTask = ImageMetaService.GetImagesByTrimIdAsync(trimid);
        trimImageTask.Wait();
        var trimImage = trimImageTask.Result.FirstOrDefault();
        if (trimImage != null)
        {
          var response = new LeadEngineSelectedTrimImage();
          response.UrlPathPrefix = trimImage.UrlPrefix;
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

    [Route("leadengine/acode/{acode:minlength(5)}/trim", Name = "GetAbtTrimByAcode"), HttpGet]
    public DataWrapper GetAbtTrimByAcode(string acode)
    {
      if (acode == null)
        return DataWrapper();

      try
      {
        var trim = VehicleSpecService.GetAbtTrimsByAcodeAsync(acode).Result;

        if (trim != null)
        {

          var response = new LeadEngineAbtTrim
          {
            AbtName = trim.AbtName
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

    [Route("leadengine/ping-dealers",
      Name = "GetDealers"), HttpGet]
    public DataWrapper GetDealers(string year, string make, string model, string trim, string zipcode, int affiliateid = 24093)
    {
      LeadEnginePingResults pingResults;
      int ppcSourceGroupId = 2065;

      try
      {
        using (var dropZone = new DropZone())
        {
          var providerId = affiliateid;
          
          dropZone.Url = WebConfig.Get<string>("LeadService:Endpoint");

          var result = ppcSourceGroupId == 0
                         ? dropZone.Ping(providerId, Int32.Parse(year), make, model, trim, zipcode)
                         : dropZone.PingEx(providerId, Int32.Parse(year), make, model, trim, zipcode, ppcSourceGroupId);

          var dealerList = new List<int>();

          pingResults = new LeadEnginePingResults(result) { Make = make };
          if (pingResults.Coverage)
          {
            foreach (var dItem in pingResults.Dealers)
            {
              dealerList.Add(dItem.DealerID);
            }
            //var dealerCustomTask = DealerService.GetDisplayContentsByDealerIdsAsync(dealerList);
            //var texasAdTask = DealerService.GetTexasAdContentsByDealerIdsAsync(dealerList);
            //dealerCustomTask.Wait();
            //texasAdTask.Wait();
            //var dealerCustomData = dealerCustomTask.Result;
            //var texasAdContent = texasAdTask.Result;

            var dealerCustomData = DealerService.GetDisplayContentsByDealerIdsAsync(dealerList).Result;
            var texasAdContent = DealerService.GetTexasAdContentsByDealerIdsAsync(dealerList).Result;

            foreach (var dItem in pingResults.Dealers)
            {
              
              foreach (var cuItem in dealerCustomData)
              {
                if (dItem.DealerID == cuItem.DealerId)
                {

                  if (cuItem.DealerMessage.IsNotNullOrEmpty())
                  {
                    dItem.DealerMessageFlag = true;
                    dItem.DealerMessage = cuItem.DealerMessage;
                  }

                  if (cuItem.Logo.Url.IsNotNullOrEmpty())
                  {
                    dItem.LogoImageFlag = true;
                    dItem.LogoImage = cuItem.Logo.Url;

                    if (cuItem.Logo.Width > 230)
                    {
                      dItem.LogoWidthDisplay = "width=230";
                    }
                  }
                  break;
                }
              }
              
              foreach (var taItem in texasAdContent)
              {
                if (dItem.DealerID == taItem.DealerId)
                {
                  dItem.TexasAdFlag = true;
                  dItem.TexasImageUrl = taItem.ImageUrl;
                  dItem.SupplierAdDesc = taItem.SupplierAdDescription;
                  dItem.SupplierAdTypeID = taItem.SupplierAdTypeId;
                  dItem.ImageWidth = taItem.Image.Width;
                  dItem.ImageHeight = taItem.Image.Height;
                  dItem.FrameWidth = taItem.Frame.Width;
                  dItem.FrameHeight = taItem.Frame.Height;
                  dItem.TexasAdWidth = taItem.SupplierAdTypeId > 1 ? 350 + (5 * (taItem.SupplierAdTypeId - 1)) : 350;
                  dItem.TexasAdHeight = taItem.SupplierAdTypeId > 1 ? 380 + (6 * (taItem.SupplierAdTypeId - 1)) : 380;
                  break;
                }
              }

            }
          }
        }
      }
      catch (Exception e)
      {
        pingResults = new LeadEnginePingResults();

        if (e.InnerException != null && e.InnerException.InnerException != null)
          pingResults.ErrMessage = string.Format("Error: ({0})-({1})-({2})", e.InnerException.InnerException.Message, e.InnerException.Message, e.Message);
        else if (e.InnerException != null)
          pingResults.ErrMessage = string.Format("Error: ({0})-({1})", e.InnerException.Message, e.Message);
        else
          pingResults.ErrMessage = string.Format("Error: ({0})", e.Message);

        Log.Error("LeadEngine Ping:", e);
      }

      return DataWrapper(pingResults);
    }




    [Route("leadengine/post-lead",Name = "PostLead"), HttpPost]
    public DataWrapper PostLead([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<LeadEnginePostInput>(queryStr);

      int testAffiliateIdInt;
      string status;
      string leadId;
      var message = String.Empty;
      var bestContactTimeVal = ContactTime.NoPreference;
      var purchaseTimeFrameVal = TimeFrame.Within30Days;

      try
      {

        switch (query.contacttime)
        {
          case "NoPreference":
            bestContactTimeVal = ContactTime.NoPreference;
            break;
          case "Morning":
            bestContactTimeVal = ContactTime.Morning;
            break;
          case "Afternoon":
            bestContactTimeVal = ContactTime.Afternoon;
            break;
          case "Evening":
            bestContactTimeVal = ContactTime.Evening;
            break;
        }

        switch (query.timeframe)
        {
          case "Within30Days":
            purchaseTimeFrameVal = TimeFrame.Within30Days;
            break;
          case "Within48Hours":
            purchaseTimeFrameVal = TimeFrame.Within48Hours;
            break;
          case "Within14Days":
            purchaseTimeFrameVal = TimeFrame.Within14Days;
            break;
          case "Over30Days":
            purchaseTimeFrameVal = TimeFrame.Over30Days;
            break;
        }

        var lead = new Lead
        {
          Customer = new Customer
          {
            FirstName = query.firstname,
            LastName = query.lastname,
            Address1 = query.streetaddress,
            ZipCode = query.zipcode,
            HomePhone = query.phonenumber,
            EmailAddress = query.emailaddress,
            Comments = query.comments,
            BestContactTime = bestContactTimeVal,
            PurchaseTimeFrame = purchaseTimeFrameVal
          }
        };

        if (!String.IsNullOrWhiteSpace(query.contacttime))
          lead.Customer.BestContactTime = (ContactTime)Enum.Parse(typeof(ContactTime), query.contacttime.Replace(" ", ""));

        if (!String.IsNullOrWhiteSpace(query.timeframe))
          lead.Customer.PurchaseTimeFrame = (TimeFrame)Enum.Parse(typeof(TimeFrame), query.timeframe.Replace(" ", ""));

        lead.Vehicle = new Vehicle();

        // for used car set vehicle id
        // for new car set year, make, model, and trim           
        if (query.vehicleid > 0)
        {
          lead.Provider = !int.TryParse(query.affiliateid, out testAffiliateIdInt) || query.affiliateid.IsNullOrEmpty() || query.affiliateid.Trim() == "24093"
                 ? new Provider { ProviderID = 33079 } 
                 : new Provider { ProviderID = Int32.Parse(query.affiliateid) };
          lead.Vehicle.Status = VehicleStatus.Used;
          lead.Vehicle.VehicleID = query.vehicleid.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
          lead.Provider = !int.TryParse(query.affiliateid, out testAffiliateIdInt) || query.affiliateid.IsNullOrEmpty()
            ? new Provider {ProviderID = 24093}
            : new Provider {ProviderID = Int32.Parse(query.affiliateid)};
          lead.Vehicle.Status = VehicleStatus.New;
          lead.Vehicle.Year = Int32.Parse(query.year);
          lead.Vehicle.Make = query.make;
          lead.Vehicle.Model = query.model;
          lead.Vehicle.Trim = query.trim;
        }

        var dealerNum = (query.dealers == null ? 0 : query.dealers.Count());
        var dealersList = new Dealer[dealerNum];
        var dealerValues = new string[1];
        int dlItem = 0;

        foreach (var dItem in query.dealers)
        {
          dealerValues = dItem.dealer.Split('^');

          var dealer = new Dealer
          {
            DealerID = int.Parse(dealerValues[0]),
            DealerCode = dealerValues[1],
            ProgramID = int.Parse(dealerValues[2])
          };

          dealersList[dlItem++] = dealer;
        }

        lead.Dealers = dealersList;

        //submit leads
        using (var dropZone = new DropZone())
        {
          dropZone.Url = WebConfig.Get<string>("LeadService:Endpoint");
          var postResult = dropZone.PostCommOptions(lead, Convert.ToBoolean(query.optin), Convert.ToBoolean(query.optMkgBox));

          // check for end result
          if (postResult.Accepted)
          {
            status = "0";
            leadId = postResult.LeadID; // Send to Thank You page with LeadID for details
            message = string.Empty;
          }
          else if (postResult.Errors[0].Code == "DealerNotAvailable")
          {
            status = "1";
            leadId = postResult.LeadID; // Send to Thank You page but status 1 for No Dealer Match
            message = string.Empty;
          }
          else
          {
            status = "-1";
            leadId = string.Empty;
            message = postResult.Errors.Aggregate(message, (current, t) => current + "<br>" + t.Message);
          }
        }
      }
      catch (Exception e)
      {
        status = "-1";
        leadId = String.Empty;
        message = "System error, please try again.";

        Log.Error("LeadEngine Post:", e);
      }

      var response = new LeadEnginePostResults
      {
        Status = status,
        LeadId = leadId,
        Message = message
      };

      return DataWrapper(response);
      //return JsonCodec.EncodeJsonLeadPostResult(status, leadId, message);
    }

    [Route("leadengine/post-multi-lead", Name = "PostMultiLead"), HttpPost]
    public DataWrapper PostMultiLead([FromBody] string queryStr)
    {
      var query = JsonConvert.DeserializeObject<LeadEnginePostMultiInput>(queryStr);

      int testAffiliateIdInt;
      string status;
      string leadId;
      var message = String.Empty;
      var bestContactTimeVal = ContactTime.NoPreference;
      var purchaseTimeFrameVal = TimeFrame.Within30Days;
      // Will default the response status to 2 for Multi C4S Lead submit
      var response = new LeadEngineMultiPostResults
      {
        Status = "-1",
        PostResults = new List<LeadEnginePostResults>(),
        Message = ""
      };

      try
      {

        switch (query.contacttime)
        {
          case "NoPreference":
            bestContactTimeVal = ContactTime.NoPreference;
            break;
          case "Morning":
            bestContactTimeVal = ContactTime.Morning;
            break;
          case "Afternoon":
            bestContactTimeVal = ContactTime.Afternoon;
            break;
          case "Evening":
            bestContactTimeVal = ContactTime.Evening;
            break;
        }

        switch (query.timeframe)
        {
          case "Within30Days":
            purchaseTimeFrameVal = TimeFrame.Within30Days;
            break;
          case "Within48Hours":
            purchaseTimeFrameVal = TimeFrame.Within48Hours;
            break;
          case "Within14Days":
            purchaseTimeFrameVal = TimeFrame.Within14Days;
            break;
          case "Over30Days":
            purchaseTimeFrameVal = TimeFrame.Over30Days;
            break;
        }

        var lead = new Lead
        {
          Customer = new Customer
          {
            FirstName = query.firstname,
            LastName = query.lastname,
            Address1 = query.streetaddress,
            ZipCode = query.zipcode,
            HomePhone = query.phonenumber,
            EmailAddress = query.emailaddress,
            Comments = query.comments,
            BestContactTime = bestContactTimeVal,
            PurchaseTimeFrame = purchaseTimeFrameVal
          }
        };

        if (!String.IsNullOrWhiteSpace(query.contacttime))
          lead.Customer.BestContactTime = (ContactTime)Enum.Parse(typeof(ContactTime), query.contacttime.Replace(" ", ""));

        if (!String.IsNullOrWhiteSpace(query.timeframe))
          lead.Customer.PurchaseTimeFrame = (TimeFrame)Enum.Parse(typeof(TimeFrame), query.timeframe.Replace(" ", ""));

        lead.Vehicle = new Vehicle();

        // Multi Lead is for Used Car, only need to set vehicleid
          lead.Provider = !int.TryParse(query.affiliateid, out testAffiliateIdInt) || query.affiliateid.IsNullOrEmpty() || query.affiliateid.Trim() == "24093"
                 ? new Provider { ProviderID = 33079 }
                 : new Provider { ProviderID = Int32.Parse(query.affiliateid) };
          lead.Vehicle.Status = VehicleStatus.Used;
          lead.Vehicle.VehicleID = query.vehiclelist.First().vehicleid.ToString(CultureInfo.InvariantCulture);

        var dealerNum = (query.dealers == null ? 0 : query.dealers.Count());
        var dealersList = new Dealer[dealerNum];
        var dealerValues = new string[1];
        int dlItem = 0;

        foreach (var dItem in query.dealers)
        {
          dealerValues = dItem.dealer.Split('^');

          var dealer = new Dealer
          {
            DealerID = int.Parse(dealerValues[0]),
            DealerCode = dealerValues[1],
            ProgramID = int.Parse(dealerValues[2])
          };

          dealersList[dlItem++] = dealer;
        }

        lead.Dealers = dealersList;

        //submit leads
        using (var dropZone = new DropZone())
        {
          var dealerListExculde = new List<int>();
          var postResultList = new List<LeadEnginePostResults>();

          dropZone.Url = WebConfig.Get<string>("LeadService:Endpoint");

          foreach (var vehicle in query.vehiclelist)
          {
            if (!dealerListExculde.Contains(vehicle.dealerid))
            {
              dealerListExculde.Add(vehicle.dealerid);
              lead.Vehicle.VehicleID = vehicle.vehicleid.ToString(CultureInfo.InvariantCulture);
              lead.Customer.Comments = "";
              foreach (var addVehicle in query.vehiclelist)
              {
                if (addVehicle.vehicleid != vehicle.vehicleid && addVehicle.dealerid == vehicle.dealerid)
                {
                  lead.Customer.Comments = String.Format("{1} {0} Also interested in Vin# {2} - {3} {4} {5}", 
                        Environment.NewLine, lead.Customer.Comments, addVehicle.vin, addVehicle.year, addVehicle.make, addVehicle.model);
                }
              }
              var postResult = dropZone.PostCommOptions(lead, Convert.ToBoolean(query.optin), Convert.ToBoolean(query.optMkgBox));

              // check for end result
              if (postResult.Accepted)
              {
                // Send to Thank You page with LeadID for details
                response.Status = "2";
                postResultList.Add(new LeadEnginePostResults() { Status = "0", LeadId = postResult.LeadID, Message = string.Empty});
              }
              else if (postResult.Errors[0].Code == "DealerNotAvailable")
              {
                // Send to Thank You page but status 1 for No Dealer Match
                response.Status = "2";
                postResultList.Add(new LeadEnginePostResults() { Status = "1", LeadId = postResult.LeadID, Message = string.Empty });
              }
              else
              {
                postResultList.Add(new LeadEnginePostResults() { Status = "-1", LeadId = string.Empty, Message = postResult.Errors.Aggregate(message, (current, t) => current + "<br>" + t.Message) });
                response.Message = postResult.Errors.Aggregate(message, (current, t) => current + "<br>" + t.Message);
              }

            }
          }
          response.PostResults = postResultList;
        }
      }
      catch (Exception e)
      {
        status = "-1";
        leadId = String.Empty;
        message = "System error, please try again.";

        Log.Error("LeadEngine Post:", e);
      }


      return DataWrapper(response);
      //return JsonCodec.EncodeJsonLeadPostResult(status, leadId, message);
    }



    
    #region Input / Output Models

    //Input
    protected class LeadEnginePostInput
    {
      [JsonProperty("firstname", NullValueHandling = NullValueHandling.Ignore)]
      public string firstname { get; set; }

      [JsonProperty("lastname", NullValueHandling = NullValueHandling.Ignore)]
      public string lastname { get; set; }

      [JsonProperty("streetaddress", NullValueHandling = NullValueHandling.Ignore)]
      public string streetaddress { get; set; }

      [JsonProperty("zipcode", NullValueHandling = NullValueHandling.Ignore)]
      public string zipcode { get; set; }

      [JsonProperty("phonenumber", NullValueHandling = NullValueHandling.Ignore)]
      public string phonenumber { get; set; }

      [JsonProperty("emailaddress", NullValueHandling = NullValueHandling.Ignore)]
      public string emailaddress { get; set; }

      [JsonProperty("comments", NullValueHandling = NullValueHandling.Ignore)]
      public string comments { get; set; }

      [JsonProperty("contacttime", NullValueHandling = NullValueHandling.Ignore)]
      public string contacttime { get; set; }

      [JsonProperty("timeframe", NullValueHandling = NullValueHandling.Ignore)]
      public string timeframe { get; set; }

      [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
      public string year { get; set; }

      [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
      public string make { get; set; }

      [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
      public string model { get; set; }

      [JsonProperty("trim", NullValueHandling = NullValueHandling.Ignore)]
      public string trim { get; set; }

      [JsonProperty("optin", NullValueHandling = NullValueHandling.Ignore)]
      public int optin { get; set; }

      [JsonProperty("optMkgBox", NullValueHandling = NullValueHandling.Ignore)]
      public int optMkgBox { get; set; }

      [JsonProperty("vehicleid", NullValueHandling = NullValueHandling.Ignore)]
      public int vehicleid { get; set; }

      [JsonProperty("affiliateid", NullValueHandling = NullValueHandling.Ignore)]
      public string affiliateid { get; set; }

      [JsonProperty("dealers", NullValueHandling = NullValueHandling.Ignore)]
      public IEnumerable<LeadEnginePostDealers> dealers { get; set; }
    }

    protected class LeadEnginePostMultiInput
    {
      [JsonProperty("firstname", NullValueHandling = NullValueHandling.Ignore)]
      public string firstname { get; set; }

      [JsonProperty("lastname", NullValueHandling = NullValueHandling.Ignore)]
      public string lastname { get; set; }

      [JsonProperty("streetaddress", NullValueHandling = NullValueHandling.Ignore)]
      public string streetaddress { get; set; }

      [JsonProperty("zipcode", NullValueHandling = NullValueHandling.Ignore)]
      public string zipcode { get; set; }

      [JsonProperty("phonenumber", NullValueHandling = NullValueHandling.Ignore)]
      public string phonenumber { get; set; }

      [JsonProperty("emailaddress", NullValueHandling = NullValueHandling.Ignore)]
      public string emailaddress { get; set; }

      [JsonProperty("comments", NullValueHandling = NullValueHandling.Ignore)]
      public string comments { get; set; }

      [JsonProperty("contacttime", NullValueHandling = NullValueHandling.Ignore)]
      public string contacttime { get; set; }

      [JsonProperty("timeframe", NullValueHandling = NullValueHandling.Ignore)]
      public string timeframe { get; set; }

      [JsonProperty("trim", NullValueHandling = NullValueHandling.Ignore)]
      public string trim { get; set; }

      [JsonProperty("optin", NullValueHandling = NullValueHandling.Ignore)]
      public int optin { get; set; }

      [JsonProperty("optMkgBox", NullValueHandling = NullValueHandling.Ignore)]
      public int optMkgBox { get; set; }

      [JsonProperty("vehiclelist", NullValueHandling = NullValueHandling.Ignore)]
      public IEnumerable<LeadEnginePostMultiVehicles> vehiclelist { get; set; }

      [JsonProperty("affiliateid", NullValueHandling = NullValueHandling.Ignore)]
      public string affiliateid { get; set; }

      [JsonProperty("dealers", NullValueHandling = NullValueHandling.Ignore)]
      public IEnumerable<LeadEnginePostDealers> dealers { get; set; }
    }

    protected class LeadEnginePostDealers
    {
      [JsonProperty("dealer", NullValueHandling = NullValueHandling.Ignore)]
      public string dealer { get; set; }
    }

    protected class LeadEnginePostMultiVehicles
    {
      [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
      public string year { get; set; }

      [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
      public string make { get; set; }

      [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
      public string model { get; set; }

      [JsonProperty("vin", NullValueHandling = NullValueHandling.Ignore)]
      public string vin { get; set; }

      [JsonProperty("vehicleid", NullValueHandling = NullValueHandling.Ignore)]
      public int vehicleid { get; set; }

      [JsonProperty("dealerid", NullValueHandling = NullValueHandling.Ignore)]
      public int dealerid { get; set; }

    }


    //output
    protected class LeadEngineMakes
    {
      [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
      public string Name { get; set; }

      [JsonProperty("seo_name", NullValueHandling = NullValueHandling.Ignore)]
      public string SeoName { get; set; }
    }

    protected class LeadEngineAbtTrim
    {
      [JsonProperty("abt_name", NullValueHandling = NullValueHandling.Ignore)]
      public string AbtName { get; set; }
    }

    protected class LeadEngineSuperModels
    {
      [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
      public string Make { get; set; }

      [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
      public string Name { get; set; }

      [JsonProperty("seo_name", NullValueHandling = NullValueHandling.Ignore)]
      public string SeoName { get; set; }
    }

    protected class LeadEngineListTrim
    {
      [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
      public string Name { get; set; }

      [JsonProperty("seo_name", NullValueHandling = NullValueHandling.Ignore)]
      public string SeoName { get; set; }

      [JsonProperty("lead_trim_name", NullValueHandling = NullValueHandling.Ignore)]
      public string LeadTrimName { get; set; }

      [JsonProperty("full_display_name", NullValueHandling = NullValueHandling.Ignore)]
      public string FullDisplayName { get; set; }

      [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
      public string Model { get; set; }

      [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
      public int Year { get; set; }

      [JsonProperty("msrp", NullValueHandling = NullValueHandling.Ignore)]
      public string Msrp { get; set; }

      [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
      public int Id { get; set; }

      [JsonProperty("acode", NullValueHandling = NullValueHandling.Ignore)]
      public string Acode { get; set; }
    }

    protected class LeadEngineCityStates
    {
      [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
      public string City { get; set; }

      [JsonProperty("state_abb", NullValueHandling = NullValueHandling.Ignore)]
      public string StateAbbreviation { get; set; }

      [JsonProperty("state_name", NullValueHandling = NullValueHandling.Ignore)]
      public string StateName { get; set; }
    }

    protected class LeadEnginePrInfo
    {
      [JsonProperty("firstname", NullValueHandling = NullValueHandling.Ignore)]
      public string FirstName { get; set; }

      [JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
      public string Make { get; set; }

      [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
      public string Model { get; set; }

      [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
      public int Year { get; set; }

      [JsonProperty("prnumber", NullValueHandling = NullValueHandling.Ignore)]
      public int PrNumber { get; set; }

      [JsonProperty("dealers", NullValueHandling = NullValueHandling.Ignore)]
      public IEnumerable<LeadEnginePrDealers> Dealers { get; set; }

    }

    protected class LeadEnginePrDealers
    {
      [JsonProperty("confirmationnum", NullValueHandling = NullValueHandling.Ignore)]
      public int ConfirmationNum { get; set; }

      [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
      public int Id { get; set; }

      [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
      public string Name { get; set; }

      [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
      public string Address { get; set; }

      [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
      public string City { get; set; }

      [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
      public string State { get; set; }

      [JsonProperty("zip", NullValueHandling = NullValueHandling.Ignore)]
      public string Zip { get; set; }

      [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
      public string Message { get; set; }

      [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
      public string Phone { get; set; }

      [JsonProperty("logourl", NullValueHandling = NullValueHandling.Ignore)]
      public string LogoUrl { get; set; }

      [JsonProperty("logowidth", NullValueHandling = NullValueHandling.Ignore)]
      public string LogoWidth { get; set; }

      [JsonProperty("logoheight", NullValueHandling = NullValueHandling.Ignore)]
      public string LogoHeight { get; set; }

      [JsonProperty("autonationnewcardealer", NullValueHandling = NullValueHandling.Ignore)]
      public bool AutonationNewCarDealer { get; set; }

      [JsonProperty("autonationusedcardealer", NullValueHandling = NullValueHandling.Ignore)]
      public bool AutonationUsedCarDealer { get; set; }

    }

    protected class LeadEngineSelectedTrimImage
    {
      [JsonProperty("url_path_prefix", NullValueHandling = NullValueHandling.Ignore)]
      public string UrlPathPrefix { get; set; }
    }

    protected class LeadEnginePostResults
    {
      [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
      public string Status { get; set; }

      [JsonProperty("leadId", NullValueHandling = NullValueHandling.Ignore)]
      public string LeadId { get; set; }

      [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
      public string Message { get; set; }
    }

    protected class LeadEngineMultiPostResults
    {
      [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
      public string Status { get; set; }

      [JsonProperty("postresults")]
      public IEnumerable<LeadEnginePostResults> PostResults { get; set; }

      [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
      public string Message { get; set; }
    }

    protected class LeadEnginePingResults
    {
      [JsonProperty("dealers")]
      public IEnumerable<PingResultDealer> Dealers { get; set; }

      [JsonProperty("coverage")]
      public bool Coverage { get; set; }

      [JsonProperty("errmessage")]
      public string ErrMessage { get; set; }

      [JsonIgnore]
      public string Make { get; set; }
      public bool CheckAutobytelNewletterBox { get { return !(Make == "Chevrolet" || Make == "Cadillac" || Make == "GMC" || Make == "Buick"); } }

      public LeadEnginePingResults()
      {
        Coverage = false;
      }

      public LeadEnginePingResults(PingResult pingResult)
      {
        //PurchaseRequestService purchaseRequestService = new PurchaseRequestService(Configuration.AbtProdConnectionString);
        bool firstTrustedDealerChecked = false;
        List<PingResultDealer> resultDealers = new List<PingResultDealer>();

        foreach (Dealer dealerItem in pingResult.Dealers)
        {
          PingResultDealer resultDealer = new PingResultDealer(dealerItem, firstTrustedDealerChecked);

          resultDealers.Add(resultDealer);

          if (resultDealer.PreCheckedFlag)
          {
            firstTrustedDealerChecked = true;
          }
        }

        Dealers = resultDealers;
        Coverage = pingResult.Coverage;
      }

      public class PingResultDealer
      {
        public int DealerID { get; set; }
        public string DealerCode { get; set; }
        public int DealerTypeID { get; set; }
        public int ProgramID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Distance { get; set; }
        public bool TrustedDealer { get; set; }
        public bool PreCheckedFlag { get; set; }
        public string PreChecked { get; set; }
        public bool DealerMessageFlag { get; set; }
        public string DealerMessage { get; set; }
        public bool LogoImageFlag { get; set; }
        public string LogoImage { get; set; }
        public string LogoWidthDisplay { get; set; }
        public bool TexasAdFlag { get; set; }
        public string TexasImageUrl { get; set; }
        public string SupplierAdDesc { get; set; }
        public int SupplierAdTypeID { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int TexasAdWidth { get; set; }
        public int TexasAdHeight { get; set; }
        public bool PhoneLeadsFlag { get; set; }
        public string PhoneLeadsPhone { get; set; }
        public int AwardType { get; set; }
        public string DealerAwardText { get; set; }
        public bool IsAutoNationNewCarDealer { get; set; }
        public bool IsAutoNationUsedCarDealer { get; set; }

        public PingResultDealer(Dealer dealerItem, bool firstTrustedDealerChecked)
        {
          DealerID = dealerItem.DealerID;
          DealerCode = dealerItem.DealerCode;
          DealerTypeID = string.IsNullOrEmpty(dealerItem.DealerCode) ? 1 : 2;
          ProgramID = dealerItem.ProgramID;
          Name = dealerItem.Name;
          Address = dealerItem.Address;
          City = dealerItem.City;
          State = dealerItem.State;
          ZipCode = dealerItem.ZipCode;
          Distance = dealerItem.Distance;
          TrustedDealer = false;
          AwardType = 0;
          DealerAwardText = string.Empty;
          PreCheckedFlag = false;
          PreChecked = string.Empty;
          DealerMessageFlag = false;
          DealerMessage = string.Empty;
          LogoImageFlag = false;
          LogoImage = string.Empty;
          LogoWidthDisplay = string.Empty;
          TexasAdFlag = false;
          PhoneLeadsPhone = dealerItem.Phone;
          PhoneLeadsFlag = string.IsNullOrEmpty(PhoneLeadsPhone) ? false : true;
          if (PhoneLeadsFlag && PhoneLeadsPhone.Length == 10)
          {
            PhoneLeadsPhone = PhoneLeadsPhone.Insert(6, "-");
            PhoneLeadsPhone = PhoneLeadsPhone.Insert(3, "-");
          }

          IsAutoNationNewCarDealer = false;
          IsAutoNationUsedCarDealer = false;

          /*Trusted/Member Dealer ProgramIDs:
          1 New Car Leads 
          127 iControl 
          201 AutoUSA New Car Leads 
          216 New Car Campaigns 
          316 Dealix New Car Campaigns 
          327 Dealix New Car Leads
          */
          if ((ProgramID == 1) || (ProgramID == 127) || (ProgramID == 201) || (ProgramID == 216) || (ProgramID == 316) || (ProgramID == 327))
          {
            TrustedDealer = true;
            SetAutoNationNewCarDealer();
            SetAutoNationUsedCarDealer();
          }

          // pre-checked
          if (TrustedDealer && !firstTrustedDealerChecked)
          {
            PreChecked = "checked=checked";
            PreCheckedFlag = true;
          }
        }

        private void SetAutoNationNewCarDealer()
        {
          try
          {
            List<int> autonationDealers = LeadService.GetLeadAutonationNewCarDealers();
            if (autonationDealers != null && autonationDealers.Count > 0)
            {
              foreach (var autoNationDealer in autonationDealers)
              {
                if (autoNationDealer == DealerID)
                {
                  IsAutoNationNewCarDealer = true;
                }
              }
            }
         
          }
          catch (Exception e) {}
        }

        private void SetAutoNationUsedCarDealer()
        {
          try
          {
            List<int> autonationDealers = LeadService.GetLeadAutonationUsedCarDealers();
            if (autonationDealers != null && autonationDealers.Count > 0)
            {
              foreach (var autoNationDealer in autonationDealers)
              {
                if (autoNationDealer == DealerID)
                {
                  IsAutoNationUsedCarDealer = true;
                }
              }
            }
         
          }
          catch (Exception e) {}
        }

      }
    }

    #endregion



    #region Services

    private static IGeoService GeoService
    {
      get { return ServiceLocator.Get<IGeoService>(); }
    }

    private static IVehicleSpecService VehicleSpecService
    {
      get { return ServiceLocator.Get<IVehicleSpecService>(); }
    }

    private static IDealerService DealerService
    {
      get { return ServiceLocator.Get<IDealerService>(); }
    }

    private static IImageMetaService ImageMetaService
    {
      get { return ServiceLocator.Get<IImageMetaService>(); }
    }

    private static ILeadService LeadService
    {
      get { return ServiceLocator.Get<ILeadService>(); }
    }

    #endregion

  }
}

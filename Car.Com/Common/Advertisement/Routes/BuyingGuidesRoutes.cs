using Car.Com.Common.Advertisement.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Car.Com.Common.Advertisement.Routes
{
  public class BuyingGuidesRoutes
  {
    public static ITagGenerator GetAdTagGenerator(string action)
    {
      switch (action)
      {
        case "index":
        case "articlecategory":
        {
          return new Index();
        }
        case "article":
        {
          return new Article();
        }
      }
      return null;
    }

    public class Index : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Landing,
              Size = TagAs.Size.Desktop.Sz728X90
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Landing,
              Size = TagAs.Size.Desktop.Sz300X250
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Landing,
              Size = TagAs.Size.Desktop.Sz300X120
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.LandingPage,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.Landing,
            Size = TagAs.Size.Mobile.Sz320X50Flex
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.LandingPage,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.Landing,
            Size = TagAs.Size.Mobile.Sz320X51Flex
          }
        };
      }
    }

    public class Article : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria1 = new Criteria
        {
          Fuel = Criteria.FuelType.Gas,
          Make = (adMeta.ads[0].AdMake ?? string.Empty),
          Model = (adMeta.ads[0].AdModel ?? string.Empty),
          Year = (adMeta.ads[0].AdVehicleYear ?? string.Empty),
          Category = (adMeta.ads[0].AdCategory ?? string.Empty)
        };

        var criteria2 = new Criteria
        {
          Fuel = Criteria.FuelType.Gas,
          Make = (adMeta.ads[1].AdMake ?? string.Empty),
          Model = (adMeta.ads[1].AdModel ?? string.Empty),
          Year = (adMeta.ads[1].AdVehicleYear ?? string.Empty),
          Category = (adMeta.ads[1].AdCategory ?? string.Empty)
        };

        var criteria3 = new Criteria
        {
          Fuel = Criteria.FuelType.Gas,
          Make = (adMeta.ads[2].AdMake ?? string.Empty),
          Model = (adMeta.ads[2].AdModel ?? string.Empty),
          Year = (adMeta.ads[2].AdVehicleYear ?? string.Empty),
          Category = (adMeta.ads[2].AdCategory ?? string.Empty)
        };

        var criteria4 = new Criteria
        {
          Fuel = Criteria.FuelType.Gas,
          Make = (adMeta.ads[3].AdMake ?? string.Empty),
          Model = (adMeta.ads[3].AdModel ?? string.Empty),
          Year = (adMeta.ads[3].AdVehicleYear ?? string.Empty),
          Category = (adMeta.ads[3].AdCategory ?? string.Empty)
        };

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Landing,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria1
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Landing,
              Size = TagAs.Size.Desktop.Sz300X250,
              Criteria = criteria2
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Landing,
              Size = TagAs.Size.Desktop.Sz300X120,
              Criteria = criteria3
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Landing,
              Size = TagAs.Size.Desktop.Sz400X40,
              Criteria = criteria4
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.LandingPage,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.Landing,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria1
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.LandingPage,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.Landing,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria2
          }
        };
      }
    }
  }
}
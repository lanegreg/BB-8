using Car.Com.Common.Advertisement.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Car.Com.Common.Advertisement.Routes
{
  public class SiteRoutes
  {
    public static ITagGenerator GetAdTagGenerator(string action)
    {
      switch (action)
      {
        case "index":
        case "about":
        case "contact":
        case "copyright":
        case "fraud":
        case "privacy":
        case "sitemap":
        case "terms":
        {
          return new FooterPage();
        }

        case "tools":
        {
          return new Tools();
        }
      }

      return null;
    }


    public class FooterPage : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria = new Criteria
        {
          Year = DateTime.Now.Year.ToString()
        };

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = TagAs.FirstLevel.Home,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.None,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Home,
              Size = TagAs.Size.Desktop.Sz728X90Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.Home,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.None,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Home,
              Size = TagAs.Size.Desktop.Sz300X250,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.Home,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.None,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Home,
              Size = TagAs.Size.Desktop.Sz300X120,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.Home,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.None,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.Home,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.Home,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.None,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.Home,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }
    

    public class Tools : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria = new Criteria
        {
          Year = DateTime.Now.Year.ToString()
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
              Size = TagAs.Size.Desktop.Sz728X90Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Landing,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
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
              Criteria = criteria
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
            Criteria = criteria
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
            Criteria = criteria
          }
        };
      }
    }
  }
}
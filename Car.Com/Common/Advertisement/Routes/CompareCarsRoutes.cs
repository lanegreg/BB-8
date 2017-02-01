using System;
using Car.Com.Common.Advertisement.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Car.Com.Common.Advertisement.Routes
{
  public class CompareCarsRoutes
  {
    public static ITagGenerator GetAdTagGenerator(string action)
    {
      switch (action)
      {
        case "index":
        {
          return new Index();
        }

        case "results":
        {
          return new Results();
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
              Section = TagAs.Section.Comparison,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Comparison,
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
              Section = TagAs.Section.Comparison,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          //new AdSpot
          //{
          //  Tile = TagAs.Tile.Ten,
          //  FirstLevel = TagAs.FirstLevel.New,
          //  Device = TagAs.Device.Mobile,
          //  SecondLevel = TagAs.SecondLevel.LandingPage,
          //  ThirdLevel = TagAs.ThirdLevel.None,
          //  Section = TagAs.Section.Comparison,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //    Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.LandingPage,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.Comparison,
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
            Section = TagAs.Section.Comparison,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
              Criteria = criteria
          }
        };
      }
    }

    public class Results : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria = new Criteria
        {
          Make = adMeta.make,
          Model = adMeta.model,
          Year = adMeta.year,
          Category = adMeta.category
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
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.Comparison,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.Comparison,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.Comparison,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.Comparison,
              Size = TagAs.Size.Desktop.Sz400X40,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          //new AdSpot
          //{
          //  Tile = TagAs.Tile.Ten,
          //  FirstLevel = TagAs.FirstLevel.New,
          //  Device = TagAs.Device.Mobile,
          //  SecondLevel = TagAs.SecondLevel.None,
          //  ThirdLevel = TagAs.ThirdLevel.None,
          //  Section = TagAs.Section.Comparison,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //    Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Category,
            Section = TagAs.Section.Comparison,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
              Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Category,
            Section = TagAs.Section.Comparison,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
              Criteria = criteria
          }
        };
      }
    }
  }
}
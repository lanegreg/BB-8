﻿using Car.Com.Common.Advertisement.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Car.Com.Common.Advertisement.Routes
{
  public class CarValueRoutes
  {
    public static ITagGenerator GetAdTagGenerator(string action)
    {
      switch (action)
      {
        case "index":
          {
            return new Index();
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
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.None,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.None,
              Size = TagAs.Size.Desktop.Sz728X90
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.None,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.None,
              Size = TagAs.Size.Desktop.Sz300X250Flex
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.None,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.None,
              Size = TagAs.Size.Desktop.Sz300X251
            }
          };
        }

        return new[]
        {
          //new AdSpot
          //{
          //  Tile = TagAs.Tile.Ten,
          //  FirstLevel = TagAs.FirstLevel.Used,
          //  Device = TagAs.Device.Mobile,
          //  SecondLevel = TagAs.SecondLevel.None,
          //  ThirdLevel = TagAs.ThirdLevel.None,
          //  Section = TagAs.Section.None,
          //  Size = TagAs.Size.Mobile.Sz300X50
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.None,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.None,
            Size = TagAs.Size.Mobile.Sz320X50Flex
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.None,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.None,
            Size = TagAs.Size.Mobile.Sz320X51Flex
          }
        };
      }
    }
  }
}
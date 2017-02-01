using Car.Com.Common.Advertisement.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Car.Com.Common.Advertisement.Routes
{
  public class HttpErrorRoutes
  {
    public static ITagGenerator GetAdTagGenerator(string action)
    {
      switch (action)
      {
        case "status404":
        case "status500":
          {
            return new ErrorPage();
          }
      }

      return null;
    }

    public class ErrorPage : ITagGenerator
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
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.Other,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.None,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.None,
              Size = TagAs.Size.Desktop.Sz300X250,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.Other,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.None,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.None,
              Size = TagAs.Size.Desktop.Sz300X120,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.Other,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.None,
            ThirdLevel = TagAs.ThirdLevel.None,
            Section = TagAs.Section.None,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }
  }
}
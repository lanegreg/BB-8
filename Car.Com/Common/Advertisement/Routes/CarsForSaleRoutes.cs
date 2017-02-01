using Car.Com.Common.Advertisement.Common;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Car.Com.Common.Advertisement.Routes
{
  public class CarsForSaleRoutes
  {
    public static ITagGenerator GetAdTagGenerator(string action)
    {
      switch (action)
      {
        case "index":
          {
            return new Index();
          }
        case "make":
          {
            return new Make();
          }
        case "results":
          {
            return new Results();
          }
        case "vehicledetails":
          {
            return new VehicleDetails();
          }
        case "selectmodels":
          {
            return new SelectModels();
          }

        case "selectcategorymakes":
          {
            return new SelectCategoryMakes();
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
              FirstLevel = TagAs.FirstLevel.Used,
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
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.LandingPage,
              ThirdLevel = TagAs.ThirdLevel.None,
              Section = TagAs.Section.Landing,
              Size = TagAs.Size.Desktop.Sz300X250,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.Used,
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
            FirstLevel = TagAs.FirstLevel.Used,
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
            FirstLevel = TagAs.FirstLevel.Used,
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
    
    public class Make : ITagGenerator
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
          Year = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture) 
        };

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Make,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Make,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Make,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Make,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz400X40,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Make,
            Section = TagAs.Section.Listings,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Make,
            Section = TagAs.Section.Listings,
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

        string categoryString = adMeta.category;
        categoryString = categoryString.IsNotNullOrEmpty() ? categoryString.ToLower() : string.Empty;

        //only convertible or hatchback can be styles per Amy 20150513
        //convertible category must be tagged as sedan per Amy 20150519

        var criteria = new Criteria
        {
          Make = adMeta.make ?? String.Empty,
          Model = adMeta.superModel ?? String.Empty,
          Category = categoryString == "convertible" ? "sedan" : categoryString,
          Style = categoryString == "convertible" || categoryString == "hatchback" ? categoryString : string.Empty
        };
        
        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.Listings,
              SubSection = TagAs.SubSection.SearchResults,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.Listings,
              SubSection = TagAs.SubSection.SearchResults,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.Listings,
              SubSection = TagAs.SubSection.SearchResults,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.Listings,
              SubSection = TagAs.SubSection.SearchResults,
              Size = TagAs.Size.Desktop.Sz400X40,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Category,
            Section = TagAs.Section.Listings,
            SubSection = TagAs.SubSection.SearchResults,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Category,
            Section = TagAs.Section.Listings,
            SubSection = TagAs.SubSection.SearchResults,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class VehicleDetails : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        string categoryString = adMeta.category;
        categoryString = categoryString.IsNotNullOrEmpty() ? categoryString.ToLower() : string.Empty;

        //only convertible or hatchback can be styles per Amy 20150513
        //convertible category must be tagged as sedan per Amy 20150519

        var criteria = new Criteria
        {
          Make = adMeta.make,
          Model = adMeta.model,
          Year = adMeta.year,
          Category = categoryString == "convertible" ? "sedan" : categoryString,
          Style = categoryString == "convertible" || categoryString == "hatchback" ? categoryString : string.Empty,
          Fuel = "gas"
        };

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz400X40,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Listings,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Listings,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class SelectModels : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        string categoryString = adMeta.category;
        categoryString = categoryString.IsNotNullOrEmpty() ? categoryString.ToLower() : string.Empty;

        //only convertible or hatchback can be styles per Amy 20150513
        //convertible category must be tagged as sedan per Amy 20150519

        var criteria = new Criteria
        {
          Make = adMeta.make,
          Model = adMeta.model,
          Year = adMeta.year,
          Category = categoryString == "convertible" ? "sedan" : categoryString,
          Style = categoryString == "convertible" || categoryString == "hatchback" ? categoryString : string.Empty,
          Fuel = "gas"
        };

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz400X40,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Listings,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Listings,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class SelectCategoryMakes : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        string categoryString = adMeta.category;
        categoryString = categoryString.IsNotNullOrEmpty() ? categoryString.ToLower() : string.Empty;

        //only convertible or hatchback can be styles per Amy 20150513
        //convertible category must be tagged as sedan per Amy 20150519

        var criteria = new Criteria
        {
          Make = adMeta.make,
          Model = adMeta.model,
          Year = adMeta.year,
          Category = categoryString == "convertible" ? "sedan" : categoryString,
          Style = categoryString == "convertible" || categoryString == "hatchback" ? categoryString : string.Empty,
          Fuel = "gas"
        };

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = TagAs.FirstLevel.Used,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Listings,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Listings,
              Size = TagAs.Size.Desktop.Sz400X40,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Listings,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.Used,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Listings,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Listings,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }
  }
}
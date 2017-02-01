using System;
using Car.Com.Common.Advertisement.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Car.Com.Common.Advertisement.Routes
{
  public class ResearchRoutes
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

        case "supermodel":
          {
            return new SuperModel();
          }

        case "year":
          {
            return new Year();
          }

        case "trim":
        case "trimoverview":
        case "trimspecs":
        case "trimwarranty":
        case "trimcolor":
        case "trimincentives":
          {
            return new Trim();
          }

        case "trimpicsnvids":
          {
            return new TrimPicsAndVids();
          }

        case "trimsafety":
          {
            return new TrimSafety();
          }

        case "alternativefuel":
          {
            return new AlternativeFuel();
          }

        case "category":
          {
            return new Category();
          }

        case "vehicleattribute":
          {
            return new VehicleAttribute();
          }

        case "vehicleattributeresult":
          {
            return new VehicleAttributeResult();
          }

        case "vehicleattributenocategoryresult":
          {
            return new VehicleAttributeNoCategoryResult();
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
              Size = TagAs.Size.Desktop.Sz300X250,
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
          //new AdSpot
          //{
          //  Tile = TagAs.Tile.Ten,
          //  FirstLevel = TagAs.FirstLevel.New,
          //  Device = TagAs.Device.Mobile,
          //  SecondLevel = TagAs.SecondLevel.LandingPage,
          //  ThirdLevel = TagAs.ThirdLevel.None,
          //  Section = TagAs.Section.Landing,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
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
          Fuel = Criteria.FuelType.Gas,
          Make = adMeta.make,
          Year = DateTime.Now.Year.ToString()
        };

        var firstLevel = TagAs.FirstLevel.New;

        if (((IList<string>)TagAs.FirstLevel.Makes.Enthusiasts).Contains(criteria.Make.ToLower()))
        {
          firstLevel = TagAs.FirstLevel.Enthusiast;
        }
        
        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Make,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Make,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Make,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Make,
              Section = TagAs.Section.BuyersGuide,
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
          //  FirstLevel = firstLevel,
          //  Device = TagAs.Device.Mobile,
          //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
          //  ThirdLevel = TagAs.ThirdLevel.Make,
          //  Section = TagAs.Section.BuyersGuide,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = firstLevel,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Make,
            Section = TagAs.Section.BuyersGuide,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = firstLevel,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Make,
            Section = TagAs.Section.BuyersGuide,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class SuperModel : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria = new Criteria
        {
          Fuel = Criteria.FuelType.Gas,
          Make = adMeta.make,
          Model = adMeta.model,
          Year = DateTime.Now.Year.ToString() 
        };

        var firstLevel = TagAs.FirstLevel.New;

        if (((IList<string>)TagAs.FirstLevel.Makes.Enthusiasts).Contains(criteria.Make.ToLower()))
        {
          firstLevel = TagAs.FirstLevel.Enthusiast;
        }

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.BuyersGuide,
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
          //  FirstLevel = firstLevel,
          //  Device = TagAs.Device.Mobile,
          //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
          //  ThirdLevel = TagAs.ThirdLevel.Model,
          //  Section = TagAs.Section.BuyersGuide,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = firstLevel,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.BuyersGuide,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = firstLevel,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.BuyersGuide,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class Year : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria = new Criteria
        {
          Fuel = Criteria.FuelType.Gas,
          Make = adMeta.make,
          Model = adMeta.model,
          Year = adMeta.year
        };

        var firstLevel = TagAs.FirstLevel.New;

        if (((IList<string>)TagAs.FirstLevel.Makes.Enthusiasts).Contains(criteria.Make.ToLower()))
        {
          firstLevel = TagAs.FirstLevel.Enthusiast;
        }

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          if (criteria.Year == DateTime.Now.Year.ToString())
          {
            return new[]
            {
              new AdSpot
              {
                Tile = TagAs.Tile.One,
                FirstLevel = firstLevel,
                Device = TagAs.Device.Desktop,
                SecondLevel = TagAs.SecondLevel.BuyingGuides,
                ThirdLevel = TagAs.ThirdLevel.Model,
                Section = TagAs.Section.BuyersGuide,
                Size = TagAs.Size.Desktop.Sz728X90,
                Criteria = criteria
              },
              new AdSpot
              {
                Tile = TagAs.Tile.Two,
                FirstLevel = firstLevel,
                Device = TagAs.Device.Desktop,
                SecondLevel = TagAs.SecondLevel.BuyingGuides,
                ThirdLevel = TagAs.ThirdLevel.Model,
                Section = TagAs.Section.BuyersGuide,
                Size = TagAs.Size.Desktop.Sz300X250Flex,
                Criteria = criteria
              },
              new AdSpot
              {
                Tile = TagAs.Tile.Three,
                FirstLevel = firstLevel,
                Device = TagAs.Device.Desktop,
                SecondLevel = TagAs.SecondLevel.BuyingGuides,
                ThirdLevel = TagAs.ThirdLevel.Model,
                Section = TagAs.Section.BuyersGuide,
                Size = TagAs.Size.Desktop.Sz300X251,
                Criteria = criteria
              },
              new AdSpot
              {
                Tile = TagAs.Tile.Four,
                FirstLevel = firstLevel,
                Device = TagAs.Device.Desktop,
                SecondLevel = TagAs.SecondLevel.BuyingGuides,
                ThirdLevel = TagAs.ThirdLevel.Model,
                Section = TagAs.Section.BuyersGuide,
                Size = TagAs.Size.Desktop.Sz400X40,
                Criteria = criteria
              }
            };
          }
          else //used vehicle
          {
            firstLevel = TagAs.FirstLevel.Used;
            if (((IList<string>)TagAs.FirstLevel.Makes.Enthusiasts).Contains(criteria.Make.ToLower()))
            {
              firstLevel = TagAs.FirstLevel.Enthusiast;
            }

            return new[]
            {
              new AdSpot
              {
                Tile = TagAs.Tile.One,
                FirstLevel = firstLevel,
                Device = TagAs.Device.Desktop,
                SecondLevel = TagAs.SecondLevel.BuyingGuides,
                ThirdLevel = TagAs.ThirdLevel.Model,
                Section = TagAs.Section.BuyersGuide,
                Size = TagAs.Size.Desktop.Sz728X90,
                Criteria = criteria
              },
              new AdSpot
              {
                Tile = TagAs.Tile.Two,
                FirstLevel = firstLevel,
                Device = TagAs.Device.Desktop,
                SecondLevel = TagAs.SecondLevel.BuyingGuides,
                ThirdLevel = TagAs.ThirdLevel.Model,
                Section = TagAs.Section.BuyersGuide,
                Size = TagAs.Size.Desktop.Sz300X250Flex,
                Criteria = criteria
              },
              new AdSpot
              {
                Tile = TagAs.Tile.Three,
                FirstLevel = firstLevel,
                Device = TagAs.Device.Desktop,
                SecondLevel = TagAs.SecondLevel.BuyingGuides,
                ThirdLevel = TagAs.ThirdLevel.Model,
                Section = TagAs.Section.BuyersGuide,
                Size = TagAs.Size.Desktop.Sz300X251,
                Criteria = criteria
              },
              new AdSpot
              {
                Tile = TagAs.Tile.Four,
                FirstLevel = firstLevel,
                Device = TagAs.Device.Desktop,
                SecondLevel = TagAs.SecondLevel.BuyingGuides,
                ThirdLevel = TagAs.ThirdLevel.Model,
                Section = TagAs.Section.BuyersGuide,
                Size = TagAs.Size.Desktop.Sz400X40,
                Criteria = criteria
              }
            };
          }
        }

        if (criteria.Year == DateTime.Now.Year.ToString())
        {
          firstLevel = TagAs.FirstLevel.New;
          if (((IList<string>)TagAs.FirstLevel.Makes.Enthusiasts).Contains(criteria.Make.ToLower()))
          {
            firstLevel = TagAs.FirstLevel.Enthusiast;
          }

          return new[]
          {
            //new AdSpot
            //{
            //  Tile = TagAs.Tile.Ten,
            //  FirstLevel = firstLevel,
            //  Device = TagAs.Device.Mobile,
            //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
            //  ThirdLevel = TagAs.ThirdLevel.Model,
            //  Section = TagAs.Section.BuyersGuide,
            //  Size = TagAs.Size.Mobile.Sz300X50,
            //  Criteria = criteria
            //},
            new AdSpot
            {
              Tile = TagAs.Tile.Eleven,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Mobile,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Mobile.Sz320X50Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Twelve,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Mobile,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Mobile.Sz320X51Flex,
              Criteria = criteria
            }
          };
        }
        else
        {
          firstLevel = TagAs.FirstLevel.Used;
          if (((IList<string>)TagAs.FirstLevel.Makes.Enthusiasts).Contains(criteria.Make.ToLower()))
          {
            firstLevel = TagAs.FirstLevel.Enthusiast;
          }

          return new[]
          {
            //new AdSpot
            //{
            //  Tile = TagAs.Tile.Ten,
            //  FirstLevel = firstLevel,
            //  Device = TagAs.Device.Mobile,
            //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
            //  ThirdLevel = TagAs.ThirdLevel.Model,
            //  Section = TagAs.Section.BuyersGuide,
            //  Size = TagAs.Size.Mobile.Sz300X50,
            //  Criteria = criteria
            //},
            new AdSpot
            {
              Tile = TagAs.Tile.Eleven,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Mobile,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Mobile.Sz320X50Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Twelve,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Mobile,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Mobile.Sz320X51Flex,
              Criteria = criteria
            }
          };
        }
      }
    }

    public class Trim : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        string bodystyle = adMeta.bodystyle;
        bodystyle = bodystyle.IsNotNullOrEmpty() ? bodystyle.ToLower() : string.Empty;

        string categoryString = adMeta.category;
        categoryString = categoryString.IsNotNullOrEmpty() ? categoryString.ToLower() : string.Empty;

        //only convertible or hatchback can be styles per Amy 20150513
        //convertible category must be tagged as sedan per Amy 20150519

        var criteria = new Criteria
        {
          Fuel = adMeta.fuel,
          Make = adMeta.make,
          Model = adMeta.model,
          Year = adMeta.year,
          Trim = adMeta.trim,
          Category = categoryString == "convertible" ? "sedan" : categoryString, 
          Style = bodystyle == "convertible" || bodystyle == "hatchback" ? bodystyle : string.Empty
        };

        var firstLevel = TagAs.FirstLevel.New;

        if (((IList<string>)TagAs.FirstLevel.Makes.Enthusiasts).Contains(criteria.Make.ToLower()))
        {
          firstLevel = TagAs.FirstLevel.Enthusiast;
        }

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Reviews,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Review,
              SubSection = TagAs.SubSection.VehicleDetails,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Reviews,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Review,
              SubSection = TagAs.SubSection.VehicleDetails,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Reviews,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Review,
              SubSection = TagAs.SubSection.VehicleDetails,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Reviews,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Review,
              SubSection = TagAs.SubSection.VehicleDetails,
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
          //  FirstLevel = firstLevel,
          //  Device = TagAs.Device.Mobile,
          //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
          //  ThirdLevel = TagAs.ThirdLevel.Model,
          //  Section = TagAs.Section.BuyersGuide,
          //  SubSection = TagAs.SubSection.VehicleDetails,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = firstLevel,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Reviews,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Review,
            SubSection = TagAs.SubSection.VehicleDetails,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = firstLevel,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Reviews,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Review,
            SubSection = TagAs.SubSection.VehicleDetails,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class TrimPicsAndVids : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        string bodystyle = adMeta.bodystyle;
        bodystyle = bodystyle.IsNotNullOrEmpty() ? bodystyle.ToLower() : string.Empty;

        string categoryString = adMeta.category;
        categoryString = categoryString.IsNotNullOrEmpty() ? categoryString.ToLower() : string.Empty;

        //only convertible or hatchback can be styles per Amy 20150513
        //convertible category must be tagged as sedan per Amy 20150519

        var criteria = new Criteria
        {
          Fuel = adMeta.fuel,
          Make = adMeta.make,
          Model = adMeta.model,
          Year = adMeta.year,
          Trim = adMeta.trim,
          Category = categoryString == "convertible" ? "sedan" : categoryString,
          Style = bodystyle == "convertible" || bodystyle == "hatchback" ? bodystyle : string.Empty
        };

        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = TagAs.FirstLevel.GalleryNew,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Gallery,
              SubSection = TagAs.SubSection.VehicleDetails,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = TagAs.FirstLevel.GalleryNew,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Gallery,
              SubSection = TagAs.SubSection.VehicleDetails,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = TagAs.FirstLevel.GalleryNew,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Gallery,
              SubSection = TagAs.SubSection.VehicleDetails,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = TagAs.FirstLevel.GalleryNew,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Gallery,
              SubSection = TagAs.SubSection.VehicleDetails,
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
          //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
          //  ThirdLevel = TagAs.ThirdLevel.Model,
          //  Section = TagAs.Section.Gallery,
          //  SubSection = TagAs.SubSection.VehicleDetails,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.GalleryNew,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Gallery,
            SubSection = TagAs.SubSection.VehicleDetails,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = TagAs.FirstLevel.GalleryNew,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Gallery,
            SubSection = TagAs.SubSection.VehicleDetails,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class TrimSafety : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        string bodystyle = adMeta.bodystyle;
        bodystyle = bodystyle.IsNotNullOrEmpty() ? bodystyle.ToLower() : string.Empty;

        string categoryString = adMeta.category;
        categoryString = categoryString.IsNotNullOrEmpty() ? categoryString.ToLower() : string.Empty;

        //only convertible or hatchback can be styles per Amy 20150513
        //convertible category must be tagged as sedan per Amy 20150519

        var criteria = new Criteria
        {
          Fuel = adMeta.fuel,
          Make = adMeta.make,
          Model = adMeta.model,
          Year = adMeta.year,
          Trim = adMeta.trim,
          Category = categoryString == "convertible" ? "sedan" : categoryString,
          Style = bodystyle == "convertible" || bodystyle == "hatchback" ? bodystyle : string.Empty
        };

        var firstLevel = TagAs.FirstLevel.New;
        var subSection = TagAs.SubSection.VehicleDetails;

        if (((IList<string>)TagAs.FirstLevel.Makes.Enthusiasts).Contains(criteria.Make.ToLower()))
        {
          firstLevel = TagAs.FirstLevel.Enthusiast;
          subSection = TagAs.SubSection.Safety;
        }
        
        if (pgCtx.IsDesktop || pgCtx.IsTablet)
        {
          return new[]
          {
            new AdSpot
            {
              Tile = TagAs.Tile.One,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Reviews,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Review,
              SubSection = subSection,
              Size = TagAs.Size.Desktop.Sz728X90,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Two,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Reviews,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Review,
              SubSection = subSection,
              Size = TagAs.Size.Desktop.Sz300X250Flex,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Three,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Reviews,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Review,
              SubSection = subSection,
              Size = TagAs.Size.Desktop.Sz300X251,
              Criteria = criteria
            },
            new AdSpot
            {
              Tile = TagAs.Tile.Four,
              FirstLevel = firstLevel,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.Reviews,
              ThirdLevel = TagAs.ThirdLevel.Model,
              Section = TagAs.Section.Review,
              SubSection = subSection,
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
          //  FirstLevel = firstLevel,
          //  Device = TagAs.Device.Mobile,
          //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
          //  ThirdLevel = TagAs.ThirdLevel.Model,
          //  Section = TagAs.Section.BuyersGuide,
          //  SubSection = subSection,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = firstLevel,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Reviews,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Review,
            SubSection = subSection,
            Size = TagAs.Size.Mobile.Sz320X50Flex,
            Criteria = criteria
          },
          new AdSpot
          {
            Tile = TagAs.Tile.Twelve,
            FirstLevel = firstLevel,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.Reviews,
            ThirdLevel = TagAs.ThirdLevel.Model,
            Section = TagAs.Section.Review,
            SubSection = subSection,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class AlternativeFuel : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria = new Criteria
        {
          Fuel = "hybrid",
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
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
          //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
          //  ThirdLevel = TagAs.ThirdLevel.Category,
          //  Section = TagAs.Section.BuyersGuide,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Category,
            Section = TagAs.Section.BuyersGuide,
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
            Section = TagAs.Section.BuyersGuide,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class Category : ITagGenerator
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
              FirstLevel = TagAs.FirstLevel.New,
              Device = TagAs.Device.Desktop,
              SecondLevel = TagAs.SecondLevel.BuyingGuides,
              ThirdLevel = TagAs.ThirdLevel.Category,
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
          //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
          //  ThirdLevel = TagAs.ThirdLevel.Category,
          //  Section = TagAs.Section.BuyersGuide,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Category,
            Section = TagAs.Section.BuyersGuide,
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
            Section = TagAs.Section.BuyersGuide,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class VehicleAttribute : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria = new Criteria();
        string vehicleAttributeTestValue = adMeta.vehicleattribute;

        vehicleAttributeTestValue = vehicleAttributeTestValue.ToLower().Replace(" ", string.Empty);
        
        switch (vehicleAttributeTestValue)
        {
          case "luxury":
          case "sport":
          case "compact":
          case "supercar":
          {
            //all other vehicleAttributeTestValues are fuel types
            break;
          }
          default:
          {
            criteria.Fuel = vehicleAttributeTestValue;
            break;
          }
        }
       
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
          //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
          //  ThirdLevel = TagAs.ThirdLevel.Category,
          //  Section = TagAs.Section.BuyersGuide,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Category,
            Section = TagAs.Section.BuyersGuide,
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
            Section = TagAs.Section.BuyersGuide,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    public class VehicleAttributeResult : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria = new Criteria();
        string vehicleAttributeTestValue = adMeta.vehicleattributename;

        vehicleAttributeTestValue = vehicleAttributeTestValue.ToLower().Replace(" ", string.Empty);

        switch (vehicleAttributeTestValue)
        {
          case "luxury":
          case "sport":
          case "compact":
          case "supercar":
            {
              //all other vehicleAttributeTestValues are fuel types
              break;
            }
          default:
            {
              criteria.Fuel = vehicleAttributeTestValue;
              break;
            }
        }

        //only convertible or hatchback can be styles per Amy 20150513
        //convertible category must be tagged as sedan per Amy 20150519

        string categoryString = adMeta.category;
        categoryString = categoryString.IsNotNullOrEmpty() ? categoryString.ToLower() : string.Empty;

        criteria.Style = categoryString == "convertible" || categoryString == "hatchback" ? categoryString : string.Empty;
        criteria.Category = categoryString == "convertible" ? "sedan" : categoryString;

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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
              Size = TagAs.Size.Desktop.Sz400X40,
              Criteria = criteria
            }
          };
        }

        return new[]
        {
          new AdSpot
          {
            Tile = TagAs.Tile.One,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Category,
            Section = TagAs.Section.BuyersGuide,
            Size = TagAs.Size.Mobile.Sz300X50,
            Criteria = criteria
          }
        };
      }
    }

    public class VehicleAttributeNoCategoryResult : ITagGenerator
    {
      public IList<AdSpot> ProduceAdTags(string pageCtx)
      {
        // Page Context
        var pgCtx = JsonConvert.DeserializeObject<PageContext>(pageCtx);

        // Page Advertisement Meta
        dynamic adMeta = JObject.Parse(pageCtx);

        var criteria = new Criteria();
        string vehicleAttributeTestValue = adMeta.vehicleattributename;

        vehicleAttributeTestValue = vehicleAttributeTestValue.ToLower().Replace(" ", string.Empty);

        switch (vehicleAttributeTestValue)
        {
          case "luxury":
          case "sport":
          case "compact":
          case "supercar":
            {
              //all other vehicleAttributeTestValues are fuel types
              break;
            }
          default:
            {
              criteria.Fuel = vehicleAttributeTestValue;
              break;
            }
        }

        //only convertible or hatchback can be styles per Amy 20150513
        //convertible category must be tagged as sedan per Amy 20150519

        string categoryString = adMeta.category;
        categoryString = categoryString.IsNotNullOrEmpty() ? categoryString.ToLower() : string.Empty;

        criteria.Style = categoryString == "convertible" || categoryString == "hatchback" ? categoryString : string.Empty;
        criteria.Category = categoryString == "convertible" ? "sedan" : categoryString;
        
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
              Section = TagAs.Section.BuyersGuide,
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
          //  SecondLevel = TagAs.SecondLevel.BuyingGuides,
          //  ThirdLevel = TagAs.ThirdLevel.Category,
          //  Section = TagAs.Section.BuyersGuide,
          //  Size = TagAs.Size.Mobile.Sz300X50,
          //  Criteria = criteria
          //},
          new AdSpot
          {
            Tile = TagAs.Tile.Eleven,
            FirstLevel = TagAs.FirstLevel.New,
            Device = TagAs.Device.Mobile,
            SecondLevel = TagAs.SecondLevel.BuyingGuides,
            ThirdLevel = TagAs.ThirdLevel.Category,
            Section = TagAs.Section.BuyersGuide,
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
            Section = TagAs.Section.BuyersGuide,
            Size = TagAs.Size.Mobile.Sz320X51Flex,
            Criteria = criteria
          }
        };
      }
    }

    
  }
}
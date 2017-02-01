using System;

namespace Car.Com.Common.Advertisement.Common
{
  public class TagAs
  {
    public static class FirstLevel
    {
      public static string Home
      {
        get { return "home"; }
      }

      public static string New
      {
        get { return "new"; }
      }

      public static string Used
      {
        get { return "used"; }
      }

      public static string Finance
      {
        get { return "finance"; }
      }

      public static string GalleryNew
      {
        get { return "gallery.new"; }
      }

      public static string Other
      {
        get { return "other"; }
      }

      public static string Enthusiast
      {
        get { return "enth"; }
      }

      public static class Makes
      {
        private static string[] _enthusiasts =
			  {
          "aston-martin",
          "astonmartin",
				  "bentley",
				  "bugatti",
				  "caterham",
				  "ferrari",
				  "fisker",
				  "lamborghini",
				  "lotus",
				  "maserati",
				  "maybach",
				  "mclaren",
				  "morgan",
				  "pagani",
				  "rinspeed",
				  "rolls-royce",
          "rollsroyce",
				  "tesla"
			  };

        public static string[] Enthusiasts
        {
          get { return _enthusiasts; }
        }
      }
    }

    public static class Device
    {
      public static string Mobile
      {
        get { return "mob"; }
      }

      public static string Desktop
      {
        get { return "dfp"; }
      }
    }

    public static class SecondLevel
    {
      public static string None
      {
        get { return String.Empty; }
      }

      public static string BuyingGuides
      {
        get { return "/bg"; }
      }

      public static string LandingPage
      {
        get { return "/ld"; }
      }

      public static string Listings
      {
        get { return "/ls"; }
      }

      public static string Reviews
      {
        get { return "/rv"; }
      }
    }
    
    public static class ThirdLevel
    {
      public static string None
      {
        get { return String.Empty; }
      }

      public static string Category
      {
        get { return "/ct"; }
      }

      public static string Make
      {
        get { return "/mak"; }
      }

      public static string Model
      {
        get { return "/mod"; }
      }
    }

    public static class Section
    {
      public static string None
      {
        get { return String.Empty; }
      }

      public static string Article
      {
        get { return "article"; }
      }

      public static string BuyersGuide
      {
        get { return "buyersguide"; }
      }

      public static string Comparison
      {
        get { return "comparison"; }
      }

      public static string Gallery
      {
        get { return "gallery"; }
      }

      public static string Home
      {
        get { return "homepage"; }
      }

      public static string Landing
      {
        get { return "landingpage"; }
      }

      public static string Listings
      {
        get { return "listings"; }
      }

      public static string Research
      {
        get { return "rhp"; }
      }

      public static string Review
      {
        get { return "review"; }
      }
    }

    public static class SubSection
    {
      public static string None
      {
        get { return String.Empty; }
      }

      public static string Finance
      {
        get { return "finance"; }
      }

      public static string Safety
      {
        get { return "safety"; }
      }

      public static string VehicleDetails
      {
        //used for new cars per jumpstart
        get { return "vdp"; }
      }

      public static string SearchResults
      {
        //used for used cars per jumpstart
        get { return "srp"; }
      }
    }

    public static class Size
    {
      public static class Desktop
      {
        public static string Sz728X90
        {
          get { return "[728,90]"; }
        }

        public static string Sz728X90Flex
        {
          get { return "[[728,90],[970,90]]"; }
        }

        public static string Sz300X120
        {
          get { return "[300,120]"; }
        }

        public static string Sz300X250
        {
          get { return "[300,250]"; }
        }

        public static string Sz300X251
        {
          get { return "[300,251]"; }
        }

        public static string Sz300X250Flex
        {
          get { return "[[300,250],[300,600]]"; }
        }

        public static string Sz400X40
        {
          get { return "[400,40]"; }
        }
      }

      public static class Mobile
      {
        //the Sz300X50 is used at the bottom of the mobile view for targeted ads (not used yet)
        public static string Sz300X50
        {
          get { return "[300,50]"; }
        }

        public static string Sz300X50Flex
        {
          get { return "[[300,50],[320,50]]"; }
        }

        public static string Sz320X50Flex
        {
          get { return "[[320,50],[300,250]]"; }
        }

        public static string Sz320X51Flex
        {
          get { return "[[320,51],[300,251]]"; }
        }
      }
    }

    public static class Tile
    {
      //===================================
      //desktop and tablet ad tiles are <10
      //===================================

      public static int One
      {
        get { return 1; }
      }

      public static int Two
      {
        get { return 2; }
      }

      public static int Three
      {
        get { return 3; }
      }

      public static int Four
      {
        get { return 4; }
      }

      //========================
      //mobile ad tiles are >=10
      //========================

      //10 is fixed at bottom and used for focused ad targeting by jumpstart
      public static int Ten
      {
        get { return 10; }
      }

      //11 to float at top
      public static int Eleven
      {
        get { return 11; }
      }

      //12 to float at bottom
      public static int Twelve
      {
        get { return 12; }
      }
    }
  }
}
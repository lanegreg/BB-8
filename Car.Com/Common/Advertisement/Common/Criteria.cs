using System;

namespace Car.Com.Common.Advertisement.Common
{
  public class Criteria
  {
    private string _make = String.Empty;
    private string _model = String.Empty;
    private string _year = String.Empty;
    private string _trim = String.Empty;

    public Criteria()
    {
      Category = Style = Fuel = String.Empty;
      _make = _model = _year = String.Empty;
    }


    public string Make
    {
      get { return _make.TagifyMakeModel(); }
      set { _make = value; }
    }

    public string Model
    {
      get { return _model.TagifyMakeModel(); }
      set { _model = value; }
    }

    public string Year
    {
      get { return _year.Tagify(); }
      set { _year = value; }
    }

    public string Trim
    {
      get { return _trim.Tagify(); }
      set { _trim = value; }
    }


    public string Category { get; set; }

    public class CategoryType
    {
      public static string Sedan
      {
        get { return "sedan"; }
      }

      public static string Coupe
      {
        get { return "coupe"; }
      }

      public static string Compact
      {
        get { return "compact"; }
      }
    }


    public string Style { get; set; }

    public class StyleType
    {
      public static string Hatchback
      {
        get { return "hatchback"; }
      }
    }
    

    public string Fuel { get; set; }
    
    public class FuelType
    {
      public static string Gas
      {
        get { return "gas"; }
      }
    }
  }
}
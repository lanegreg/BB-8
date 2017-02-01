using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public class FilterDomain : IRecacheable
  {
  #region Declarations

    private static IEnumerable<IFeature> _fuelTypes = new List<Feature>();

    private static IEnumerable<IFeature> _cylinders = new List<Feature>
    {
      new Feature("3", "3 Cyl"),
      new Feature("4", "4 Cyl"),
      new Feature("5", "5 Cyl"),
      new Feature("6", "6 Cyl"),
      new Feature("8", "8 Cyl"),
      new Feature("10", "10 Cyl"),
      new Feature("12", "12 Cyl")
    };

    private static IEnumerable<IFeature> _mpgs = new List<Feature>
    {
      new Feature("10", "10 MPG & Over"),
      new Feature("15", "15 MPG & Over"),
      new Feature("20", "20 MPG & Over"),
      new Feature("25", "25 MPG & Over"),
      new Feature("30", "30 MPG & Over"),
      new Feature("35", "35 MPG & Over"),
      new Feature("40", "40 MPG & Over")
    };


    private static IEnumerable<IFeature> _driveTypes = new List<Feature>
    {
      new Feature("2WD", "2 wheel drive"),
      new Feature("4WD", "4 wheel drive"),
      new Feature("AWD", "All wheel drive")
    };



    private static IEnumerable<IFeature> _trannyTypes = new List<Feature>
    {
      new Feature("MAN", "Manual"),
      new Feature("AUTO", "Automatic")
    };

    #endregion


    #region Public Methods

    public static IEnumerable<IFeature> Cylinders
    {
      get { return _cylinders; }
      set { _cylinders = value; }
    }

    public static string CylindersStringified
    {
      get { return String.Join("|", _cylinders); }
    }



    public static IEnumerable<IFeature> CityMpgs
    {
      get { return _mpgs; }
      set { _mpgs = value; }
    }

    public static string CityMpgsStringified
    {
      get { return String.Join("|", _mpgs); }
    }



    public static IEnumerable<IFeature> HighwayMpgs
    {
      get { return _mpgs; }
      set { _mpgs = value; }
    }

    public static string HighwayMpgsStringified
    {
      get { return String.Join("|", _mpgs); }
    }

    #endregion



    #region Interface Implementations

    public void Recache()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Common
{
  public class FilterDomains : IFilterDomains
  {
    private static readonly object Mutex = new object();
    private static IDictionary<int, IEnumerable<Feature>> _modelsDictByMakeId;
    private static IDictionary<int, IEnumerable<Feature>> _makesDictByCategoryId;


    public FilterDomains()
    {
      Cylinders = _cylinders;
      CityMpgs = _mpgs;
      HighwayMpgs = _mpgs;
      DriveTypes = _driveTypes;
      TrannyTypes = _trannyTypes;
      PriceRanges = _priceRanges;
      MileageRanges = _mileageRanges;
    }



    #region Statically Derived Feature Domains

    private readonly IEnumerable<IFeature> _cylinders = new List<Feature>
    {
      new Feature("3", "3 Cyl"),
      new Feature("4", "4 Cyl"),
      new Feature("5", "5 Cyl"),
      new Feature("6", "6 Cyl"),
      new Feature("8", "8 Cyl"),
      new Feature("10", "10 Cyl"),
      new Feature("12", "12 Cyl")
    };

    private readonly IEnumerable<IFeature> _mpgs = new List<Feature>
    {
      new Feature("10", "10 MPG & Over"),
      new Feature("15", "15 MPG & Over"),
      new Feature("20", "20 MPG & Over"),
      new Feature("25", "25 MPG & Over"),
      new Feature("30", "30 MPG & Over"),
      new Feature("35", "35 MPG & Over"),
      new Feature("40", "40 MPG & Over")
    };

    private readonly IEnumerable<IFeature> _driveTypes = new List<Feature>
    {
      new Feature("2WD", "2 Wheel Drive"),
      new Feature("4WD", "4 Wheel Drive"),
      new Feature("AWD", "All Wheel Drive")
    };

    private readonly IEnumerable<IFeature> _trannyTypes = new List<Feature>
    {
      new Feature("MAN", "Manual"),
      new Feature("AUTO", "Automatic")
    };

    private readonly IEnumerable<IFeature> _priceRanges = new List<Feature>
    {
      new Feature("0|15", "Up to $15k"),
      new Feature("15|20", "$15k to $20k"),
      new Feature("20|25", "$20k to $25k"),
      new Feature("25|35", "$25k to $35k"),
      new Feature("35|50", "$35k to $50k"),
      new Feature("50|300", "$50k and Up")
    };

    private readonly IEnumerable<IFeature> _mileageRanges = new List<Feature>
    {
      new Feature("0|15", "Up to 15k"),
      new Feature("15|25", "15k to 25k"),
      new Feature("25|35", "25k to 35k"),
      new Feature("35|50", "35k to 50k"),
      new Feature("50|80", "50k to 80k"),
      new Feature("80|300", "80k and Up")
    };

    #endregion

    #region Serializable Feature Domains

    [JsonProperty("cylinders")]
    public IEnumerable<IFeature> Cylinders { get; private set; }

    [JsonProperty("cityMpgs")]
    public IEnumerable<IFeature> CityMpgs { get; private set; }

    [JsonProperty("highwayMpgs")]
    public IEnumerable<IFeature> HighwayMpgs { get; private set; }

    [JsonProperty("driveTypes")]
    public IEnumerable<IFeature> DriveTypes { get; private set; }

    [JsonProperty("trannyTypes")]
    public IEnumerable<IFeature> TrannyTypes { get; private set; }

    [JsonProperty("options")]
    public IEnumerable<IFeature> Options { get; private set; }

    [JsonProperty("fuelTypes")]
    public IEnumerable<IFeature> FuelTypes { get; private set; }

    [JsonProperty("mileageRanges")]
    public IEnumerable<IFeature> MileageRanges { get; private set; }

    [JsonProperty("priceRanges")]
    public IEnumerable<IFeature> PriceRanges { get; private set; }

    [JsonProperty("categories")]
    public IEnumerable<IFeature> Categories { get; private set; }

    [JsonProperty("years")]
    public IEnumerable<IFeature> Years { get; private set; }

    [JsonProperty("makes")]
    public IEnumerable<IFeature> Makes { get; private set; }
    
    #endregion



    #region Public Methods

    public void SetYears(IEnumerable<IFeature> years)
    {
      lock (Mutex)
      {
        Years = years;
      }
    }

    public void SetOptions(IEnumerable<IFeature> options)
    {
      lock (Mutex)
      {
        Options = options.OrderBy(f => f.Description);
      }
    }

    public void SetFuelTypes(IEnumerable<IFeature> fuelTypes)
    {
      lock (Mutex)
      {
        FuelTypes = fuelTypes.OrderBy(f => f.Description);
      }
    }

    public void SetCategories(IEnumerable<IFeature> categories)
    {
      lock (Mutex)
      {
        Categories = categories;
      }
    }

    public void SetMakes(IEnumerable<IFeature> makes)
    {
      lock (Mutex)
      {
        Makes = makes.OrderBy(f => f.Description);
      }
    }

    public void SetModelsDictionary(IDictionary<int, IEnumerable<Feature>> modelsDict)
    {
      lock (Mutex)
      {
        _modelsDictByMakeId = modelsDict;
      }
    }

    public void SetMakesDictionary(IDictionary<int, IEnumerable<Feature>> makesDict)
    {
      lock (Mutex)
      {
        _makesDictByCategoryId = makesDict;
      }
    }



    public IEnumerable<IFeature> GetModelsDomainByMakeId(int makeId)
    {
      return _modelsDictByMakeId.ContainsKey(makeId) ? _modelsDictByMakeId[makeId] : new List<Feature>();
    }

    public IEnumerable<IFeature> GetMakesDomainByCategoryId(int categoryId)
    {
      return _makesDictByCategoryId.ContainsKey(categoryId) ? _makesDictByCategoryId[categoryId] : new List<Feature>();
    }

    #endregion
  }
}

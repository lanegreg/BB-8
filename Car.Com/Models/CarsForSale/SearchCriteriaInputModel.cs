
namespace Car.Com.Models.CarsForSale
{
  public class SearchCriteriaInputModel
  {
    public string Zip { get; set; }
    public int RadiusMiles { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }

    public string SortBy { get; set; }

    public string Makes { get; set; }
    public string Years { get; set; }
    public string Models { get; set; }
    public string OptionIds { get; set; }
    public string PriceRange { get; set; }
    public string MileageRange { get; set; }
    public string CategoryIds { get; set; }
    public string Cylinders { get; set; }
    public string DriveTypes { get; set; }
    public string FuelTypeIds { get; set; }
    public string TrannyTypes { get; set; }
    public string CityMpg { get; set; }
    public string HwyMpg { get; set; }
  }
}
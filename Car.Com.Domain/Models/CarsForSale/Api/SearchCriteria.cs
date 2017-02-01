using Car.Com.Domain.Models.CarsForSale.Common;
using Car.Com.Domain.Models.CarsForSale.Filters;

namespace Car.Com.Domain.Models.CarsForSale.Api
{
  public sealed class SearchCriteria
  {
    public SearchCriteria()
    {
      // Set defaults
      Zipcode = "92612";
      vdpPrice = "";
      vdpMileage = "";
      RadiusMiles = 100;

      Skip = 0;
      Take = 12;

      Sort = new Dto.Sort
      {
        By = Common.Sort.By.BestMatch, 
        Direction = Common.Sort.Direction.Ascending
      };

      MakeModelComboFilter = new MakeModelComboFilter();
      CategoryMakeComboFilter = new CategoryMakeComboFilter();
      NewStatusFilter = new NewStatusFilter();
      CityMpgFilter = new CityMpgFilter();
      HighwayMpgFilter = new HighwayMpgFilter();
      PriceRangeFilter = new PriceRangeFilter();
      MileageRangeFilter = new MileageRangeFilter();
      YearRangeFilter = new YearRangeFilter();
      CylindersFilter = new CylindersFilter();
      DriveTypeFilter = new DriveTypeFilter();
      FuelTypeFilter = new FuelTypeFilter();
      TransmissionTypeFilter = new TransmissionTypeFilter();
      OptionBitsFilter = new OptionBitsFilter();
      TrimIdFilter = new TrimIdFilter();
    }

    public string Zipcode { get; set; }
    public string vdpPrice { get; set; }
    public string vdpMileage { get; set; }
    public int RadiusMiles { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public string MaxPrice { get; set; }
    public string MaxMileage { get; set; }

    public Dto.Sort Sort { get; set; }

    public IComboFilter MakeModelComboFilter { get; set; }
    public IComboFilter CategoryMakeComboFilter { get; set; }
    public IExclusion DealersExclusion { get; set; }
    public IFilter NewStatusFilter { get; set; }
    public IFilter CityMpgFilter { get; set; }
    public IFilter HighwayMpgFilter { get; set; }
    public IFilter PriceRangeFilter { get; set; }
    public IFilter MileageRangeFilter { get; set; }
    public IFilter YearRangeFilter { get; set; }
    public IFilter CylindersFilter { get; set; }
    public IFilter DriveTypeFilter { get; set; }
    public IFilter FuelTypeFilter { get; set; }
    public IFilter TransmissionTypeFilter { get; set; }
    public IFilter OptionBitsFilter { get; set; }
    public IFilter TrimIdFilter { get; set; }



    public static class Dto
    {
      public class Sort
      {
        internal Sort() { }
        public Sort(string by, string direction)
        {
          By = by;
          Direction = direction;
        }

        public string By { get; set; }
        public string Direction { get; set; }
      }
    }
  }
}
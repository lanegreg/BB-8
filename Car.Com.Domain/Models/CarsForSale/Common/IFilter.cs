
namespace Car.Com.Domain.Models.CarsForSale.Common
{
  public interface IFilter
  {
    bool MatchesThis(CarForSale car);
  }
}
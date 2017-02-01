
namespace Car.Com.Domain.Models.CarsForSale.Common
{
  public interface IExclusion
  {
    bool MatchesThis(CarForSale car);
  }
}

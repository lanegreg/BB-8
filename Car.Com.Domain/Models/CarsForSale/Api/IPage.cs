
namespace Car.Com.Domain.Models.CarsForSale.Api
{
  public interface IPage
  {
    int Current { get; }
    int ItemsPerPage { get; }
    int TotalPages { get; }
  }
}
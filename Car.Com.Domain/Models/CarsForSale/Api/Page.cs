
namespace Car.Com.Domain.Models.CarsForSale.Api
{
  public sealed class Page : IPage
  {
    public Page(int current, int itemsPerPage, int totalPages)
    {
      Current = current;
      ItemsPerPage = itemsPerPage;
      TotalPages = totalPages;
    }


    public int Current { get; private set; }
    public int ItemsPerPage { get; private set; }
    public int TotalPages { get; private set; }
  }
}

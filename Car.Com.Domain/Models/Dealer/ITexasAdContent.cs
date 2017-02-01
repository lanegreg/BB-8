
namespace Car.Com.Domain.Models.Dealer
{
  public interface ITexasAdContent
  {
    int DealerId { get; }
    string ImageUrl { get; }
    string SupplierAdDescription { get; }
    int SupplierAdTypeId { get; }
    TexasAdContent.Dto.Image Image { get; }
    TexasAdContent.Dto.Frame Frame { get; }
  }
}
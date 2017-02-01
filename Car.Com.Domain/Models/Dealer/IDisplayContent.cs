
namespace Car.Com.Domain.Models.Dealer
{
  public interface IDisplayContent
  {
    int DealerId { get; }
    string DealerMessage { get; }
    string ThankYouMessage { get; }
    DisplayContent.Dto.Logo Logo { get; }
  }
}
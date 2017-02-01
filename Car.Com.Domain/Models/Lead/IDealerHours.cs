
namespace Car.Com.Domain.Models.Lead
{
  public interface IDealerHours
  {
    string SalesMonOpen { get; }
    string SalesMonClose { get; }
    string SalesTueOpen { get; }
    string SalesTueClose { get; }
    string SalesWedOpen { get; }
    string SalesWedClose { get; }
    string SalesThrOpen { get; }
    string SalesThrClose { get; }
    string SalesFriOpen { get; }
    string SalesFriClose { get; }
    string SalesSatOpen { get; }
    string SalesSatClose { get; }
    string SalesSunOpen { get; }
    string SalesSunClose { get; }
  }
}

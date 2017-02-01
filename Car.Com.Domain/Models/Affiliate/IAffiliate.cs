
namespace Car.Com.Domain.Models.Affiliate
{
  public interface IAffiliate
  {
    int Id { get; }
    string Name { get; }
    string GroupName { get; }
    string TrafficSource { get; }
  }
}
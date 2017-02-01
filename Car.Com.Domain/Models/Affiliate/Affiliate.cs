using Car.Com.Domain.Common;

namespace Car.Com.Domain.Models.Affiliate
{
  public class Affiliate : Entity, IAffiliate
  {
    public string Name { get; set; }
    public string GroupName { get; set; }
    public string TrafficSource { get; set; }
  }
}

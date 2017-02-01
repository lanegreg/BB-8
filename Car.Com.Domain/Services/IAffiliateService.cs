using Car.Com.Domain.Models.Affiliate;

namespace Car.Com.Domain.Services
{
  public interface IAffiliateService
  {
    int CarDotComId { get; }
    IAffiliate GetAffiliateById(int id);
  }
}

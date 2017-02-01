
namespace Car.Com.Domain.Models.Sitemap
{
  public interface IPage
  {
    string Url { get; }
    string LastModified { get; }
    string Priority { get; }
    string ChangeFrequency { get; }
  }
}
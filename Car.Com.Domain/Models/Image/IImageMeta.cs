
namespace Car.Com.Domain.Models.Image
{
  public interface IImageMeta
  {
    int Id { get; }
    int Year { get; }
    int TrimId { get; }
    string Make { get; }
    string Model { get; }
    string Acode { get; }
    string UrlPrefix { get; }
    string Source { get; }
    string View { get; }
    string CategoryView { get; }
  }
}

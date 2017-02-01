
namespace Car.Com.Domain.Models.Translators
{
  public interface IMakeTranslator
  {
    int Id { get; }
    string Name { get; }
    string SeoName { get; }
    string PluralName { get; }
    int AbtMakeId { get; }
    bool IsActive { get; }
  }
}

namespace Car.Com.Domain.Models.Translators
{
  public interface ICategoryTranslator
  {
    int Id { get; }
    string Name { get; }
    string SeoName { get; }
    string PluralName { get; }
    string Code { get; }
    int Ordinal { get; }
  }
}
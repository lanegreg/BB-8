
namespace Car.Com.Domain.Models.Translators
{
  public interface IModelTranslator
  {
    string Name { get; }
    string SeoName { get; }
    string VariantName { get; }
  }
}
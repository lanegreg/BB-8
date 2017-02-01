
namespace Car.Com.Domain.Models.Translators
{
  public class CategoryTranslator : ICategoryTranslator
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string SeoName { get; set; }
    public string PluralName { get; set; }
    public string Code { get; set; }
    public int Ordinal { get; set; }
  }
}
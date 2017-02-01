
namespace Car.Com.Domain.Models.Translators
{
  public class MakeTranslator : IMakeTranslator
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string SeoName { get; set; }
    public string PluralName { get; set; }
    public int AbtMakeId { get; set; }
    public bool IsActive { get; set; }
  }
}
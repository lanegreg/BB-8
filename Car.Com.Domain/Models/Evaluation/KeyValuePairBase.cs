
namespace Car.Com.Domain.Models.Evaluation
{
  public abstract class KeyValuePairBase<T>
  {
    public T Key { get; set; }
    public string Value { get; set; }
  }
}

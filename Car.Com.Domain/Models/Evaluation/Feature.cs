
namespace Car.Com.Domain.Models.Evaluation
{
  public class Feature : KeyValuePairBase<string>
  {
    public bool PreSelect { get; set; }
    public int DisplayOrder { get; set; }
    public FeatureType Type { get; set; }
  }
}

using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;

namespace Car.Com.Models.CarValue
{
  public class IndexViewModel : ViewModelBase
  {
    public IndexViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {}

    public IndexViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {}

    public IEnumerable<Dto.EvaluationType> EvaluationTypes { get; set; }
    public IEnumerable<Dto.Year> Years { get; set; }


    public static class Dto
    {
      public abstract class KeyValuePairBase<T>
      {
        public T Key { get; set; }
        public string Value { get; set; }
      }

      public class EvaluationType : KeyValuePairBase<string>
      {

      }

      public class Year : KeyValuePairBase<int>
      {
        
      }
    }
  }
}
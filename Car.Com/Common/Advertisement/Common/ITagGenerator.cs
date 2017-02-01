using System.Collections.Generic;

namespace Car.Com.Common.Advertisement.Common
{
  public interface ITagGenerator
  {
    IList<AdSpot> ProduceAdTags(string pageCtx);
  }
}

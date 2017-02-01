using System;
using System.Collections;
using System.Collections.Generic;

namespace Car.Com.Common.Merger
{
  public class ObjectMergerPolicy
  {
    public IList Ignored { get; private set; }

    public ObjectMergerPolicy(IList ignored)
    {
      Ignored = ignored;
    }

    public ObjectMergerPolicy(object ignoreValue)
    {
      Ignored = new List<object> {ignoreValue};
    }

    public ObjectMergerPolicy Ignore(object value)
    {
      Ignored.Add(value);

      return this;
    }

    public Object MergeTypes(Object obj1, Object obj2)
    {
      return ObjectMerger.MergeObjects(obj1, obj2, this);
    }
  }
}

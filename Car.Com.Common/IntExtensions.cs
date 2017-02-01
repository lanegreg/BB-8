using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Car.Com.Common
{
  public static class IntExtensions
  {
    public static string Join(this IEnumerable<int> integers, string delimiter)
    {
      return String.Join(delimiter, integers.Select(i => i.ToString(CultureInfo.InvariantCulture)).ToArray());
    }
  }
}

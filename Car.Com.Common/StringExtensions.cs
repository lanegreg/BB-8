using System;
using System.Linq;

namespace Car.Com.Common
{
  public static class StringExtensions
  {
    public static bool IsNullOrWhiteSpace(this string s)
    {
      return String.IsNullOrWhiteSpace(s);
    }

    public static bool IsNullOrEmpty(this string s)
    {
      return String.IsNullOrEmpty(s);
    }

    public static bool IsNotNullOrEmpty(this string s)
    {
      return !String.IsNullOrEmpty(s);
    }

    public static string NotAvailify(this string s)
    {
      return s.IsNullOrEmpty() ? "N/A" : s;
    }

    public static string ToValueOrEmpty(this string s)
    {
      return s.IsNullOrEmpty() ? String.Empty : s;
    }

    public static string EnsureTrailingForwardSlash(this string url)
    {
      var hasQueryStringOrFragment = false;
      string[] parts = { url };

      if (url.Contains("?"))
      {
        hasQueryStringOrFragment = true;
        parts = url.Split('?');
      }

      if (url.Contains("#"))
      {
        hasQueryStringOrFragment = true;
        parts = url.Split('#');
      }

      var path = (parts.Count() == 1 ? url : parts[0]).TrimEnd('/') + "/";


      return hasQueryStringOrFragment
        ? String.Concat(path, (url.Contains("?") ? "?" : "#"), parts[1])
        : path;
    }
  }
}

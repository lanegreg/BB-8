using System;
using System.Text.RegularExpressions;

namespace Car.Com.Common.Advertisement.Common
{
  public static class StringExtensions
  {
    public static string Tagify(this string s)
    {
      if (s != null)
      {
        return s.Replace(" ", String.Empty).ToLower();
      }
      else
      {
        return String.Empty;
      }
    }

    //Amy and Jumpstart want only letters and numbers in the ad tag for make and model
    public static string TagifyMakeModel(this string s)
    {
      if (s != null)
      {
        string cleanedMakeModel = String.Empty;

        Regex rgx = new Regex("[^a-zA-Z0-9]+");
        cleanedMakeModel = rgx.Replace(s, "");

        return cleanedMakeModel;

      }
      else
      {
        return String.Empty;
      }
    }
  }
}
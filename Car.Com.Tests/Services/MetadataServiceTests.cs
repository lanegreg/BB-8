using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Car.Com.Tests.Services
{
  [TestClass]
  public class MetadataServiceTests
  {
    [TestMethod]
    public void TestMethod()
    {
      const string uriPathPattern = "/{make}/{model}/dang/{year}/";
      //var match = Regex.Match(uriPathPattern, @"\{[a-z\-]\}");
      var match = Regex.Matches(uriPathPattern, @"\{[a-z\-]+\}?");

      var count = match.Count;
    }
  }
}

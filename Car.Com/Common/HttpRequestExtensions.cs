using System.Web;

namespace Car.Com.Common
{
  public static class HttpRequestExtensions
  {
    public static bool ConnectionIsSecureOrTerminatedSecure(this HttpRequestBase request)
    {
      if (request.Headers["X-Forwarded-Proto"] == "https")
        request.ServerVariables["HTTPS"] = "on";
      
      return request.IsSecureConnection;
    }
  }
}
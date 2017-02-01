using Car.Com.Common.DeviceDetection;
using Car.Com.Common.Environment;
using System;
using System.Web;


namespace Car.Com.Common
{
  public static class HttpContextExtensions
  {
    public static IBrowserCaps GetBrowserCaps(this HttpContextBase httpContext)
    {
      return DeviceDetector.GetBrowserCaps(httpContext);
    }

    public static void SetActionCount(this HttpContextBase httpContext)
    {
      const string keyName = "_action_count_";

      if (httpContext.Items.Contains(keyName))
      {
        var actionCount = httpContext.Items[keyName] as string;
        var count = actionCount == null ? 1 : Int32.Parse(actionCount);
        count++;

        if (count > 1)
          throw new Exception("Request thread has already processed a controller-action.");        
      }
      
      httpContext.Items[keyName] = 1;
    }


    #region Environment Booleans

    public static bool IsProduction(this HttpContextBase httpContext)
    {
      return AppEnvironment.IsProduction;
    }

    public static bool IsLocalDev(this HttpContextBase httpContext)
    {
      return AppEnvironment.IsLocalDev;
    }
    
    public static bool IsQaStage(this HttpContextBase httpContext)
    {
      return AppEnvironment.IsQaStage;
    }

    #endregion
  }
}
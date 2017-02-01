using Car.Com.Common.Api;
using Car.Com.Common.Environment;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
    public class DevOpsController : BaseApiController
    {
      private static bool _inMaintenanceMode;

      [Route("cmd", Name = "CommandAction"), HttpPut]
      public DataWrapper CommandAction(string command)
      {
        try
        {
          switch (command.ToLower())
          {
            case "up":
              _inMaintenanceMode = false;
              break;

            case "down":
              _inMaintenanceMode = true;
              break;

            default:
              throw new Exception(String.Format("Command '{0}' not understood.", command));
          }

          return DataWrapper(new { status = "success" });
        }
        catch (Exception ex)
        {
          return DataWrapper(new
          {
            status = "failure",
            exception = ex.Message
          });
        }
      }


      [Route("health", Name = "Health"), HttpGet]
      public DataWrapper Health()
      {
        var version = "0.0.0";
        var pid = Process.GetCurrentProcess().Id;
        var ts = TimeSpan.FromTicks(DateTime.Now.Ticks - Process.GetProcessById(pid).StartTime.Ticks);
        var uptime = String.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);

        // ReSharper disable once EmptyGeneralCatchClause
        try { version = AppEnvironment.BuildVersion; }
        catch { }

        try
        {
          if (_inMaintenanceMode)
          {
            HttpContext.Current.Response.StatusCode = 503;
            HttpContext.Current.Response.TrySkipIisCustomErrors = true;
          }

          var jsonString = new
          {
            server = HttpContext.Current.Server.MachineName,
            platform = "asp.net",
            app = new
            {
              name = AppEnvironment.ServerName,
              version = String.Format("v{0}", version),
              status = _inMaintenanceMode ? "DOWN" : "UP"
            },
            pid,
            memory = new
            {
              rss = 25231360,
              heapTotal = 17603072,
              heapUsed = 5337224
            },
            uptime
          };

          return DataWrapper(jsonString);
        }
        catch (Exception ex)
        {
          HttpContext.Current.Response.StatusCode = 503;
          HttpContext.Current.Response.TrySkipIisCustomErrors = true;

          var jsonString = String
            .Format("{{\"status\": \"failure\", \"server\": \"{0}\", \"exception\":\"{1}\"}}",
              HttpContext.Current.Server.MachineName,
              ex.Message);

          return DataWrapper(jsonString);
        }
      }
    }
}

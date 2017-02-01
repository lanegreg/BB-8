using Car.Com.Common.Api;
using System;
using System.Diagnostics;
using System.Web.Http;

namespace Car.Com.Controllers.Api
{
  public class BaseApiController : ApiController
  {
    private readonly string _server;
    private readonly string _queryTime;
    private readonly Stopwatch _stopWatch;

    public BaseApiController()
    {
      _stopWatch = new Stopwatch();
      _stopWatch.Start();
      _server = Environment.MachineName;
      _queryTime = DateTime.Now.ToShortDateString() + " @ " + DateTime.Now.ToShortTimeString();
    }

    public DataWrapper DataWrapper()
    {
      return DataWrapper(null, 0, Status.Failure);
    }

    public DataWrapper DataWrapper(string status)
    {
      return DataWrapper(null, 0, status);
    }

    public DataWrapper DataWrapper(object data)
    {
      return DataWrapper(data, 1, Status.Success);
    }

    public DataWrapper DataWrapper(object data, string status)
    {
      return DataWrapper(data, 1, status);
    }

    public DataWrapper DataWrapper(object data, int recordCount)
    {
      return DataWrapper(data, recordCount, Status.Success);
    }

    public DataWrapper DataWrapper(object data, int recordCount, string status)
    {
      _stopWatch.Stop();

      return new DataWrapper
      {
        TimeToProcess = _stopWatch.Elapsed.ToString(),
        Status = status,
        Server = _server,
        QueryTime = _queryTime,
        QueryUrl = Request.RequestUri.OriginalString.Replace(" ", "%20"),
        RecordCount = recordCount,
        Data = data
      };
    }
  }
}
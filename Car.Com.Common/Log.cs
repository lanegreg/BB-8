using log4net;
using System;

namespace Car.Com.Common
{
  public static class Log
  {
    // TODO *** NEED TO REPLACE LOG4NET WITH NLOG WHEN V3.0 COMES OUT ***
    private static readonly ILog LogManager;
    private static readonly bool LoggerIsEnabled = WebConfig.Get<bool>("Logger:Enabled");

    static Log()
    {
      LogManager = log4net.LogManager.GetLogger("Car.Com");
    }

    public static void Error(object message)
    {
      if (LoggerIsEnabled && LogManager.IsErrorEnabled)
        LogManager.Error(message);
    }
    public static void Error(object message, Exception exception)
    {
      if (LoggerIsEnabled && LogManager.IsErrorEnabled)
        LogManager.Error(message, exception);
    }

    public static void Fatal(object message)
    {
      if (LoggerIsEnabled && LogManager.IsFatalEnabled)
        LogManager.Fatal(message);
    }
    public static void Fatal(object message, Exception exception)
    {
      if (LoggerIsEnabled && LogManager.IsFatalEnabled)
        LogManager.Fatal(message, exception);
    }

    public static void Debug(object message)
    {
      if (LoggerIsEnabled && LogManager.IsDebugEnabled)
        LogManager.Debug(message);
    }
    public static void Debug(object message, Exception exception)
    {
      if (LoggerIsEnabled && LogManager.IsDebugEnabled)
        LogManager.Debug(message, exception);
    }

    public static void Warn(object message)
    {
      if (LoggerIsEnabled && LogManager.IsWarnEnabled)
        LogManager.Warn(message);
    }
    public static void Warn(object message, Exception exception)
    {
      if (LoggerIsEnabled && LogManager.IsWarnEnabled)
        LogManager.Warn(message, exception);
    }

    public static void Info(object message)
    {
      if (LoggerIsEnabled && LogManager.IsInfoEnabled)
        LogManager.Info(message);
    }
    public static void Info(object message, Exception exception)
    {
      if (LoggerIsEnabled && LogManager.IsInfoEnabled)
        LogManager.Info(message, exception);
    }
  }
}

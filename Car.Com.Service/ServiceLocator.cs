using System;
using System.Collections.Generic;
using Ninject;

namespace Car.Com.Service
{
  public static class ServiceLocator
  {
    private static IKernel _kernel;

    public static void Init(IKernel kernel)
    {
        _kernel = kernel;
    }

    public static T Get<T>()
    {
      return _kernel.Get<T>();
    }

    public static IEnumerable<T> GetAll<T>()
    {
      return _kernel.GetAll<T>();
    }

    public static object GetService(Type type)
    {
      return _kernel.GetService(type);
    }
  }
}

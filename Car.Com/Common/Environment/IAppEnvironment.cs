
namespace Car.Com.Common.Environment
{
  public interface IAppEnvironment
  {
    bool IsProduction { get; }
    bool IsTest { get; }
    bool IsLocalDev { get; }
  }
}

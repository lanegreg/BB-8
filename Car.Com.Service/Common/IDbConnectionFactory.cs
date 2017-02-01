using System.Data;

namespace Car.Com.Service.Common
{
  public interface IDbConnectionFactory
  {
    IDbConnection CreateConnection(string connString);
  }
}

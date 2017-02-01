
using System;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public interface ITrimIncentive
  {
    DateTime Expires { get; }
    string GroupDesc { get; }
    string CatDesc { get; }
    string MasterDesc { get; }
    decimal Amount { get; }
    double APR_24 { get; }
    double APR_36 { get; }
    double APR_48 { get; }
    double APR_60 { get; }
    double APR_72 { get; }
    double APR_84 { get; }
  }
}

using Car.Com.Domain.Models.CarsForSale.Common;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class NewStatusFilter : IFilter
  {
    private readonly int? _newStatus;

    internal NewStatusFilter() { }

    public NewStatusFilter(int? newStatus)
    {
      if (newStatus.HasValue && new[] {0, 1}.Contains(newStatus.Value))
        _newStatus = newStatus;
    }

    public bool MatchesThis(CarForSale car)
    {
      return !_newStatus.HasValue || _newStatus.Value == car.IsNewStatus;
    }

    public static class Status
    {
      public static int New { get { return 1; } }
      public static int Used { get { return 0; } }
      public static int Both { get { return -1; } } 
    }
  }
}

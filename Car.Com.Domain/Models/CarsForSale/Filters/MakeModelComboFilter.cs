using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class MakeModelComboFilter : IComboFilter
  {
    private readonly IList<ComboTag> _makeModelCombos = new List<ComboTag>();
    

    internal MakeModelComboFilter() { }

    public MakeModelComboFilter(string makes, string makeModels)
    {
      // this is just some sort of simple defense against the possibility of a malformed string
      if (makeModels.Length > 2 && makeModels.Contains("~"))
      {
        _makeModelCombos = makeModels
          .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
          .Select(makeModel => new ComboTag(makeModel))
          .ToList();
      }

      var makeIds = (makes ?? String.Empty)
        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
        .Select(Int32.Parse)
        .ToList();

      makeIds.ToList().ForEach(id => _makeModelCombos.Add(new ComboTag(id)));
    }


    public bool NotDefined { get { return !_makeModelCombos.Any(); } }

    public bool MatchesThis(CarForSale car)
    {
      return _makeModelCombos.Any(combo =>
        combo.IgnoreThisComboMatch ||
        (car.MakeId == combo.Id && combo.Name == String.Empty) ||
        (car.MakeId == combo.Id && combo.Name.Equals(car.Model, StringComparison.InvariantCultureIgnoreCase)));
    }
  }
}

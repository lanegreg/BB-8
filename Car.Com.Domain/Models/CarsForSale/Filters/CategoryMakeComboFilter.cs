using Car.Com.Domain.Models.CarsForSale.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Domain.Models.CarsForSale.Filters
{
  public sealed class CategoryMakeComboFilter : IComboFilter
  {
    private readonly IList<ComboTag> _categoryMakeCombos = new List<ComboTag>();
    

    internal CategoryMakeComboFilter() { }

    public CategoryMakeComboFilter(string categories, string categoryMakes)
    {
      // This check is just some sort of simple defense against the possibility of a malformed string.
      if (categoryMakes.Length > 2 && categoryMakes.Contains("~"))
      {
        _categoryMakeCombos = categoryMakes
          .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
          .Select(categoryMake => new ComboTag(categoryMake))
          .ToList();
      }

      var categoryIds = (categories ?? String.Empty)
        .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(Int32.Parse)
        .ToList();

      categoryIds.ToList().ForEach(id => _categoryMakeCombos.Add(new ComboTag(id)));
    }


    public bool NotDefined { get { return !_categoryMakeCombos.Any(); } }
    
    public bool MatchesThis(CarForSale car)
    {
      return _categoryMakeCombos.Any(combo =>
        combo.IgnoreThisComboMatch ||
        (car.CategoryId == combo.Id && combo.Name == String.Empty) ||
        (car.CategoryId == combo.Id && combo.Name.Equals(car.Make, StringComparison.InvariantCultureIgnoreCase)));
    }
  }
}
using System;

namespace Car.Com.Domain.Models.CarsForSale.Common
{
  public class ComboTag
  {
    public readonly bool IgnoreThisComboMatch;
    public readonly int Id;
    public readonly string Name;

    public ComboTag(int id)
    {
      Id = id;
      Name = String.Empty;
    }

    public ComboTag(string makeModel)
    {
      IgnoreThisComboMatch = true;

      if (!makeModel.Contains("~"))
        return;

      var parts = makeModel.Split(new[] {'~'}, StringSplitOptions.RemoveEmptyEntries);
      Id = Int32.Parse(parts[0]);
      Name = parts[1];
      IgnoreThisComboMatch = false;
    }
  }
}
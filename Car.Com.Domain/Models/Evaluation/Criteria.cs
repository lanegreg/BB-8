using System;

namespace Car.Com.Domain.Models.Evaluation
{
  public class Criteria : ICriteria
  {
    #region ctors
    
    public Criteria() { }

    public Criteria(string mileage, string zipcode)
    {
      SetMileage(mileage);
      SetZipcode(zipcode);
    }

    #endregion



    public EvaluationType EvaluationType { get; set; }
    public Make Make { get; set; }
    public Trim Trim { get; set; }
    public Year Year { get; set; }
    public int Mileage { get; private set; }
    public int Zipcode { get; private set; }

    public void SetMileage(string value)
    {
      int mileage;
      if (Int32.TryParse(value, out mileage))
        Mileage = mileage;
    }

    public void SetZipcode(string value)
    {
      int zipcode;
      if (Int32.TryParse(value, out zipcode))
        Zipcode = zipcode;
    }
  }

  public interface ICriteria
  {
    EvaluationType EvaluationType { get; }
    Make Make { get; }
    Trim Trim { get; }
    Year Year { get; }
    int Mileage { get; }
    int Zipcode { get; }
  }
}

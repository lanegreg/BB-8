
namespace Car.Com.Domain.Models.CarsForSale.Common
{
  public sealed class Sort
  {
    public class By
    {
      public static string BestMatch = "best_match";
      public static string Distance = "distance";
      public static string Price = "price";
      public static string Mileage = "mileage";
      public static string SuggestedRanking = "suggested_ranking";
    }

    public class Direction
    {
      public static string Ascending = "asc";
      public static string Descending = "desc";
    }
  }
}
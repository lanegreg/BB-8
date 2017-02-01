using Car.Com.Common.Advertisement.Common;
using Car.Com.Common.Advertisement.Routes;

namespace Car.Com.Common.Advertisement
{
  public class Router
  {
    public static ITagGenerator GetGeneratorForRoute(string routeName)
    {
      var parts = routeName.Split('.');
      var controller = parts[0];
      var action = parts[1];

      switch (controller)
      {
        case "article":
          {
            return ArticleRoutes.GetAdTagGenerator(action);
          }

        case "buyingguides":
          {
              return BuyingGuidesRoutes.GetAdTagGenerator(action);
          }

        case "finance":
          {
              return FinanceRoutes.GetAdTagGenerator(action);
          }

        case "calculator":
          {
            return CalculatorRoutes.GetAdTagGenerator(action);
          }

        case "carsforsale":
          {
            return CarsForSaleRoutes.GetAdTagGenerator(action);
          }

        case "carvalue":
          {
            return CarValueRoutes.GetAdTagGenerator(action);
          }

        case "research":
          {
            return ResearchRoutes.GetAdTagGenerator(action);
          }

        case "comparecars":
          {
            return CompareCarsRoutes.GetAdTagGenerator(action);
          }

        case "site":
          {
            return SiteRoutes.GetAdTagGenerator(action);
          }

        case "sponsored":
        {
          return SponsoredRoutes.GetAdTagGenerator(action);
        }

        case "httperror":
          {
            return HttpErrorRoutes.GetAdTagGenerator(action);
          }
      }

      return null;
    }
  }
}
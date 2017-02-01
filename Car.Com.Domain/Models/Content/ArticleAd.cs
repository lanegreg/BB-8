
namespace Car.Com.Domain.Models.Content
{
  public class ArticleAd : IArticleAd
  {
    public int AdUnitId { get; set; }
    // public int PageOrder { get; set; }

    /* Ad Keyword for Article / Article Page */
    // public int AdKeywordId { get; set; }

    /* Ad Vehicle for Article / Article Page */
    // public int AdMakeId { get; set; }
    public string AdMake { get; set; }

    // public int AdModelId { get; set; }
    public string AdModel { get; set; }

    public int AdVehicleYear { get; set; }

    // public int AdSeriesId { get; set; }
    public string AdSeries { get; set; }

    /* Ad Category for Article / Article Page */
    // public int AdCategoryId { get; set; }
    public string AdCategory { get; set; }

  }
}

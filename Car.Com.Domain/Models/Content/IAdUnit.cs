namespace Car.Com.Domain.Models.Content
{
  public interface IAdUnit
  {
    int ContentId { get; set; }
    int ContentDetailId { get; set; }

    int AdUnitId { get; set; }
    int PageOrder { get; set; }

    /* Article Vehicle - default if there are no ads defined */
    int MakeId { get; set; }
    string Make { get; set; }

    int ModelId { get; set; }
    string Model { get; set; }

    int VehicleYear { get; set; }

    int SeriesId { get; set; }
    string Series { get; set; }

    /* Article Category - default if there are no ads defined */
    int CategoryId { get; set; }
    string Category { get; set; }

    /* Ad Keyword for Article / Article Page */
    int AdKeywordId { get; set; }

    int KeywordAdUnitId { get; set; }

    /* Ad Vehicle for Article / Article Page */
    int AdMakeId { get; set; }
    string AdMake { get; set; }

    int AdModelId { get; set; }
    string AdModel { get; set; }

    int AdVehicleYear { get; set; }

    int AdSeriesId { get; set; }
    string AdSeries { get; set; }

    int VehicleAdUnitId { get; set; }

    /* Ad Category for Article / Article Page */
    int AdCategoryId { get; set; }
    string AdCategory { get; set; }
    int CategoryAdUnitId { get; set; }
  }
}


namespace Car.Com.Domain.Models.Content

{
  public class AdUnit : IAdUnit
  {
    public int ContentId { get; set; }
    public int ContentDetailId { get; set; }

    public int AdUnitId { get; set; }
    public int PageOrder { get; set; }

    /* Article Vehicle - default if there are no ads defined */
    public int MakeId { get; set; }
    public string Make { get; set; }

    public int ModelId { get; set; }
    public string Model { get; set; }

    public int VehicleYear { get; set; }

    public int SeriesId { get; set; }
    public string Series { get; set; }

    /* Article Category - default if there are no ads defined */
    public int CategoryId { get; set; }
    public string Category { get; set; }

    /* Ad Keyword for Article / Article Page */
    public int AdKeywordId { get; set; }

    public int KeywordAdUnitId { get; set; }

    /* Ad Vehicle for Article / Article Page */
    public int AdMakeId { get; set; }
    public string AdMake { get; set; }

    public int AdModelId { get; set; }
    public string AdModel { get; set; }

    public int AdVehicleYear { get; set; }

    public int AdSeriesId { get; set; }
    public string AdSeries { get; set; }

    public int VehicleAdUnitId { get; set; }

    /* Ad Category for Article / Article Page */
    public int AdCategoryId { get; set; }
    public string AdCategory { get; set; }
    public int CategoryAdUnitId { get; set; }
  }
}

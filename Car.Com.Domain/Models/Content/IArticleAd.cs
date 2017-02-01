using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.Content
{
  public interface IArticleAd
  {
    int AdUnitId { get; set; }
    // public int PageOrder { get; set; }

    /* Ad Keyword for Article / Article Page */
    // public int AdKeywordId { get; set; }

    /* Ad Vehicle for Article / Article Page */
    // public int AdMakeId { get; set; }
    string AdMake { get; set; }

    // public int AdModelId { get; set; }
    string AdModel { get; set; }

    int AdVehicleYear { get; set; }

    // public int AdSeriesId { get; set; }
    string AdSeries { get; set; }

    /* Ad Category for Article / Article Page */
    // public int AdCategoryId { get; set; }
    string AdCategory { get; set; }

  }
}

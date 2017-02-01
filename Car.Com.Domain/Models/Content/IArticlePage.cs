using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public interface IArticlePage
  {
    int ContentId { get; set; }
    int ContentDetailId { get; set; }
    int PageOrder { get; set; }
    string PageTitle { get; set; }

    string PageImage { get; set; }
    string PageImageThumbnail { get; set; }
    string PageImageThumbnailSmall { get; set; }
    string VideoUrl { get; set; }
    string VideoImageUrl { get; set; }

    string PageText { get; set; }

    int MakeId { get; set; }
    string Make { get; set; }

    int ModelId { get; set; }
    string Model { get; set; }

    int Year { get; set; }

    int TrimId { get; set; }
    string Trim { get; set; }

    int CategoryId { get; set; }
    string Category { get; set; }

    ICollection<ArticleAd> AdUnits { get; set; } 
  }
}

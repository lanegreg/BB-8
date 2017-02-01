using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public class ArticlePage : IArticlePage
  {
    public int ContentId { get; set; }
    public int ContentDetailId { get; set; }
    public int PageOrder { get; set; }
    public string PageTitle { get; set; }

    public string PageImage { get; set; }
    public string PageImageThumbnail { get; set; }
    public string PageImageThumbnailSmall { get; set; }
    public string VideoUrl { get; set; }
    public string VideoImageUrl { get; set; }

    public string PageText { get; set; }

    public int MakeId { get; set; }
    public string Make { get; set; }

    public int ModelId { get; set; }
    public string Model { get; set; }

    public int Year { get; set; }

    public int TrimId { get; set; }
    public string Trim { get; set; }

    public int CategoryId { get; set; }
    public string Category { get; set; }

    public ICollection<ArticleAd> AdUnits { get; set; } 

  }
}

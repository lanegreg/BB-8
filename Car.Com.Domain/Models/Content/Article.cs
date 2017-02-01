using System;
using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public class Article : IArticle
  {
    public int ContentId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string Author { get; set; }
    public DateTime PublishStartDate { get; set; }

    public string Category { get; set; }
    public string CategoryType { get; set; }
    public string CategorySeoName { get; set; }
    public string CategoryPluralName { get; set; }

    public bool DisplayAuthor { get; set; }
    public bool DisplayPublishDate { get; set; }

    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string SeoTitle { get; set; }

    public string ArticleType { get; set; }
    public string Template { get; set; }

    public int MakeId { get; set; }
    public string Make { get; set; }

    public int ModelId { get; set; }
    public string Model { get; set; }

    public int VehicleCategoryId { get; set; }
    public string VehicleCategory { get; set; }

    public int Year { get; set; }

    public int TrimId { get; set; }
    public string Trim { get; set; }
    
    public int CurrentPage { get; set; }
    public int NumPages { get; set; }

    public string MainArticleImage { get; set; }

    public string ImageAlt { get; set; }
    public string ImageTitle { get; set; }

    public string ImageCredit { get; set; }
    public string ImageCreditUrl { get; set; }

    public string VideoUrl { get; set; }
    public string VideoImageUrl { get; set; }

    public string Url { get; set; }
    public string UriPathPattern { get; set; }

    public int TotalRows { get; set; }  /* Used to store total records in full collection */

    public ICollection<Category> VehicleCategories { get; set; }

    public ICollection<Topic> Topics { get; set; }

    public ICollection<ArticlePage> ArticlePages { get; set; }

    public ICollection<ArticleAd> AdUnits { get; set; }

    public ICollection<Article> RelatedArticles { get; set; } 

  }
}

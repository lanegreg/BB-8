using System;
using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public interface IArticle
  {
     int ContentId { get; set; }
     string Title { get; set; }
     string SubTitle { get; set; }
     string Author { get; set; }
     DateTime PublishStartDate { get; set; }

     string Category { get; set; }
     string CategoryType { get; set; }
     string CategorySeoName { get; set; }
     string CategoryPluralName { get; set; }

     bool DisplayAuthor { get; set; }
     bool DisplayPublishDate { get; set; }

     string MetaKeywords { get; set; }
     string MetaDescription { get; set; }
     string SeoTitle { get; set; }

     string ArticleType { get; set; }
     string Template { get; set; }

     int MakeId { get; set; }
     string Make { get; set; }

     int ModelId { get; set; }
     string Model { get; set; }

     int VehicleCategoryId { get; set; }
     string VehicleCategory { get; set; }

     int Year { get; set; }

     int TrimId { get; set; }
     string Trim { get; set; }

     int CurrentPage { get; set; }
     int NumPages { get; set; }

     string MainArticleImage { get; set; }

     string ImageAlt { get; set; }
     string ImageTitle { get; set; }

     string ImageCredit { get; set; }
     string ImageCreditUrl { get; set; }

     string VideoUrl { get; set; }
     string VideoImageUrl { get; set; }

     string Url { get; set; }
     string UriPathPattern { get; set; }

     int TotalRows { get; set; }  /* Used to store total records in full collection */

     ICollection<Topic> Topics { get; set; }

     ICollection<ArticlePage> ArticlePages { get; set; }

     ICollection<ArticleAd> AdUnits { get; set; } 
  }
}

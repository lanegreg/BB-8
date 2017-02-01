using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public interface IArticleListWithSort
  {
    int TotalRecords { get; set; }
    ICollection<ArticleWithSort> Articles { get; set; }
  }
}

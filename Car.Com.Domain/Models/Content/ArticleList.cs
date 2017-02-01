using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public class ArticleList : IArticleList
  {
    public int TotalRecords { get; set; }
    public IList<Article> Articles { get; set; }
  }
}

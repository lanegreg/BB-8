using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public class ArticleListWithCategory : IArticleListWithCategory
  {
    public string Category { get; set; }
    public string CategoryType { get; set; }
    public string CategorySeoName { get; set; }
    public string CategoryPluralName { get; set; }
    public int TotalRecords { get; set; }
    public IList<Article> Articles { get; set; }
  }
}

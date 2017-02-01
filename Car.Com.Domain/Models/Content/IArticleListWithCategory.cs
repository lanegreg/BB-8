using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public interface IArticleListWithCategory
  {
    string Category { get; set; }
    string CategoryType { get; set; }
    string CategorySeoName { get; set; }
    string CategoryPluralName { get; set; }
  }
}

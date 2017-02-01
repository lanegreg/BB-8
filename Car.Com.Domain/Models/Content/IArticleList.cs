using System.Collections.Generic;

namespace Car.Com.Domain.Models.Content
{
  public interface IArticleList
  {
    int TotalRecords { get; set; }
    IList<Article> Articles { get; set; }
  }
}

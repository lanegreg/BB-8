using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.Content
{
  public class ArticleWithSort : IArticleWithSort
  {
    public int SortOrder { get; set; }
  }
}

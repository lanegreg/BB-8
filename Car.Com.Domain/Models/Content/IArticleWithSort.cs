using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.Content
{
  public interface IArticleWithSort
  {
    int SortOrder { get; set; }
  }
}

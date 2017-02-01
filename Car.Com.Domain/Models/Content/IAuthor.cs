using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.Content
{
  public interface IAuthor
  {
    int AuthorId { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    int OrderOnWeb { get; set; }
    string Title { get; set; }
    string Bio { get; set; }
    string GooglePlusUrl { get; set; }
    string TwitterUrl { get; set; }
    string FacebookUrl { get; set; }
    string Email { get; set; }
    string City { get; set; }
    string State { get; set; }
  }
}

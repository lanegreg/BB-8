using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.Content
{
  public class Author : IAuthor
  {
    public int AuthorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int OrderOnWeb { get; set; }
    public string Title { get; set; }
    public string Bio { get; set; }
    public string GooglePlusUrl { get; set; }
    public string TwitterUrl { get; set; }
    public string FacebookUrl { get; set; }
    public string Email { get; set; }
    public string City { get; set; }
    public string State { get; set; }
  }
}

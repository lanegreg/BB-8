using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.Content
{
  public class Topic : ITopic
  {
    public int TopicId { get; set; }
    public string TopicName { get; set; }
    public string ContentPriority { get; set; }
  }
}

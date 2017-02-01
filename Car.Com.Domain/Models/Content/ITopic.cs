using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car.Com.Domain.Models.Content
{
  public interface ITopic
  {
    int TopicId { get; set; }
    string TopicName { get; set; }
    string ContentPriority { get; set; }
  }
}

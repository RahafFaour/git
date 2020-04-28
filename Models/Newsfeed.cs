using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace meldboek.Models
{
    public class Newsfeed
    {
        public IEnumerable<Newspost> Post { get; set; }
        public IEnumerable<Group> Group { get; set; }
    }
}

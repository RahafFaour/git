using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace meldboek.Models
{
    public class Newspost
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DateAdded { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

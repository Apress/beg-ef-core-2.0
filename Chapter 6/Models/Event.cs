using System;
using System.Collections.Generic;

namespace EquineTracker.Models
{
    public partial class Event
    {
        public Event()
        {
            Result = new HashSet<Result>();
        }

        public int EventId { get; set; }
        public string Name { get; set; }
        public int LocationId { get; set; }
        public string Description { get; set; }
        public DateTime? EventDate { get; set; }

        public Location Location { get; set; }
        public ICollection<Result> Result { get; set; }
    }
}

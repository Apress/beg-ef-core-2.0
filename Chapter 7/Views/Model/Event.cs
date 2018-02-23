using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EquineTracker.Models {
    public partial class Event {
        public Event() {
            Result = new HashSet<Result>();
        }

        public int EventId { get; set; }
        [Required, StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        [Display(Name = "Location")]
        public int LocationId { get; set; }
        [Required, StringLength(50, MinimumLength = 3)]
        public string Description { get; set; }
        [Display(Name = "Event Date"), DataType(DataType.DateTime)]
        public DateTime? EventDate { get; set; }

        public Location Location { get; set; }
        public ICollection<Result> Result { get; set; }
    }
}

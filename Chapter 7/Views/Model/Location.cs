using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EquineTracker.Models {
    public partial class Location {
        private string _state;
        public Location() {
            Event = new HashSet<Event>();
        }

        public int LocationId { get; set; }
        [Required, StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        [Required, StringLength(50, MinimumLength = 6)]
        public string StreetAddress { get; set; }
        [Required, StringLength(50, MinimumLength = 3)]
        public string City { get; set; }
        [RegularExpression("[a-zA-Z][a-zA-Z]", ErrorMessage = "Must Be 2 Alpha Characters"), Required]
        public string State {
            get { return _state; }
            set { _state = value.ToUpper(); }
        }

        [StringLength(10, MinimumLength = 5)]
        public string ZipCode { get; set; }

        public ICollection<Event> Event { get; set; }
    }
}

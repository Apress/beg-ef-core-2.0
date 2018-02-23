using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EquineTracker.Models {
    public partial class Horse {
        public Horse() {
            Result = new HashSet<Result>();
        }

        public int HorseId { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal? Height { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Value { get; set; }

        public ICollection<Result> Result { get; set; }
    }
}

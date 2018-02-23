using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EquineTracker.Models {
    public partial class Horse {
        public Horse() {
            Result = new HashSet<Result>();
        }

        public int HorseId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Breed { get; set; }
        [RegularExpression(@"([1-2]|1[0-9])\.[123]", ErrorMessage = "Enter The Height In Hands, for example: 14.0 between 1.0 and 20.3")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal? Height { get; set; }
        [Range(0.0, double.MaxValue, ErrorMessage = "Value Must Be Positive"), DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Value { get; set; }

        public ICollection<Result> Result { get; set; }
    }
}

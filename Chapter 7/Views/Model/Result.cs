using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EquineTracker.Models
{
    public partial class Result
    {
        public int ResultId { get; set; }
        public int EventId { get; set; }
        public int HorseId { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public decimal Score { get; set; }
        [Required]
        public string Notes { get; set; }

        public Event Event { get; set; }
        public Horse Horse { get; set; }
    }
}

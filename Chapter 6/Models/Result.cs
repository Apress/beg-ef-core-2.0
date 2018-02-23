using System;
using System.Collections.Generic;

namespace EquineTracker.Models
{
    public partial class Result
    {
        public int ResultId { get; set; }
        public int EventId { get; set; }
        public int HorseId { get; set; }
        public string Class { get; set; }
        public decimal Score { get; set; }
        public string Notes { get; set; }

        public Event Event { get; set; }
        public Horse Horse { get; set; }
    }
}

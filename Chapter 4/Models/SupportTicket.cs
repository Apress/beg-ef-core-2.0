using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComputerInventory.Models {
    public partial class SupportTicket {
        public SupportTicket() {
            SupportLog = new HashSet<SupportLog>();
        }

        public int SupportTicketId { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateReported { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateResolved { get; set; }
        [Required]
        [StringLength(150)]
        public string IssueDescription { get; set; }
        [Required]
        public string IssueDetail { get; set; }
        [Required, StringLength(50)]
        public string TicketOpenedBy { get; set; }
        public int MachineId { get; set; }

        public Machine Machine { get; set; }
        public ICollection<SupportLog> SupportLog { get; set; }
    }
}

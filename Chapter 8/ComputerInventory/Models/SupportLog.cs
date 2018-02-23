using System;
using System.Collections.Generic;

namespace ComputerInventory.Models {
    public partial class SupportLog {
        public int SupportLogId { get; set; }
        public DateTime SupportLogEntryDate { get; set; }
        public string SupportLogEntry { get; set; }
        public string SupportLogUpdatedBy { get; set; }
        public int SupportTicketId { get; set; }

        public SupportTicket SupportTicket { get; set; }
    }
}
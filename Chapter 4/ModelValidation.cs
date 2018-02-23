using System;
using System.Collections.Generic;
using System.Text;

using ComputerInventory.Models;
using ComputerInventory.Data;
using System.Linq;

namespace ComputerInventory {
    public class Errors {
        public int Position { get; set; }
        public string FieldName { get; set; }
        public string Error { get; set; }
    }

    public class ModelValidation {
        public List<Errors> errorList;
        public ModelValidation() {
            errorList = new List<Errors>();
        }

        public bool ValidateSupportLog(SupportLog supportLog, string updateOrSave) {
            bool valid = true;
            Errors error;
            if (updateOrSave.ToLower() == "update") {
                valid = SupportTicketIdExists(supportLog.SupportTicketId);
                error = new Errors {
                    Position = 0,
                    FieldName = "SupportTicketId",
                    Error = "Not a valid SupportTicketId"
                };
                errorList.Add(error);
            }

            if (supportLog.SupportLogEntryDate < DateTime.Now.AddSeconds(-180)) {
                valid = false;
                error = new Errors {
                    Position = 0,
                    FieldName = "SupportLogEntryDate",
                    Error = "Date is not valid"
                };
                errorList.Add(error);
            }
            if (supportLog.SupportLogEntry.Length < 10) {
                valid = false;
                error = new Errors {
                    Position = 0,
                    FieldName = "SupportLogEntry",
                    Error = "Enter at least 10 characters."
                };
                errorList.Add(error);
            }
            if (supportLog.SupportLogUpdatedBy.Length < 2) {
                valid = false;
                error = new Errors {
                    Position = 0,
                    FieldName = "SupportLogUpdatedBy",
                    Error = "Enter at least 2 characters"
                };
                errorList.Add(error);
            }
            return valid;
        }


        public bool SupportTicketIdExists(int supportTicketId) {
            bool exists = false;
            using (MachineContext context = new MachineContext()) {
                exists = context.SupportTicket.Any(x => x.SupportTicketId.Equals(supportTicketId));
            }
            return exists;
        }
    }
}

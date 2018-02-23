using System;
using System.Collections.Generic;

using ComputerInventory.Models;
using Microsoft.EntityFrameworkCore;
using ComputerInventory.Data;
using System.Linq;

namespace ComputerInventory {
    public class SupportTicketLogEntry {
        private ConsoleHelper cHelper = new ConsoleHelper();

        public SupportTicketLogEntry() { }

        private void AddSupportLogEntry(string entry, bool close, int ticketId) {
            bool modelIsValid = false;
            ModelValidation mv = new ModelValidation();
            Console.WriteLine("Saving new log entry");
            using (MachineContext context = new MachineContext()) {
                SupportLog sLogEntry = new SupportLog() {
                    SupportTicketId = ticketId,
                    SupportLogEntry = entry,
                    SupportLogUpdatedBy = Environment.UserName,
                    SupportLogEntryDate = DateTime.Now
                };
                modelIsValid = mv.ValidateSupportLog(sLogEntry, "save");
                if (modelIsValid) {
                    context.SupportLog.Add(sLogEntry);
                    int res = context.SaveChanges();
                    Console.WriteLine($"{res} record saved");
                }
            }
            if (modelIsValid) {
                if (close) {
                    CloseTicket(ticketId);
                }
            }
            else {
                Console.WriteLine("There is a problem with your Entry");
                foreach (var error in mv.errorList) {
                    Console.WriteLine($"{error.FieldName} {error.Error}");
                }
            }
        }

        private void CloseTicket(int ticketId) {
            Console.WriteLine("Closing ticket...");
            using (MachineContext context = new MachineContext()) {
                SupportTicket sTicket = context.SupportTicket.Where(x => x.SupportTicketId == ticketId).FirstOrDefault();
                sTicket.DateResolved = DateTime.Now;
                context.Update(sTicket);
                context.SaveChanges();
                Console.WriteLine("Ticket is closed");
            }
        }

        public List<SupportLog> RetreiveLogEntries(int ticketId) {
            List<SupportLog> logEntries = new List<SupportLog>();
            using (MachineContext context = new MachineContext()) {
                logEntries = context.SupportLog
                   .Where(x => x.SupportTicketId == ticketId)
                   .OrderBy(x => x.SupportTicket).ToList();
            }
            return logEntries;
        }

        public List<SupportTicket> RetreiveOpenTickets() {
            List<SupportTicket> openTickets = new List<SupportTicket>();
            using (MachineContext context = new MachineContext()) {
                openTickets = context.SupportTicket
                   .Include(m => m.Machine)
                   .Where(x => x.DateResolved == null).ToList();
            }
            return openTickets;
        }

        public void UpdateSupportTicket() {
            Console.Clear();
            cHelper.WriteHeader("Update Support Ticket");
            Console.WriteLine("\r\nPlease Select an Open Support Ticket from the List\r\n");
            List<SupportTicket> openTickets = RetreiveOpenTickets();
            string logEntry;
            foreach (var ticket in openTickets) {
                Console.WriteLine($"Ticket Number: {ticket.SupportTicketId} Machine Name: {ticket.Machine.Name}\r\n    Description: {ticket.IssueDescription}");
            }
            int ticketId = cHelper.GetNumbersFromConsole();
            Console.WriteLine();
            bool validEntry = false;
            int errorCount = 0;
            do {
                if (openTickets.Exists(x => x.SupportTicketId == ticketId)) {
                    validEntry = true;
                }
                else {
                    Console.WriteLine($"{ticketId} is not a valid SupportId, please try again");
                    ticketId = cHelper.GetNumbersFromConsole();
                    Console.WriteLine();
                    errorCount++;
                }
                if (errorCount > 2) {
                    // 4 times should be enough, initial try plus 3
                    break;
                }
            } while (!validEntry);

            if (validEntry) {
                List<SupportLog> logEntries = RetreiveLogEntries(ticketId);
                Console.Clear();
                cHelper.WriteHeader("Update Support Ticket");
                Console.WriteLine("\r\n");
                if (logEntries.Count > 0) {
                    foreach (var log in logEntries) {
                        Console.WriteLine($"{log.SupportLogEntryDate}: {log.SupportLogEntry}");
                        Console.WriteLine($"   Entered By: {log.SupportLogUpdatedBy}");
                        Console.WriteLine("==============================");
                    }
                }
                else {
                    Console.WriteLine("There are currently no Log Entries");
                }
                Console.WriteLine("Please enter your notes, when you are done hit the Enter Key");
                logEntry = cHelper.GetTextFromConsole(10);
                Console.WriteLine("\r\nIs this ticket now closed? Y or N");
                bool closeTicket = false;
                char yOrN = cHelper.CheckForYorN(true);
                if (yOrN == 'y' | yOrN == 'Y') {
                    closeTicket = true;
                }
                AddSupportLogEntry(logEntry, closeTicket, ticketId);
            }
            else {
                Console.WriteLine("I'm sorry it looks like you are having trouble, please try again later.");
            }
            Console.WriteLine("Hit any key to continue...");
            Console.ReadKey();
        }
    }
}

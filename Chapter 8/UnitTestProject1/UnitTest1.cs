using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComputerInventory;
using ComputerInventory.Models;
using ComputerInventory.Data;
using System.Linq;
using System;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void Validate_Y_or_N() {
            // Arrange - method is static so no object needed
            //           If you needed an object you would do the following:
            BasicValidation validation = new BasicValidation();

            // Act - Test if ValidateYorN works
            bool bY = BasicValidation.ValidateYorN("Y");
            bool bN = BasicValidation.ValidateYorN("N");
            bool by = BasicValidation.ValidateYorN("y");
            bool bn = BasicValidation.ValidateYorN("n");
            bool bAnyOtherChar = BasicValidation.ValidateYorN("G");
            bool bNumToString = BasicValidation.ValidateYorN(9.ToString());

            // Assert
            Assert.IsTrue(bY, "Validating Y");
            Assert.IsTrue(bN, "Validating N");
            Assert.IsTrue(by, "Validating y");
            Assert.IsTrue(bn, "Validating n");
            Assert.IsFalse(bAnyOtherChar, "Validating G");
            Assert.IsFalse(bNumToString, "Validating 9");
        }

        [TestMethod]
        public void Check_For_Existing_OS() {
            // Arrange
            string osName = "Windows 7";

            // Act - Test for existing OSName in Database
            bool success = Program.CheckForExistingOS(osName);

            // Assert
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Retreive_Support_Ticket_By_ID() {
            // Arrange
            int ticketId = 1;
            SupportTicket sTicket;
            // Act
            using (MachineContext context = new MachineContext()) {
                sTicket = context.SupportTicket.Where(x => x.SupportTicketId == ticketId).FirstOrDefault();
            }

            //Assert
            Assert.AreEqual(1, sTicket.SupportTicketId, message: "TicketId test");
            Assert.AreEqual(5, sTicket.MachineId, message: "MachineId test");
        }

        [TestMethod]
        public void Validate_Support_Log() {
            // Arrange
            SupportLog supportLog = new SupportLog() {
                SupportLogEntryDate = DateTime.Now,
                SupportLogEntry = "Filler text, just needs to be long enough",
                SupportLogUpdatedBy = "John Smith"
            };
            SupportLog supportLog2 = new SupportLog() {
                SupportLogEntryDate = Convert.ToDateTime("2017-1-1 10:00"),
                SupportLogEntry = "fail",
                SupportLogUpdatedBy = "J"
            };

            ModelValidation modelValidation = new ModelValidation();

            // Act
            bool valid = modelValidation.ValidateSupportLog(supportLog, "save");
            bool invald = modelValidation.ValidateSupportLog(supportLog2, "save");

            // Assert
            Assert.IsTrue(valid);
            Assert.IsFalse(invald);
            Assert.AreEqual("Date is not valid", modelValidation.errorList[0].Error.ToString());
            Assert.AreEqual("Enter at least 10 characters.", modelValidation.errorList[1].Error.ToString());
            Assert.AreEqual("Enter at least 2 characters", modelValidation.errorList[2].Error.ToString());
        }
    }
}

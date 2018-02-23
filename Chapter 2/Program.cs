using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using ComputerInventory.Data;
using ComputerInventory.Models;

namespace ComputerInventory
{
    class Program
    {
        static void Main(string[] args) {
            // Set a color you like other than green or red as this will be used later
            Console.ForegroundColor = ConsoleColor.Black;
            int result = -1;
            while (result != 9) {
                result = MainMenu();
            }
        }

        static int MainMenu() {
            int result = -1;
            ConsoleKeyInfo cki;
            bool cont = false;
            do {
                Console.Clear();
                WriteHeader("Welcome to Newbie Data Systems");
                WriteHeader("Main Menu");
                Console.WriteLine("\r\nPlease select from the list below for what you would like to do today");
                Console.WriteLine("1. List All Machines in Inventory");
                Console.WriteLine("2. List All Operating Systems");
                Console.WriteLine("3. Data Entry Menu");
                Console.WriteLine("4. Data Modification Menu");
                Console.WriteLine("5. Update Operating Systems");
                Console.WriteLine("9. Exit");
                cki = Console.ReadKey();
                try {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    if (result == 1) {
                        DisplayAllMachines();
                        //SeedOperatingSystemTable();
                    }
                    else if (result == 2) {
                        DisplayOperatingSystems();
                    }
                    else if (result == 3) {
                        DataEntryMenu();
                    }
                    else if (result == 4) {
                        DataModificationMenu();
                    }
                    else if (result == 5) {
                        UpdateOperatingSystems();
                    }
                    else if (result == 9) {
                        // We are exiting so nothing to do
                        cont = true;
                    }
                }
                catch (System.FormatException) {
                    // a key that wasn't a number
                }
            } while (!cont);

            return result;
        }

        static void DataEntryMenu() {
            ConsoleKeyInfo cki;
            int result = -1;
            bool cont = false;
            do {
                Console.Clear();
                WriteHeader("Data Entry Menu");
                Console.WriteLine("\r\nPlease select from the list below for what you would like to do today");
                Console.WriteLine("1. Add a New Machine");
                Console.WriteLine("2. Add a New Operating System");
                Console.WriteLine("3. Add a New Warranty Provider");
                Console.WriteLine("4. Add new Machine Warranty Information");
                Console.WriteLine("9. Exit Menu");
                cki = Console.ReadKey();
                try {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    if (result == 1) {
                        AddMachine();
                    }
                    else if (result == 2) {
                        AddOperatingSystem();
                    }
                    else if (result == 3) {
                        WarrantyProvider wp = new WarrantyProvider();
                        wp = CreateNewWarrantyProvider();
                        AddNewWarrantyProvider(wp);
                    }
                    else if (result == 4) {
                        AddMachineWarrantyInfo();
                    }
                    else if (result == 9) {
                        // We are exiting so nothing to do
                        cont = true;
                    }
                }
                catch (System.FormatException) {
                    // a key that wasn't a number
                }
            } while (!cont);
        }

        static void WriteHeader(string headerText) {
            Console.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + headerText.Length / 2) + "}", headerText));
        }

        static bool ValidateYorN(string entry) {
            bool result = false;
            if (entry.ToLower() == "y" || entry.ToLower() == "n") {
                result = true;
            }
            return result;
        }

        static bool CheckForExistingOS(string osName) {
            bool exists = false;
            using (var context = new MachineContext()) {
                var os = context.OperatingSys.Where(o => o.Name == osName);
                if (os.Count() > 0) {
                    exists = true;
                }
            }
            return exists;
        }

        static OperatingSys AddOperatingSystem() {
            Console.Clear();
            ConsoleKeyInfo cki;
            string result;
            bool cont = false;
            OperatingSys os = new OperatingSys();
            string osName = "";
            do {
                WriteHeader("Add New Operating System");
                Console.WriteLine("Enter the Name of the Operating System and hit Enter");
                osName = Console.ReadLine();
                if (osName.Length >= 4) {
                    cont = true;
                }
                else {
                    Console.WriteLine("Please enter a vaild OS name of at least 4 characters.\r\nPress and key to continue...");
                    Console.ReadKey();
                }
            } while (!cont);
            cont = false;
            os.Name = osName;
            Console.WriteLine("Is the Operating System still supported? [y or n]");

            do {
                cki = Console.ReadKey();
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);

            if (result.ToLower() == "y") {
                os.StillSupported = true;
            }
            else {
                os.StillSupported = false;
            }
            cont = false;
            do {
                Console.Clear();
                Console.WriteLine($"You entered {os.Name} as the Operating System Name\r\nIs the OS still supported, you entered {os.StillSupported}.\r\nDo you wish to continue? [y or n]");
                cki = Console.ReadKey();
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);
            if (result.ToLower() == "y") {
                bool exists = CheckForExistingOS(os.Name);
                if (exists) {
                    Console.WriteLine("\r\nOperating System already exists in the database\r\nPress any key to continue...");
                    Console.ReadKey();
                }
                else {
                    using (var context = new MachineContext()) {
                        Console.WriteLine("\r\nAttempting to save changes...");
                        context.OperatingSys.Add(os);
                        int i = context.SaveChanges();
                        if (i == 1) {
                            Console.WriteLine("Contents Saved\r\nPress any key to continue...");
                            Console.ReadKey();
                        }
                    }
                }
            }
            return os;
        }

        static void DisplayOperatingSystems() {
            Console.Clear();
            Console.WriteLine("Operating Systems");
            using (var context = new MachineContext()) {
                foreach (var os in context.OperatingSys.ToList()) {
                    Console.Write($"Name: {os.Name,-39}\tStill Supported = ");
                    if (os.StillSupported == true) {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.WriteLine(os.StillSupported);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
            }
            Console.WriteLine("\r\nAny key to continue...");
            Console.ReadKey();
        }

        static void DeleteOperatingSystem(int id) {
            OperatingSys os = GetOperatingSystemById(id);
            if (os != null) {
                Console.WriteLine($"\r\nAre you sure you want to delete {os.Name}? [y or n]");
                ConsoleKeyInfo cki;
                string result;
                bool cont;
                do {
                    cki = Console.ReadKey(true);
                    result = cki.KeyChar.ToString();
                    cont = ValidateYorN(result);
                } while (!cont);
                if ("y" == result.ToLower()) {
                    Console.WriteLine("\r\nDeleting record");
                    using (var context = new MachineContext()) {
                        context.Remove(os);
                        context.SaveChanges();
                    }
                    Console.WriteLine("Record Deleted");
                    Console.ReadKey();
                }
                else {
                    Console.WriteLine("Delete Aborted\r\nHit any key to continue...");
                    Console.ReadKey();
                }
            }
            else {
                Console.WriteLine("\r\nOperating System Not Found!");
                Console.ReadKey();
                SelectOperatingSystem("Delete");
            }
        }

        static OperatingSys GetOperatingSystemById(int id) {
            var context = new MachineContext();
            OperatingSys os = context.OperatingSys.FirstOrDefault(i => i.OperatingSysId == id);
            return os;
        }

        static void SelectOperatingSystem(string operation) {
            ConsoleKeyInfo cki;
            Console.Clear();
            WriteHeader($"{operation} an Existing Operating System Entry");
            Console.WriteLine($"{"ID",-7}|{"Name",-50}|Still Supported");
            Console.WriteLine("--------------------------------------------------------------------------");
            using (var context = new MachineContext()) {
                List<OperatingSys> lOperatingSystems = context.OperatingSys.ToList();
                foreach (OperatingSys os in lOperatingSystems) {
                    Console.WriteLine($"{os.OperatingSysId,-7}|{os.Name,-50}|{os.StillSupported}");
                }
            }
            Console.WriteLine($"\r\nEnter the ID of the record you wish to {operation} and hit Enter\r\nYou can hit Esc to exit this menu");
            bool cont = false;
            string id = "";
            do {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Escape) {
                    cont = true;
                    id = "";
                }
                else if (cki.Key == ConsoleKey.Enter) {
                    if (id.Length > 0) {
                        cont = true;
                    }
                    else {
                        Console.WriteLine("Please enter an ID that is at least 1 digit.");
                    }
                }
                else if (cki.Key == ConsoleKey.Backspace) {
                    Console.Write("\b \b");
                    try {
                        id = GetNumbersFromConsole();
                    }
                    catch (System.ArgumentOutOfRangeException) {
                        // at the 0 position, can't go any further back
                    }
                }
                else {
                    if (char.IsNumber(cki.KeyChar)) {
                        id += cki.KeyChar.ToString();
                        Console.Write(cki.KeyChar.ToString());
                    }
                }
            } while (!cont);

            int osId = Convert.ToInt32(id);
            if ("Delete" == operation) {
                DeleteOperatingSystem(osId);
            }
            else if ("Modify" == operation) {
                ModifyOperatingSystem(osId);
            }
        }

        static void DataModificationMenu() {
            ConsoleKeyInfo cki;
            int result = -1;
            bool cont = false;
            do {
                Console.Clear();
                WriteHeader("Data Modification Menu");
                Console.WriteLine("\r\nPlease select from the list below for what you would like to do today");
                Console.WriteLine("1. Delete Operating System");
                Console.WriteLine("2. Modify Operating System");
                Console.WriteLine("3. Delete All Unsupported Operating Systems");
                Console.WriteLine("4. Update Machine Information");
                Console.WriteLine("9. Exit Menu");
                cki = Console.ReadKey();
                try {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    if (result == 1) {
                        SelectOperatingSystem("Delete");
                    }
                    else if (result == 2) {
                        SelectOperatingSystem("Modify");
                    }
                    else if (result == 3) {
                        DeleteAllUnsupportedOperatingSystems();
                    }
                    else if (result == 4) {
                        UpdateMachineDetails();
                    }
                    else if (result == 9) {
                        // We are exiting so nothing to do
                        cont = true;
                    }
                }
                catch (System.FormatException) {
                    // a key that wasn't a number
                }
            } while (!cont);
        }

        static void DeleteAllUnsupportedOperatingSystems() {
            using (var context = new MachineContext()) {
                var os = (from o in context.OperatingSys where o.StillSupported == false select o);
                Console.WriteLine("\r\nDeleting all Unsupported Operating Systems...");
                context.OperatingSys.RemoveRange(os);
                int i = context.SaveChanges();
                Console.WriteLine($"We have deleted {i} records");
                Console.WriteLine("Hit any key to continue...");
                Console.ReadKey();
            }
        }

        static void ModifyOperatingSystem(int id) {
            OperatingSys os = GetOperatingSystemById(id);
            Console.Clear();
            char operation = '0';
            bool cont = false;
            ConsoleKeyInfo cki;
            WriteHeader("Update Operating System");
            if (os != null) {
                Console.WriteLine($"\r\nOS Name: {os.Name}  Still Supported: {os.StillSupported}");
                Console.WriteLine("To modify the name press 1\r\nTo modify if the OS is Still Supported press 2");
                Console.WriteLine("Hit Esc to exit this menu");

                do {
                    cki = Console.ReadKey(true);
                    if (cki.Key == ConsoleKey.Escape)
                        cont = true;
                    else {
                        if (char.IsNumber(cki.KeyChar)) {
                            if (cki.KeyChar == '1') {
                                Console.WriteLine("Updated Operating System Name: ");
                                operation = '1';
                                cont = true;
                            }
                            else if (cki.KeyChar == '2') {
                                Console.WriteLine("Update if the OS is Still Supported [y or n]: ");
                                operation = '2';
                                cont = true;
                            }
                        }
                    }
                } while (!cont);
            }
            if (operation == '1') {
                string osName;
                cont = false;
                do {
                    osName = Console.ReadLine();
                    if (osName.Length >= 4) {
                        cont = true;
                    }
                    else {
                        Console.WriteLine("Please enter a vaild OS name of at least 4 characters.\r\nPress and key to continue...");
                        Console.ReadKey();
                    }
                } while (!cont);
                os.Name = osName;
            }
            else if (operation == '2') {
                string k;
                do {
                    cki = Console.ReadKey(true);
                    k = cki.KeyChar.ToString();
                    cont = ValidateYorN(k);
                } while (!cont);
                if (k == "y") {
                    os.StillSupported = true;
                }
                else {
                    os.StillSupported = false;
                }
            }
            using (var context = new MachineContext()) {
                var o = context.OperatingSys.FirstOrDefault(i => i.OperatingSysId == os.OperatingSysId);
                if (o != null) {
                    // just making sure
                    o.Name = os.Name;
                    o.StillSupported = os.StillSupported;
                    Console.WriteLine("\r\nUpdating the database...");
                    context.SaveChanges();
                    Console.WriteLine("Done!\r\nHit any key to continue...");
                }
            }
            Console.ReadKey();
        }

        static void AddMachine() {
            Console.Clear();
            ConsoleKeyInfo cki;
            string result;
            bool cont = false;
            Machine machine = new Machine();
            WriteHeader("Add New Machine");
            Console.WriteLine();
            List<MachineType> lMachineType = GetMachineTypes();
            MachineType machineType = new MachineType();
            Models.OperatingSys os = new Models.OperatingSys();
            if (lMachineType.Count == 0) {
                // No results so we need to add a machine type
                machineType = AddMachineType();
            }
            else {
                // We have at least one Result so we should display the results for the user to select from
                DisplayMachineTypes(lMachineType);
                Console.WriteLine("Enter the ID for the Machine Type you would like to use.");
                Console.WriteLine("To add a new Machine Type enter [a]");
                do {
                    cki = Console.ReadKey(true);
                    if (cki.Key == ConsoleKey.A) {
                        machineType = AddMachineType();
                        cont = true;
                    }
                    else {
                        if (char.IsNumber(cki.KeyChar)) {
                            int idEntered = Convert.ToInt16(cki.KeyChar.ToString());
                            if (lMachineType.Exists(x => x.MachineTypeId == idEntered)) {
                                machineType = lMachineType.Find(x => x.MachineTypeId == idEntered);
                                cont = true;
                            }
                            else {
                                // No match, could add a counter here and after a certain number of attempts
                                // Add in some error handling
                            }
                        }
                    }
                } while (!cont);
            }

            machine.MachineTypeId = machineType.MachineTypeId;
            // We have our machine type, now to get the machine info
            Console.WriteLine("What is the name of the new machine?");
            machine.Name = Console.ReadLine();

            Console.WriteLine("What is the general role for this machine?");
            Console.WriteLine("For example a server could be a Domain Contorller or Database Server.");
            machine.GeneralRole = Console.ReadLine();

            Console.WriteLine("What is the specific role of this machine? <sepearate each with a comma>");
            Console.WriteLine("For example a DC could have the following roles: DNS, DHCP");
            machine.InstalledRoles = Console.ReadLine();

            // Now we need to get the operating system
            Console.WriteLine($"\r\n{"ID",-7}|{"Name",-50}|StillSupported");
            Console.WriteLine("--------------------------------------------------------------------------");
            using (var context = new MachineContext()) {
                List<OperatingSys> lOperatingSys = context.OperatingSys.ToList();
                foreach (OperatingSys o in lOperatingSys) {
                    Console.WriteLine($"{o.OperatingSysId,-7}|{o.Name,-50}|{o.StillSupported}");
                }
            }
            Console.WriteLine("\r\nEnter the ID of the Operating System you wish to use.");
            Console.WriteLine("If you need to add an Operating System hit the 'a' key.");
            Console.WriteLine("When you are done hit the Enter Key.");

            string id = string.Empty;
            cont = false;
            do {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.A) {
                    cont = true;
                    os = AddOperatingSystem();
                }
                else if (cki.Key == ConsoleKey.Enter) {
                    if (id.Length > 0) {
                        // Need to verify the OS is "good"
                        os = GetOperatingSystemById(Convert.ToInt16(id));
                        if (os == null) {
                            Console.WriteLine("\r\nOperating System ID is not valid, please try again.");
                            // Clear out the id so we can start over
                            id = string.Empty;
                        }
                        else {
                            cont = true;
                        }
                    }
                    else {
                        Console.WriteLine("Please enter an ID that is at least 1 digit.");
                    }
                }
                else if (cki.Key == ConsoleKey.Backspace) {
                    Console.Write("\b \b");
                    try {
                        id = id.Substring(0, id.Length - 1);
                    }
                    catch (System.ArgumentOutOfRangeException) {
                        // at the 0 position, can't go any further back
                    }
                }
                else {
                    if (char.IsNumber(cki.KeyChar)) {
                        id += cki.KeyChar.ToString();
                        Console.Write(cki.KeyChar.ToString());
                    }
                }
            } while (!cont);

            machine.OperatingSysId = os.OperatingSysId;

            Console.Clear();
            WriteHeader("Add New Machine");
            Console.WriteLine("\r\nYou have entered the following:");
            Console.WriteLine($"\tName: {machine.Name}");
            Console.WriteLine($"\tType: {machineType.Description}");
            Console.WriteLine($"\tOperating System: {os.Name}");
            Console.WriteLine($"\tRole: {machine.GeneralRole}");
            Console.WriteLine($"\tInstalled Roles: {machine.InstalledRoles}");
            Console.WriteLine("Would you like to save your work? [y or n]");

            cont = false;
            do {
                cki = Console.ReadKey();
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);

            if (result.ToLower() == "y") {
                using (MachineContext context = new MachineContext()) {
                    Console.WriteLine($"Saving {machine.Name} to the database");
                    context.Machine.Add(machine);
                    context.SaveChanges();
                    Console.WriteLine("Done!");
                }
            }
            Console.WriteLine("\r\nHit any key to continue");
            Console.ReadKey();
        }

        static List<MachineType> GetMachineTypes() {
            List<MachineType> lMachineType = new List<MachineType>();
            using (var context = new MachineContext()) {
                lMachineType = context.MachineType.ToList();
            }
            return lMachineType;
        }

        static MachineType AddMachineType() {
            Console.Clear();
            ConsoleKeyInfo cki;
            string result;
            bool cont = false;
            MachineType mt = new MachineType();
            string mName = "";
            do {
                WriteHeader("Add New Machine Type");
                Console.WriteLine("Enter a Description for the Machine Type and hit Enter");
                mName = Console.ReadLine();
                if (mName.Length >= 6) {
                    cont = true;
                    mt.Description = mName;
                }
                else {
                    Console.WriteLine("Please enter a vaild Descriptioin of at least 6 characters.\r\nPress and key to continue...");
                    Console.ReadKey();
                }
            } while (!cont);

            cont = false;
            do {
                Console.Clear();
                Console.WriteLine($"You entered {mt.Description} as the Description.\r\nDo you wish to continue? [y or n]");
                cki = Console.ReadKey();
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);

            if (result.ToLower() == "y") {
                bool exists = CheckForExistingMachineType(mt.Description);
                if (exists) {
                    Console.WriteLine("\r\nMachine Type already exists in the database\r\nPress any key to continue...");
                    Console.ReadKey();
                }
                else {
                    using (var context = new MachineContext()) {
                        Console.WriteLine("\r\nAttempting to save changes...");
                        context.MachineType.Add(mt);
                        int i = context.SaveChanges();
                        if (i == 1) {
                            Console.WriteLine("Contents Saved\r\nPress any key to continue...");
                            Console.ReadKey();
                        }
                    }
                }
            }
            return mt;
        }

        static bool CheckForExistingMachineType(string mtDesc) {
            bool exists = false;
            using (var context = new MachineContext()) {
                var mt = context.MachineType.Where(t => t.Description == mtDesc);
                if (mt.Count() > 0) {
                    exists = true;
                }
            }
            return exists;
        }

        static void DisplayMachineTypes(List<MachineType> lMachineType) {
            foreach (MachineType mt in lMachineType) {
                Console.WriteLine($"ID: {mt.MachineTypeId} Description: {mt.Description}");
            }
        }

        static WarrantyProvider CreateNewWarrantyProvider() {
            string provider = string.Empty;
            string phoneNumber = string.Empty;
            int? extension = null;
            ConsoleKeyInfo cki;
            bool cont = false;
            Console.Clear();
            WriteHeader("Add new Warranty Provider");
            Console.WriteLine("\r\n\r\nPlease enter the name of the Provider and then hit Enter.");
            provider = Console.ReadLine();

            Console.WriteLine("\r\nPlease enter the Providers phone number.");
            do {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter) {
                    if (phoneNumber.Length >= 7) {
                        cont = true;
                    }
                    else {
                        Console.WriteLine("Please enter a Phonenumber that is at least 7 digits.");
                    }
                }
                else if (cki.Key == ConsoleKey.Backspace) {
                    Console.Write("\b \b");
                    try {
                        phoneNumber = phoneNumber.Substring(0, phoneNumber.Length - 1);
                    }
                    catch (System.ArgumentOutOfRangeException) {
                        // at the 0 position, can't go any further back
                    }
                }
                else {
                    if (char.IsNumber(cki.KeyChar)) {
                        phoneNumber += cki.KeyChar.ToString();
                        Console.Write(cki.KeyChar.ToString());
                    }
                }
            } while (!cont);

            cont = false;
            Console.WriteLine("\r\nPlease enter the Providers extension.\r\nIf there is no extension hit ESC or Enter to continue.");
            string tempExt = string.Empty;
            tempExt = GetNumbersFromConsole();
            if(tempExt.Length > 0) {
                extension = Convert.ToInt32(tempExt);
            }
            else {
                extension = null;
            }
            //do {
            //    cki = Console.ReadKey(true);
            //    if (cki.Key == ConsoleKey.Escape) {
            //        cont = true;
            //        extension = null;
            //    }
            //    else if (cki.Key == ConsoleKey.Enter) {
            //        // can be any length or null
            //        if (tempExt.Length > 0)
            //            extension = Convert.ToInt32(tempExt);
            //        else
            //            extension = null;
            //        cont = true;
            //    }
            //    else if (cki.Key == ConsoleKey.Backspace) {
            //        Console.Write("\b \b");
            //        try {
            //            tempExt = tempExt.Substring(0, tempExt.Length - 1);
            //        }
            //        catch (System.ArgumentOutOfRangeException) {
            //            // at the 0 position, can't go any further back
            //        }
            //    }
            //    else {
            //        if (char.IsNumber(cki.KeyChar)) {
            //            tempExt += cki.KeyChar.ToString();
            //            Console.Write(cki.KeyChar.ToString());
            //        }
            //    }
            //} while (!cont);

            WarrantyProvider wp = new WarrantyProvider() {
                ProviderName = provider,
                SupportNumber = phoneNumber,
                SupportExtension = extension
            };
            return wp;
        }

        static void AddNewWarrantyProvider(WarrantyProvider wp) {
            bool cont = false;
            ConsoleKeyInfo cki;
            string result = string.Empty;
            Console.WriteLine("\r\nYou have entered the following:");
            Console.WriteLine($"Provider Name: {wp.ProviderName}");
            Console.WriteLine($"Provider Number: {wp.SupportNumber}");
            Console.WriteLine($"Provider Extension: {wp.SupportExtension}");
            Console.WriteLine("\r\nIs this information correct? [y or n]");
            do {
                cki = Console.ReadKey(true);
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);

            if (result.ToLower() == "y") {
                using (MachineContext context = new MachineContext()) {
                    var count = context.WarrantyProvider.Count(p => p.ProviderName == wp.ProviderName);
                    if (count == 0) {
                        Console.WriteLine($"\r\nSaving {wp.ProviderName} to the database...");
                        context.WarrantyProvider.Add(wp);
                        context.SaveChanges();
                        Console.WriteLine("Complete!");
                    }
                    else {
                        Console.WriteLine(string.Format("\r\nWarranty Provider {0} already exists", wp.ProviderName));
                    }
                }
            }
            else {
                Console.WriteLine("No changes have been made.");
            }
            Console.WriteLine("\r\nHit any key to continue...");
            Console.ReadKey();
        }

        static void AddMachineWarrantyInfo() {
            bool cont = false;
            MachineWarranty machineWarranty = new MachineWarranty();
            WarrantyProvider warrantyProvider = new WarrantyProvider();
            Machine machine = new Machine();
            ConsoleKeyInfo cki;
            int machineId = -1;
            string serviceTag;
            Console.Clear();
            WriteHeader("Add Warranty Info To an Existing Machine");
            Console.WriteLine();
            List<Machine> lMachine = GetListOfMachines();
            foreach (Machine m in lMachine) {
                Console.WriteLine($"ID: {m.MachineId}  Name: {m.Name}");
            }
            Console.WriteLine("\r\nEnter the ID of the Machine you wish to add Warranty Info for and then hit the Enter key.");

            string tempMachineId = string.Empty;
            do {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter) {
                    if (tempMachineId.Length > 0) {
                        machineId = Convert.ToInt32(tempMachineId);
                        machine = lMachine.Find(x => x.MachineId == machineId);
                    }
                    cont = true;
                }
                else if (cki.Key == ConsoleKey.Backspace) {
                    Console.Write("\b \b");
                    try {
                        tempMachineId = tempMachineId.Substring(0, tempMachineId.Length - 1);
                    }
                    catch (System.ArgumentOutOfRangeException) {
                        // at the 0 position, can't go any further back
                    }
                }
                else {
                    if (char.IsNumber(cki.KeyChar)) {
                        tempMachineId += cki.KeyChar.ToString();
                        Console.Write(cki.KeyChar.ToString());
                    }
                }
            } while (!cont);

            Console.WriteLine("\r\nEnter the Service Tag of the machine and then hit Enter.");
            serviceTag = Console.ReadLine();

            cont = false;
            string tDate;
            DateTime warrantyExp;
            do {
                Console.WriteLine("Enter the date the warranty expires [mm/dd/yyyy].");
                tDate = Console.ReadLine();

                if (DateTime.TryParse(tDate, out warrantyExp)) {
                    cont = true;
                }
                else {
                    Console.WriteLine("\r\nNot a valid date, please try again.");
                }
            } while (!cont);

            List<WarrantyProvider> lWarrantyProvider = new List<WarrantyProvider>();
            using (MachineContext context = new MachineContext()) {
                lWarrantyProvider = context.WarrantyProvider.ToList();
            }
            Console.WriteLine();
            foreach (WarrantyProvider wp in lWarrantyProvider) {
                Console.WriteLine($"ID: {wp.WarrantyProviderId}   Name: {wp.ProviderName}");
            }

            machineWarranty.ServiceTag = serviceTag;
            machineWarranty.WarrantyExpiration = warrantyExp;
            machineWarranty.MachineId = machineId;

            Console.WriteLine("Enter the ID for the Warranty Provider you would like to use.");
            Console.WriteLine("To add a new Provider enter [a]");
            string tempProviderId = string.Empty;
            cont = false;
            do {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.A) {
                    cont = true;
                    warrantyProvider = CreateNewWarrantyProvider();
                    machineWarranty.WarrantyProvider = warrantyProvider;
                }
                else if (cki.Key == ConsoleKey.Enter) {
                    if (tempProviderId.Length > 0) {
                        int provID = Convert.ToInt32(tempProviderId);
                        if (lWarrantyProvider.Exists(x => x.WarrantyProviderId == provID)) {
                            warrantyProvider = lWarrantyProvider.Find(x => x.WarrantyProviderId == provID);
                            machineWarranty.WarrantyProviderId = provID;
                            cont = true;
                        }
                        else {
                            Console.WriteLine("No match please verify your entry and try again!");
                        }
                    }
                    else {
                        Console.WriteLine("Please enter an ID that is at least 1 digit.");
                    }
                }
                else if (cki.Key == ConsoleKey.Backspace) {
                    Console.Write("\b \b");
                    try {
                        tempProviderId = tempProviderId.Substring(0, tempProviderId.Length - 1);
                    }
                    catch (System.ArgumentOutOfRangeException) {
                        // at the 0 position, can't go any further back
                    }
                }
                else {
                    if (char.IsNumber(cki.KeyChar)) {
                        tempProviderId += cki.KeyChar.ToString();
                        Console.Write(cki.KeyChar.ToString());
                    }
                }
            } while (!cont);

            Console.Clear();
            WriteHeader("Add Warranty Info To an Existing Machine");
            Console.WriteLine("You have entered the following:");
            Console.WriteLine($"Machine: {machine.Name}");
            Console.WriteLine($"Service Tag: {machineWarranty.ServiceTag}");
            Console.WriteLine($"Warranty Expiration: {machineWarranty.WarrantyExpiration.ToShortDateString()}");
            Console.WriteLine($"Warranty Provider: {warrantyProvider.ProviderName}");

            Console.WriteLine("\r\nIs this information correct? [y or n]");
            string result;
            do {
                cki = Console.ReadKey(true);
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);
            Console.WriteLine();

            if (result.ToLower() == "y") {
                Console.WriteLine("Saving Machine Warranty Information");
                using (MachineContext context = new MachineContext()) {
                    context.MachineWarranty.Add(machineWarranty);
                    context.SaveChanges();
                    Console.WriteLine("Save complete!");
                }
            }
            Console.WriteLine("Any key to continue...");
            Console.ReadKey();
        }

        static List<Machine> GetListOfMachines() {
            List<Machine> lmachine = new List<Machine>();
            MachineContext context = new MachineContext();
            lmachine = context.Machine.ToList();
            context.Dispose();
            return lmachine;
        }

        static string DisplayAllMachines() {
            Console.Clear();
            List<Machine> lMachine = GetListOfMachines();
            Console.WriteLine("Machine ID | Machine Name");
            foreach (Machine m in lMachine) {
                Console.WriteLine($"{m.MachineId,-11}| {m.Name}");
            }

            Console.WriteLine("\r\nEnter the MachineId followed by the Enter Key for more information.");
            Console.WriteLine("Hit the Esc key at anytime to exit the menu.");

            string machineId = GetNumbersFromConsole();
            if (machineId.Length > 0) {
                int machId = Convert.ToInt32(machineId);
                DisplayMachineDetail(machId);
            }
            return machineId;
        }

        static string GetNumbersFromConsole() {
            ConsoleKeyInfo cki;
            bool cont = false;
            string numbers = string.Empty;
            do {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Escape) {
                    cont = true;
                    numbers = "";
                }
                else if (cki.Key == ConsoleKey.Enter) {
                    if (numbers.Length > 0) {
                        cont = true;
                    }
                    else {
                        Console.WriteLine("Please enter an ID that is at least 1 digit.");
                    }
                }
                else if (cki.Key == ConsoleKey.Backspace) {
                    Console.Write("\b \b");
                    try {
                        numbers = numbers.Substring(0, numbers.Length - 1);
                    }
                    catch (System.ArgumentOutOfRangeException) {
                        // at the 0 position, can't go any further back
                    }
                }
                else {
                    if (char.IsNumber(cki.KeyChar)) {
                        numbers += cki.KeyChar.ToString();
                        Console.Write(cki.KeyChar.ToString());
                    }
                }
            } while (!cont);
            return numbers;
        }

        static void DisplayMachineDetail(int machineId) {
            Console.Clear();
            Console.WriteLine("\r\nRetrieving information about our machine.");
            using (MachineContext context = new MachineContext()) {
                var mach = (from mac in context.Machine
                            join mw in context.MachineWarranty on mac.MachineId equals mw.MachineId
                            where mac.MachineId == machineId
                            select new { mac.Name, MacType = mac.MachineType.Description, osName = mac.OperatingSys.Name, mac.OperatingSys.StillSupported, mw.WarrantyProvider, mw.WarrantyExpiration, mac.MachineType.Description }).FirstOrDefault();
                if (mach != null) {
                    Console.WriteLine($"Machine: {mach.Name} is a {mach.Description} \r\nOS = {mach.osName}  Still Supported?: {mach.StillSupported}");
                    Console.WriteLine($"Warranty Provider = {mach.WarrantyProvider.ProviderName}  Support Ends on: {mach.WarrantyExpiration.ToShortDateString()}");
                }
                else {
                    Console.WriteLine($"No Machine Found with ID: {machineId}");
                }
            }
            Console.WriteLine("Hit any key to continue");
            Console.ReadKey();
        }

        static void UpdateMachineDetails() {
            Console.Clear();
            List<Machine> lMachine = GetListOfMachines();
            WriteHeader("Update Machine Details");

            string machineId = DisplayAllMachines();
            if (machineId.Length > 0) {
                int mId = Convert.ToInt32(machineId);
                Machine mach;
                MachineWarranty mWar;
                using (MachineContext context = new MachineContext()) {
                    mach = context.Machine
                       .Include(o => o.OperatingSys)
                       .Include(t => t.MachineType)
                       .Where(x => x.MachineId == mId).FirstOrDefault();

                    mWar = context.MachineWarranty
                       .Include(p => p.WarrantyProvider)
                       .Where(x => x.MachineId == mach.MachineId).FirstOrDefault();
                }
                //Console.Clear();
                //Console.WriteLine("To Update the Machine Information Press 1");
                //Console.WriteLine($"Name: {mach.Name}  General Role: { mach.GeneralRole}");
                //Console.WriteLine($"Installed Role: {mach.InstalledRoles}");
                //Console.WriteLine("\r\nTo Update the Operating System Information Press 2");
                //Console.WriteLine($"OS Name: {mach.OperatingSys.Name}  Still Supported: {mach.OperatingSys.StillSupported}");
                //Console.WriteLine($"\r\nTo Update the Machine Type Press 3");
                //Console.WriteLine($"Machine Type: {mach.MachineType.Description}");
                //Console.WriteLine($"\r\nTo Update the Warranty Information Press 4");
                //Console.WriteLine($"Provider Name: {mWar.WarrantyProvider.ProviderName}");
                //Console.WriteLine($"Expiration: {mWar.WarrantyExpiration.ToShortDateString()}");

                UpdateMachine(mach.MachineId);
            }
            //Console.ReadKey();
        }

        static char CheckForYorN(bool intercept) {
            ConsoleKeyInfo cki;
            char entry;
            bool cont = false;
            do {
                cki = Console.ReadKey(intercept);
                entry = cki.KeyChar;
                cont = ValidateYorN(entry.ToString());
            } while (!cont);
            return entry;
        }

        static string GetTextFromConsole(int minLength, bool allowEscape = false) {
            ConsoleKeyInfo cki;
            bool cont = false;
            string rtnValue = string.Empty;
            do {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Escape) {
                    if (allowEscape) {
                        cont = true;
                        rtnValue = "";
                    }
                }
                else if (cki.Key == ConsoleKey.Enter) {
                    if (rtnValue.Length >= minLength) {
                        cont = true;
                    }
                    else {
                        Console.WriteLine($"Please enter least {minLength} characters.");
                    }
                }
                else if (cki.Key == ConsoleKey.Backspace) {
                    Console.Write("\b \b");
                    try {
                        rtnValue = rtnValue.Substring(0, rtnValue.Length - 1);
                    }
                    catch (System.ArgumentOutOfRangeException) {
                        // at the 0 position, can't go any further back
                    }
                }
                else {
                    rtnValue += cki.KeyChar.ToString();
                    Console.Write(cki.KeyChar.ToString());
                }
            } while (!cont);
            return rtnValue;
        }

        static void UpdateMachine(int machineId) {
            Console.Clear();
            char response;
            using (MachineContext context = new MachineContext()) {
                Machine machine = context.Machine.Where(x => x.MachineId == machineId).FirstOrDefault();
                if (machine != null) {
                    Console.WriteLine($"Machine Name: {machine.Name}\r\nDo you wish to change it?");
                    response = CheckForYorN(true);
                    if (response == 'y') {
                        Console.WriteLine("Enter a new Machine Name and then hit Enter to continue");
                        machine.Name = GetTextFromConsole(3);
                    }
                    Console.WriteLine($"General Role: {machine.GeneralRole}\r\nDo you wish to change it?");
                    response = CheckForYorN(true);
                    if (response == 'y') {
                        Console.WriteLine("Enter a new General Role For the Machine and then hit Enter to continue");
                        machine.GeneralRole = GetTextFromConsole(5);
                    }
                    Console.WriteLine($"Installed Roles: {machine.InstalledRoles}\r\nDo you wish to change it?");
                    response = CheckForYorN(true);
                    if (response == 'y') {
                        Console.WriteLine("Enter a new listing of Installed Roles For the Machine and then hit Enter to continue");
                        Console.WriteLine("You can hit Esc to exit without making any changes");
                        string result = GetTextFromConsole(5, true);
                        if (result.Length >= 5) {
                            machine.InstalledRoles = result;
                        }
                    }
                }
                if (context.ChangeTracker.HasChanges()) {
                    Console.WriteLine("\r\nSaving changes!");
                    context.SaveChanges();
                }
                else {
                    Console.WriteLine("\r\nNo Changes...");
                }
            }
            Console.WriteLine("Hit any key to continue...");
            Console.ReadKey();
        }

        static Machine GetMachine() {
            Machine machine;
            List<Machine> lMachine = GetListOfMachines();
            Console.WriteLine("Machine ID | Machine Name");
            foreach (Machine m in lMachine) {
                Console.WriteLine($"{m.MachineId,-11}| {m.Name}");
            }
            Console.WriteLine("\r\nEnter the ID of the Machine you want then hit the Enter Key");
            int mId = Convert.ToInt16(GetNumbersFromConsole());
            machine = lMachine.Where(x => x.MachineId == mId).FirstOrDefault();
            return machine;
        }

        static void UpdateOperatingSystems() {
            ConsoleKeyInfo cki;
            string result;
            int newOsId = -1;
            bool valid = false;
            Console.Clear();
            WriteHeader($"Operating System Update");
            List<Machine> lMachines = new List<Machine>();
            bool cont = false;
            do {
                valid = false;
                lMachines.Add(GetMachine());
                Console.WriteLine("Do you want to add another Machine? [y or n]");
                do {
                    cki = Console.ReadKey(true);
                    result = cki.KeyChar.ToString();
                    valid = ValidateYorN(result);
                } while (!valid);
                if (result.ToLower() == "n") {
                    cont = true;
                }
            } while (!cont);

            Console.WriteLine($"{"ID",-7}|{"Name",-50}|Still Supported");
            Console.WriteLine("--------------------------------------------------------------------------");
            using (var context = new MachineContext()) {
                List<OperatingSys> lOperatingSystems = context.OperatingSys.ToList();
                foreach (OperatingSys os in lOperatingSystems) {
                    Console.WriteLine($"{os.OperatingSysId,-7}|{os.Name,-50}|{os.StillSupported}");
                }
            }

            Console.WriteLine("\r\nEnter the ID of the Operating System you want to Update to.");
            Console.WriteLine("Then hit the Enter Key");
            string newOs = GetNumbersFromConsole();
            newOsId = Convert.ToInt16(newOs);

            using (MachineContext context = new MachineContext()) {
                foreach (Machine m in lMachines) {
                    m.OperatingSysId = newOsId;
                    context.Update(m);
                }
                context.SaveChanges();
            }
        }

        static void SeedOperatingSystemTable() {
            using (var context = new MachineContext()) {
                var os = new OperatingSys { Name = "Windows XP", StillSupported = false };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows 7", StillSupported = true };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows 8", StillSupported = false };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows 8.1", StillSupported = true };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows 10", StillSupported = true };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows Server 2000", StillSupported = false };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows Server 2003 R2", StillSupported = false };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows Server 2008", StillSupported = true };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows Server 2008 R2", StillSupported = true };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows Server 2012", StillSupported = true };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows Server 2012 R2", StillSupported = true };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Windows Server 2016", StillSupported = true };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Ubuntu Server 16.14.2 LTS", StillSupported = true };
                context.OperatingSys.Add(os);
                os = new OperatingSys { Name = "Ubuntu Server 17.04", StillSupported = true };
                context.OperatingSys.Add(os);
                context.SaveChanges();
            }
        }
    }
}

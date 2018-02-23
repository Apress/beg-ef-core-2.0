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
                Console.WriteLine("9. Exit");
                cki = Console.ReadKey();
                try {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    if (result == 1) {
                        //DisplayAllMachines();
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
                Console.WriteLine("9. Exit Menu");
                cki = Console.ReadKey();
                try {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    if (result == 1) {
                        //AddMachine();
                    }
                    else if (result == 2) {
                        AddOperatingSystem();
                    }
                    else if (result == 3) {
                        //AddNewWarrantyProvider();
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

        static void AddOperatingSystem() {
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
                //os = new OperatingSys { Name = "Windows Server 2003 R2", StillSupported = false };
                //context.OperatingSys.Add(os);
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

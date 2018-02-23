using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerInventory {
    public class ConsoleHelper {
        public void WriteHeader(string headerText) => Console.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + headerText.Length / 2) + "}", headerText));

        public char CheckForYorN(bool intercept) {
            ConsoleKeyInfo cki;
            char entry;
            bool cont = false;
            do {
                cki = Console.ReadKey(intercept);
                entry = cki.KeyChar;
                cont = BasicValidation.ValidateYorN(entry.ToString());
            } while (!cont);
            return entry;
        }

        public int GetNumbersFromConsole() {
            ConsoleKeyInfo cki;
            bool cont = false;
            string numbers = string.Empty;
            int rtnVal = -1;
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

            try {
                rtnVal = Convert.ToInt32(numbers);
            }
            catch (System.FormatException) {
                rtnVal = -5;
            }

            return rtnVal;
        }

        public string GetTextFromConsole(int minLength, bool allowEscape = false) {
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
    }
}

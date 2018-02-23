using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerInventory {
    public class BasicValidation {
        public static bool ValidateYorN(string entry) {
            bool result = false;
            if (entry.ToLower() == "y" || entry.ToLower() == "n") {
                result = true;
            }
            return result;
        }
    }
}

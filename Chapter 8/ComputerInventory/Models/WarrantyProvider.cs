using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComputerInventory.Models {
    public partial class WarrantyProvider {
        public WarrantyProvider() {
            MachineWarranty = new HashSet<MachineWarranty>();
        }

        public int WarrantyProviderId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "You must have bewteen 2 and 50 characters for the Provider Name")]
        public string ProviderName { get; set; }
        public int? SupportExtension { get; set; }

        [RegularExpression("^(?=(?:.{7}|.{10})$)[0-9]*$", ErrorMessage = "Must be 7 or 10 digits")]
        public string SupportNumber { get; set; }

        public ICollection<MachineWarranty> MachineWarranty { get; set; }
    }
}

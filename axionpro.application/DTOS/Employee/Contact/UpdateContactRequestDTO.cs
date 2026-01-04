using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Contact
{
    public class UpdateContactRequestDTO
    {

        [Required]
        public string UserEmployeeId { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public long Id { get; set; }
        public string? ContactName { get; set; }
        // 🔹 Contact Info
        public int? Relation { get; set; }        // e.g. Father, Mother, Spouse
        public int? ContactType { get; set; }          // e.g. Personal, Work, Emergency
        public string ContactNumber { get; set; } = string.Empty;
        public string? AlternateNumber { get; set; }
        public string? Email { get; set; }
        public bool? IsPrimary { get; set; }

        // 🔹 Address Info
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public string? HouseNo { get; set; }
        public string? LandMark { get; set; }
        public string? Street { get; set; }
        public string? Address { get; set; } 
        public string? Description { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

    }

}

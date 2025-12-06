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
        public string? UserEmployeeId { get; set; }
        public long _UserEmployeeId { get; set; }
        [Required]

        public string Id { get; set; } = string.Empty;
        public long Id_long { get; set; }

        // 🔹 Contact Info
        public string? ContactType { get; set; }          // e.g. Personal, Work, Emergency
        public string ContactNumber { get; set; } = string.Empty;
        public string? AlternateNumber { get; set; }
        public string? Email { get; set; }
        public bool? IsPrimary { get; set; }

        // 🔹 Address Info
        public string? CountryId { get; set; }
        public string? StateId { get; set; }
        public string? DistrictId { get; set; }
        public string? HouseNo { get; set; }
        public string? LandMark { get; set; }
        public string? Street { get; set; }
        public string? Address { get; set; }

 
        public string? Description { get; set; }


    }

}

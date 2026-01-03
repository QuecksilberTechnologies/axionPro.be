using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static axionpro.application.Constants.ConstantValues;

namespace axionpro.application.DTOS.Employee.Contact
{
    public class CreateContactRequestDTO
    {

        public string UserEmployeeId { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;

        // 🔹 Contact Info
        public string? ContactName { get; set; }          // e.g. Personal, Work, Emergency
        public ContactTypeEnum ContactType { get; set; }
        // e.g. Personal, Work, Emergency
        public string ContactNumber { get; set; } = string.Empty;
        public int? Relation { get; set; }
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
       
         public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

        // 🔹 Optional/Metadata
        public string? Remark { get; set; }
        public string? Description { get; set; }

 
         
    }
}

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Sensitive
{
    public class GetIdentityRequestDTO:BaseRequest
    {
       
        public string? EmployeeId { get; set; }
        public bool IsActive { get; set; }       
        public string? BloodGroup { get; set; }
        public string? MaritalStatus { get; set; }
        public bool? HasEPFAccount { get; set; }   
        public string? Nationality { get; set; }
        public string? EmergencyContactName { get; set; }
        // ✅ For Aadhaar / PAN / Passport Upload filters
        public bool? HasAadhaarIdUploaded { get; set; }
        public bool? HasPanIdUploaded { get; set; }
        public bool? HasPassportIdUploaded { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }
}

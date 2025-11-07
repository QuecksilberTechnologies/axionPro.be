using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Contact
{
   
        public class GetContactResponseDTO: BaseRequest
    {
          
            public string? EmployeeId { get; set; }          // e.g. "Mobile", "Office", "Home"
            public int ContactType { get; set; }          // e.g. "Mobile", "Office", "Home"
            public  string? ContactLocation{ get; set; }
            public string? ContactNumber { get; set; }  =  string.Empty;     // Primary number
            public string AlternateNumber { get; set; } = string.Empty;      // Optional alternate number
            public  string Email { get; set; } = string.Empty;               // Email ID
            public bool? IsPrimary { get; set; }              // Is this the primary contact?
            public int? CountryId { get; set; }               // Country
            public string? Country { get; set; }               // Country
            public int? StateId { get; set; }                // State
            public string? State { get; set; }                // State
            public int? DistrictId { get; set; }             // District
            public string? District { get; set; }             // District
            public string? HouseNo { get; set; }              // Address details
            public string? LandMark { get; set; }
            public string? Street { get; set; }
            public string? LocalAddress { get; set; }
            public string? PermanentAddress { get; set; }
            public string? Remark { get; set; }
            public bool? IsActive { get; set; }               // Active/Inactive
           
            public string? AddedById { get; set; }
            public DateTime? AddedDateTime { get; set; }
            public string? UpdatedById { get; set; }
            public DateTime? UpdatedDateTime { get; set; }  
         
            public bool? IsEditAllowed { get; set; }         // can edit contact?
            public bool? IsInfoVerified { get; set; }        // verified or not
            public string? InfoVerifiedById { get; set; }
            public DateTime? InfoVerifiedDateTime { get; set; }
            public string? Description { get; set; }          // extra notes / description
        }
    

}

 
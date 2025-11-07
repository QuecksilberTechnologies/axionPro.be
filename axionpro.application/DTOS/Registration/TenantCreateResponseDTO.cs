using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Registration
{
    public class TenantCreateResponseDTO
    {
        

            public bool Success { get; set; }  // Operation success status (true/false)
            public long? EmployeeId { get; set; }  // CandidateId if success, else null
            public long? TenantId { get; set; }  // CandidateId if success, else null
            public long? TenantProfileId { get; set; }  // CandidateId if success, else null
            public int? RoleId { get; set; }  // CandidateId if success, else null
            public bool? EmailSent { get; set; }  // CandidateId if success, else null
            public string LoginId { get; set; }
            public string Password { get; set; }
            public string Message { get; set; }  // Success/Failure message
                                                 //   public CandidateInfoDTO CandidateInfoDTO { get; set; }  // Employee Information

        }
     
}

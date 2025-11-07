 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Registration
{
    public class CandidateResponseDTO
    {

        public bool Success { get; set; }  // Operation success status (true/false)
        public long? CandidateId { get; set; }  // CandidateId if success, else null
        public long? TenantId { get; set; }  // CandidateId if success, else null
        public string Message { get; set; }  // Success/Failure message
                                             //   public CandidateInfoDTO CandidateInfoDTO { get; set; }  // Employee Information

    }
}

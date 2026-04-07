using axionpro.application.DTOS.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.Employee.EnrolledPolicy
{
    public  class GetEmployeeEnrolledResponseDTO
    {

         
            public long Id { get; set; }
            public required string EmployeeId { get; set; }   // 🔐 Encoded ID
            public required int PolicyTypeId { get; set; }
            public required int InsurancePolicyId { get; set; }

            // 🔹 DEPENDENT FLAG
            public bool HasDependent { get; set; } = false;

            // 🔹 DATES
            public required DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }            

            // 🔥 DEPENDENT LIST (IMPORTANT)
            public List<GetEmployeeDependentResponsePolicyDTO>? Dependents { get; set; } = new();

            // 🔹 COMMON PROPS (CLEAN ARCHITECTURE)
          
        }
    public class GetEmployeeDependentResponsePolicyDTO
    {
        public long Id { get; set; }   
        public   long DependentId { get; set; }  

        public int Relation { get; set; }  // ENUM VALUE

        public bool IsCovered { get; set; } = true;
    }

}

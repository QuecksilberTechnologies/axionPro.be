using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class GetBaseEmployeeResponseDTO: BaseResponse
    {
                  
            public long EmployeeDocumentId { get; set; }
            public string EmployementCode { get; set; } = null!;
            public string LastName { get; set; } = string.Empty;
            public string? MiddleName { get; set; }
            public string FirstName { get; set; } = string.Empty;
             public int GenderId { get; set; }

            public DateTime? DateOfBirth { get; set; }
            public DateTime? DateOfOnBoarding { get; set; }
            public DateTime? DateOfExit { get; set; }

            public long? DesignationId { get; set; }
            public long? EmployeeTypeId { get; set; }
            public long? DepartmentId { get; set; }

            public string? OfficialEmail { get; set; }
            public bool HasPermanent { get; set; } = false;
            public bool IsActive { get; set; } = true;

            public long? FunctionalId { get; set; }
            public long? ReferalId { get; set; }

            public string? Remark { get; set; }
            public bool? IsEditAllowed { get; set; } = true;
            public bool? IsInfoVerified { get; set; } = false;
        }
    



}
 

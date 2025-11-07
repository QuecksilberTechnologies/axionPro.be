using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Experience
{
    public class GetExperienceRequestDTO: BaseRequest
    {
        public string? Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? CompanyName { get; set; }             
        public bool? IsExperienceVerified { get; set; }     
        public int? ExperienceTypeId { get; set; }         
        public bool? IsExperienceVerifiedByMail { get; set; }
        public bool? IsExperienceVerifiedByCall { get; set; }         
        public bool? IsEditAllowed { get; set; }
        public bool? IsActive { get; set; }
    }



}

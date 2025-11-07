using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.EmployeeType
{
    public class GetEmployeeTypeResponseDTO: BaseRequest
    {

        public int Id { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }
 



    }

}

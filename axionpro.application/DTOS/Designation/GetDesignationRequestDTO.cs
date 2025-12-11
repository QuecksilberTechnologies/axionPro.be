using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Designation
{
    public class GetDesignationRequestDTO : BaseRequest
    {
             
        public int DepartmentId { get; set; }     
        public string? DesignationName { get; set; }
        public bool? IsActive { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();


    }
}

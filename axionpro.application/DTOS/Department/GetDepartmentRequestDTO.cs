using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Department
{
    /// <summary>
    /// post-request to fetch all department 
    /// </summary>

   

    public class GetDepartmentRequestDTO : BaseRequest
    {
       
        public string? DepartmentName { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public int? Id { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

    }
}

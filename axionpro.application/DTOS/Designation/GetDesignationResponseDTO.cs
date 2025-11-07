using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Designation
{
    public class GetDesignationResponseDTO : BaseRequest
    { 
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } 
        public DateTime AddedDateTime { get; set; }
        public string AddedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string? UpdatedById { get; set; }



    }
}

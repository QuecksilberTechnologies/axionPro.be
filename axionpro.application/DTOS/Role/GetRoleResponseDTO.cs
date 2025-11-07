using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{
    public class GetRoleResponseDTO : BaseResponse
    {
         
        public string RoleName { get; set; } = string.Empty; // Default value        
        public int? RoleType { get; set; }            
        public string? Remark { get; set; } // Nullable
        public bool IsActive { get; set; } = false; // Default false
        public string? AddedById { get; set; }
        public DateTime AddedByDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }






    }

}

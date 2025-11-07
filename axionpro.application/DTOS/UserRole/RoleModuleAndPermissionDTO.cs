using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.UserRole
{
    public class RoleModuleAndPermissionDTO
    {
        public int Id { get; set; }
        public int ProjectChildModuleDetailId { get; set; }  // Foreign key to ProjectChildModuleDetail
        public int RoleId { get; set; }  // Foreign key to Role
        public int OperationId { get; set; }  // Foreign key to Operation
        public bool HasAccess { get; set; }
        public bool IsActive { get; set; }
        public string Remark { get; set; }
        public string ImageIcon { get; set; }
        public int AddedById { get; set; }
        public DateTime AddedDateTime { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }

}

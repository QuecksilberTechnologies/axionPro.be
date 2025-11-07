using axionpro.application.DTOS.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Operation
{
    [Keyless]
    public class GetCheckOperationPermissionRequestDTO: BaseRequest
    {
        public string? RoleIds { get; set; }
       
        public int OperationId { get; set; }
        public bool IsActive { get; set; }
        public bool HasAccess { get; set; }

    }

}

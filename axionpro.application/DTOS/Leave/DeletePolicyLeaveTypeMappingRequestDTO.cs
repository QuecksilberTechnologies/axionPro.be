using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave
{
    public class DeletePolicyLeaveTypeMappingRequestDTO:BaseRequest
    {
       
        public long UserId { get; set; }
    }
}

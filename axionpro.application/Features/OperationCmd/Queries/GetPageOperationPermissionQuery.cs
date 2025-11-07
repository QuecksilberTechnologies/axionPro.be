using axionpro.application.DTOs.Operation;
using axionpro.application.DTOs.Role;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.OperationCmd.Queries
{
    public class GetPageOperationPermissionQuery : IRequest<ApiResponse<GetHasAccessOperationDTO>>
    {
        public GetCheckOperationPermissionRequestDTO? CheckOperationPermissionRequest { get; set; }
        
        public GetPageOperationPermissionQuery(GetCheckOperationPermissionRequestDTO checkOperationPermissionRequest)
        {
            this.CheckOperationPermissionRequest = checkOperationPermissionRequest;
        }
    }

}

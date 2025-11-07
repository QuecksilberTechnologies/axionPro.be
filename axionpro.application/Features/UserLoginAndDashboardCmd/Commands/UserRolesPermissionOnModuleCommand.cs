using axionpro.application.DTOs.BasicAndRoleBaseMenu;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Commands
{
    public class UserRolesPermissionOnModuleCommand : IRequest<ApiResponse<IEnumerable<UserRolesPermissionOnModuleDTO>>>
    {
        //till completed
        public AccessDetailRequestDTO AccessDetailDTO { get; set; }
        public UserRolesPermissionOnModuleCommand(AccessDetailRequestDTO accessRequestDTO)
        {
            AccessDetailDTO = accessRequestDTO;
        }

    
    }
}

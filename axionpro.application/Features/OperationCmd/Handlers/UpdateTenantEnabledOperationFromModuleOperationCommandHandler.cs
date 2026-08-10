// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates tenant-enabled operations from module operations.
// ================================================================

using axionpro.application.DTOs.Operation;
using axionpro.application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.OperationCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to update tenant-enabled operations from module operations.
    /// </summary>
    public class UpdateTenantEnabledOperationFromModuleOperationCommand : IRequest<ApiResponse<UpdateTenantEnabledOperationFromModuleOperationResponseDTO>>
    {
        public UpdateTenantEnabledOperationFromModuleOperationRequestDTO dto { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateTenantEnabledOperationFromModuleOperationCommand"/> class.
        /// </summary>
        /// <param name="dto">The tenant-enabled operation data.</param>
        public UpdateTenantEnabledOperationFromModuleOperationCommand(UpdateTenantEnabledOperationFromModuleOperationRequestDTO dto)
        {
            this.dto = dto;
        }
    }

    #endregion
}

namespace axionpro.application.Features.OperationCmd.Handlers
{
    #region Handler

    public class UpdateTenantEnabledOperationFromModuleOperationCommandHandler
    {
    }

    #endregion
}

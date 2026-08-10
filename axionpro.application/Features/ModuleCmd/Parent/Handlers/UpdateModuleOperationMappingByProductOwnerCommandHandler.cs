// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates module operation mapping by a product owner.
// ================================================================

using axionpro.application.DTOs.Module;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to update module operation mapping by a product owner.
    /// </summary>
    public class UpdateModuleOperationMappingByProductOwnerCommand : IRequest<ApiResponse<ModuleOperationMappingByProductOwnerResponseDTO>>
    {
        public UpdateModuleOperationMappingByProductOwnerRequestDTO dto { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateModuleOperationMappingByProductOwnerCommand"/> class.
        /// </summary>
        /// <param name="dto">The module operation mapping data.</param>
        public UpdateModuleOperationMappingByProductOwnerCommand(UpdateModuleOperationMappingByProductOwnerRequestDTO dto)
        {
            this.dto = dto;
        }
    }

    #endregion
}

// Handler implementation is intentionally pending.
// Request was relocated here as part of CQRS structural consolidation.

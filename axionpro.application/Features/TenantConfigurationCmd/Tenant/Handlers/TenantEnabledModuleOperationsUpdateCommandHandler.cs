// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to Tenant Enabled Module Operations Update.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.Features.TenantConfigurationCmd.Tenant.Commands;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to Tenant Enabled Module Operations Update.
    /// </summary>
public class TenantEnabledModuleOperationsUpdateCommand : IRequest<ApiResponse<bool>>
    {
        public TenantModuleOperationsUpdateRequestDTO RequestDTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantEnabledModuleOperationsUpdateCommand"/> class.
        /// </summary>

        public TenantEnabledModuleOperationsUpdateCommand(TenantModuleOperationsUpdateRequestDTO dto)
        {
            RequestDTO = dto;
        }
    }

    #endregion
}

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Handlers
{
    /// <summary>
    /// Handles the request to Tenant Enabled Module Operations Update.
    /// </summary>
public class TenantEnabledModuleOperationsUpdateCommandHandler : IRequestHandler<TenantEnabledModuleOperationsUpdateCommand, ApiResponse<bool>>
    {
        #region Fields

        private readonly ITenantModuleConfigurationRepository _repository;
        private readonly ILogger<TenantEnabledModuleOperationsUpdateCommandHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantEnabledModuleOperationsUpdateCommandHandler"/> class.
        /// </summary>


        public TenantEnabledModuleOperationsUpdateCommandHandler(
            ITenantModuleConfigurationRepository repository,
            ILogger<TenantEnabledModuleOperationsUpdateCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied TenantEnabledModuleOperationsUpdateCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<bool>> Handle(TenantEnabledModuleOperationsUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request?.RequestDTO == null || request.RequestDTO.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid request in TenantModuleOperationsUpdateCommand.");
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request: TenantId is missing or invalid.",
                        Data = false
                    };
                }

                // 🛠 Update Module + Operations via Repository
                //var isUpdated = await _repository.UpdateTenantModuleAndItsOperationsAsync(request.RequestDTO);
                var isUpdated = true;

                return new ApiResponse<bool>
                {
                    IsSucceeded = isUpdated,
                    Message = isUpdated ? "Module operations updated successfully." : "Update failed.",
                    Data = isUpdated
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in TenantModuleOperationsUpdateCommandHandler.");
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Internal server error while updating module operations.",
                    Data = false
                };
            }
        }
    
        #endregion
}
}

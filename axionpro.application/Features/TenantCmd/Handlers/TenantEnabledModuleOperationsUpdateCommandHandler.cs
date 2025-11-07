using axionpro.application.Features.TenantCmd.Commands;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TenantCmd.Handlers
{
    public class TenantEnabledModuleOperationsUpdateCommandHandler : IRequestHandler<TenantEnabledModuleOperationsUpdateCommand, ApiResponse<bool>>
    {
        private readonly ITenantModuleConfigurationRepository _repository;
        private readonly ILogger<TenantEnabledModuleOperationsUpdateCommandHandler> _logger;

        public TenantEnabledModuleOperationsUpdateCommandHandler(
            ITenantModuleConfigurationRepository repository,
            ILogger<TenantEnabledModuleOperationsUpdateCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

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
                var isUpdated = await _repository.UpdateTenantModuleAndItsOperationsAsync(request.RequestDTO);

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
    }
}

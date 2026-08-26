// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves filtered Parent/Header Module trees for authenticated Host Super Admins.
// ================================================================

using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using MediatR;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Command

    /// <summary>
    /// Represents the read-only request to retrieve Parent/Header Module trees.
    /// </summary>
    public sealed class GetModuleHeadersCommand : IRequest<ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {
        /// <summary>
        /// Gets the module-header filters.
        /// </summary>
        public GetParentModuleFilterRequestDTO? DTO { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetModuleHeadersCommand"/> class.
        /// </summary>
        /// <param name="dto">The module-header filters.</param>
        public GetModuleHeadersCommand(GetParentModuleFilterRequestDTO? dto)
        {
            DTO = dto;
        }

    }

    #endregion
}

namespace axionpro.application.Features.ModuleCmd.Parent.Handlers
{
    /// <summary>
    /// Retrieves filtered Parent/Header Module trees for an authenticated Host Super Admin.
    /// </summary>
    public class GetModuleHeadersCommandHandler : IRequestHandler<GetModuleHeadersCommand, ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<GetModuleHeadersCommandHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetModuleHeadersCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides module persistence operations.</param>
        /// <param name="commonRequestService">Validates the current Host Super Admin request.</param>
        /// <param name="logger">Writes handler diagnostics.</param>
        public GetModuleHeadersCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<GetModuleHeadersCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Retrieves module headers after validating the current Host Super Admin and the requested module scope.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<List<GetModuleChildInversResponseDTO>>> Handle(GetModuleHeadersCommand request, CancellationToken cancellationToken)
        {
            await _commonRequestService.ValidateHostSuperAdminRequestAsync();

            if (request?.DTO is null || !IsSupportedModuleScope(request.DTO.ModuleScope))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            try
            {
                var moduleHeaders = await _unitOfWork.ModuleRepository.GetAllOnlyModuleTreeAsync(
                    request.DTO.ModuleScope,
                    request.DTO.IsActive,
                    cancellationToken);

                return ApiResponse<List<GetModuleChildInversResponseDTO>>.Success(
                    moduleHeaders,
                    "Module headers retrieved successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to retrieve module headers in ModuleScope {ModuleScope}.", request.DTO.ModuleScope);
                throw;
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Determines whether the requested scope is supported by Parent Module administration.
        /// </summary>
        private static bool IsSupportedModuleScope(short moduleScope) =>
            moduleScope == AppConstants.TenantModuleScope ||
            moduleScope == AppConstants.HostModuleScope;

        #endregion
    }
}

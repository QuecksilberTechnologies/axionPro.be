// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles GetContactInfoQueryHandler requests using authenticated tenant context.
// ================================================================

using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;


namespace axionpro.application.Features.EmployeeCmd.Contact.Handlers
{

    #region Query

    /// <summary>
    /// Represents the GetContactInfoQuery application component.
    /// </summary>
    public class GetContactInfoQuery : IRequest<ApiResponse<List<GetContactResponseDTO>>>
    {
        public GetContactRequestDTO DTO { get; set; }

        public GetContactInfoQuery(GetContactRequestDTO dto)
        {
            DTO = dto;
        }

    }
    /// <summary>
    /// Handles authenticated tenant requests for this feature.
    /// </summary>
        #endregion

    #region Handler

public class GetContactInfoQueryHandler
    : IRequestHandler<GetContactInfoQuery, ApiResponse<List<GetContactResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetContactInfoQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public GetContactInfoQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetContactInfoQueryHandler> logger,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }


        public async Task<ApiResponse<List<GetContactResponseDTO>>> Handle(
        GetContactInfoQuery request,
        CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetContactInfo started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                #region Tenant Request Validation
                var validation =
                    await _commonRequestService.ValidateTenantUserRequestAsync();
                #endregion

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                    request.DTO.EmployeeId,
                    validation.Claims.TenantEncriptionKey,
                    _idEncoderService);

                if (employeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");
                // 4️⃣ FETCH DATA
                // ===============================
                var result =
                    await _unitOfWork.EmployeeContactRepository
                        .GetInfo(employeeId, request.DTO);

                // ===============================
                // 5️⃣ OPTIMIZED EMPTY HANDLING
                // ===============================
                var items = result?.Data ?? new List<GetContactResponseDTO>();

                var responseDTO = items.Any()
                    ? items // already DTO hai → projection ki zarurat nahi
                    : new List<GetContactResponseDTO>();

                _logger.LogInformation("GetContactInfo success");

                // ===============================
                // 6️⃣ SINGLE RESPONSE
                // ===============================
                return ApiResponse<List<GetContactResponseDTO>>
                    .SuccessPaginatedPercentage(
                        Data: responseDTO,
                        Message: items.Any()
                            ? "Contact info retrieved successfully."
                            : "No contact info found.",
                        PageNumber: result?.PageNumber ?? 1,
                        PageSize: result?.PageSize ?? 0,
                        TotalRecords: result?.TotalCount ?? 0,
                        TotalPages: result?.TotalPages ?? 0,
                        CompletionPercentage: result?.CompletionPercentage ?? 0,
                        HasUploadedAll: result?.HasUploadedAll ?? false
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching contact info | EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                throw; //  MUST
            }
        }
    }


    #endregion
}

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.IdentitiesInfo.Handlers
{
    #region Query Definition
    public class GetIdentityInfoQuery
        : IRequest<ApiResponse<List<GetEmployeeIdentityResponseDTO>>>
    {
        public GetIdentityRequestDTO DTO { get; }

        public GetIdentityInfoQuery(GetIdentityRequestDTO dto)
        {
            DTO = dto ?? throw new ArgumentNullException(nameof(dto));
        }
    }

    #endregion

    #region Query Handler

    public class GetIdentityInfoQueryHandler
 : IRequestHandler<GetIdentityInfoQuery, ApiResponse<List<GetEmployeeIdentityResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetIdentityInfoQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public GetIdentityInfoQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetIdentityInfoQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<List<GetEmployeeIdentityResponseDTO>>> Handle(
            GetIdentityInfoQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate Request
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetEmployeeIdentityResponseDTO>>
                        .Fail(validation.ErrorMessage);

                // 2️⃣ Decode EmployeeId
                var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                    request.DTO.EmployeeId,
                    validation.Claims.TenantEncriptionKey,
                    _idEncoderService);

                // 3️⃣ Permission check (optional strict)
                var permissions = await _permissionService
                    .GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("PersonalInfo"))
                {
                    // optional block
                }

                // 4️⃣ Fetch SP data
                var spRecords = await _unitOfWork.StoreProcedureRepository
                    .GetIdentityRecordAsync(
                        employeeId,
                        request.DTO.CountryNationalityId,
                        request.DTO.IsActive);

                if (spRecords == null || !spRecords.Any())
                    return ApiResponse<List<GetEmployeeIdentityResponseDTO>>
                        .Fail("No identity information found.");

                // 5️⃣ Projection + Encoding
                var response = ProjectionHelper.ToGetIdentityResponseDTO(
                    spRecords,
                    _idEncoderService,
                    validation.Claims.TenantEncriptionKey);

                return ApiResponse<List<GetEmployeeIdentityResponseDTO>>
                    .Success(response, "Identity information retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching identity info");

                return ApiResponse<List<GetEmployeeIdentityResponseDTO>>
                    .Fail(
                        "An unexpected error occurred.",
                        new List<string> { ex.Message });
            }
        }
    }


    #endregion
}

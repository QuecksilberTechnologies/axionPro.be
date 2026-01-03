using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Pagination;
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


namespace axionpro.application.Features.EmployeeCmd.Contact.Handlers
{
    public class GetContactInfoQuery : IRequest<ApiResponse<List<GetContactResponseDTO>>>
    {
        public GetContactRequestDTO DTO { get; set; }

        public GetContactInfoQuery(GetContactRequestDTO dto)
        {
            DTO = dto;
        }

    }
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
                await _unitOfWork.BeginTransactionAsync();

                // 🔐 STEP 1: COMMON VALIDATION (SAME AS CREATE CONTACT)
                var validation =
                    await _commonRequestService.ValidateRequestAsync(
                        request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetContactResponseDTO>>
                        .Fail(validation.ErrorMessage);

                // 🔓 STEP 2: Assign decoded common props
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // 🔓 STEP 3: Decode EmployeeId (target employee)
                request.DTO.Prop.EmployeeId =  RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

               

                // 🔑 STEP 5: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(
                        validation.RoleId);

                if (!permissions.Contains("ViewContactInfo"))
                {
                    // optional hard-stop
                    // return ApiResponse<List<GetContactResponseDTO>>
                    //     .Fail("You do not have permission to view contact info.");
                }

                // 📦 STEP 6: Repository call
                var result =
                    await _unitOfWork.EmployeeContactRepository
                        .GetInfo(request.DTO);

                if (result == null || !result.Items.Any())
                    return ApiResponse<List<GetContactResponseDTO>>
                        .Fail("No Contact info found.");

                // 🔐 STEP 7: Projection + Encryption
                var responseDTO =
                    ProjectionHelper.ToGetContactResponseDTOs(
                        result,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey);

                await _unitOfWork.CommitTransactionAsync();

                // 📤 STEP 8: Response
                return ApiResponse<List<GetContactResponseDTO>>
                    .SuccessPaginatedPercentage(
                        Data: responseDTO,
                        Message: "Contact info retrieved successfully.",
                        PageNumber: result.PageNumber,
                        PageSize: result.PageSize,
                        TotalRecords: result.TotalCount,
                        TotalPages: result.TotalPages,
                        CompletionPercentage: 80,   // TODO: calculate if needed
                        HasUploadedAll: null
                    );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error occurred while fetching Contact info for EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                return ApiResponse<List<GetContactResponseDTO>>
                    .Fail("Failed to fetch Contact info.",
                          new List<string> { ex.Message });
            }
        }
    }


}

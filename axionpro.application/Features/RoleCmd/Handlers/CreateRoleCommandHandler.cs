using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Role;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    public class CreateRoleCommand : IRequest<ApiResponse<List<GetRoleResponseDTO>>>
    {
        public CreateRoleRequestDTO DTO { get; set; }

        public CreateRoleCommand(CreateRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponse<List<GetRoleResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateRoleCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly  IIdEncoderService _idEncoderService;
        ICommonRequestService _commonRequestService;



        public CreateRoleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateRoleCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
             IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }


        public async Task<ApiResponse<List<GetRoleResponseDTO>>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;   
               

                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("AddBankInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }

                

                // ✅ Validate Role Name
                string? roleName = request.DTO.RoleName?.Trim();
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return new ApiResponse<List<GetRoleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Role name should not be empty or whitespace.",
                        Data = null
                    };
                }

                // 🧩 STEP 5: Repository call
                var responseDTO = await _unitOfWork.RoleRepository.CreateAsync(request.DTO);


                // 🧩 STEP 6: Validate response
                if (responseDTO == null || responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    _logger.LogWarning("Role creation failed or no role returned. TenantId: {TenantId}", request.DTO.Prop.UserEmployeeId);

                    return new ApiResponse<List<GetRoleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No role was created. Please check input details or try again.",
                        Data = null
                    };
                }
             //   var encryptedList = ProjectionHelper.ToGetRoleResponseDTOs(responseDTO.Items, _encryptionService, tenantKey);


                // 🧩 STEP 7: Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // ✅ Return success response
                return new ApiResponse<List<GetRoleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.PageSize} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = responseDTO.Items
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role(s) for TenantId");

                return new ApiResponse<List<GetRoleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to create role(s) due to an internal error.",
                    Data = null
                };
            }
        }
    }
}

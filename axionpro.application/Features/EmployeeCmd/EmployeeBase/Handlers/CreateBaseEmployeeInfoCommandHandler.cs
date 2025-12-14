using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class CreateBaseEmployeeInfoCommand : IRequest<ApiResponse<List<GetBaseEmployeeResponseDTO>>>
    {
        public CreateBaseEmployeeRequestDTO DTO { get; set; }

        public CreateBaseEmployeeInfoCommand(CreateBaseEmployeeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateBaseEmployeeInfoCommandHandler : IRequestHandler<CreateBaseEmployeeInfoCommand, ApiResponse<List<GetBaseEmployeeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateBaseEmployeeInfoCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;


        public CreateBaseEmployeeInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateBaseEmployeeInfoCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, 
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService ;
            _idEncoderService = idEncoderService ;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetBaseEmployeeResponseDTO>>> Handle(CreateBaseEmployeeInfoCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {

                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail(validation.ErrorMessage);

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

                // 🧩 STEP 4: Call Repository to get data

                // 3️⃣ DTO Configuration
                var entity = _mapper.Map<Employee>(request.DTO);
                // 🧩 STEP 5: Entity Mapping (join fields + base fields)

                entity.AddedById = request.DTO.Prop.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsInfoVerified = false;
                entity.IsEditAllowed = true;
                entity.InfoVerifiedById = null;
                entity.TenantId = request.DTO.Prop.TenantId;
                entity.DesignationId = request.DTO.DesignationId;
                entity.DepartmentId = request.DTO.DepartmentId;
                entity.EmployeeTypeId = request.DTO.EmployeeTypeId;
                entity.GenderId = request.DTO.GenderId;
                var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
                var timePart = DateTime.UtcNow.ToString("HHmmss");
                var randomSuffix = Path.GetRandomFileName().Replace(".", "").Substring(0, 4).ToUpper();
                entity.EmployementCode = $"{request.DTO.Prop.TenantId}/{datePart}/{timePart}-{randomSuffix}";
              //   HttpRequestOptionsKey

                // 4️⃣ Repository Operation
                var responseDTO = await _unitOfWork.Employees.CreateAsync(entity);
                 
                // 5️⃣ Encrypt Result Data
                 var encryptedList = ProjectionHelper.ToGetBaseInfoResponseDTOs(responseDTO.Items, _idEncoderService, validation.Claims.TenantEncriptionKey);

                // 6️⃣ Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // 7️⃣ Final API Response
                return new ApiResponse<List<GetBaseEmployeeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = encryptedList,
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error while adding base employee info");
                return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Failed to add base employee info.", new List<string> { ex.Message });
            }
        }
    }
}

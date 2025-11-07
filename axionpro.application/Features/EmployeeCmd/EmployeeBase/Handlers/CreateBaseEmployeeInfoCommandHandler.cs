using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public CreateBaseEmployeeInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateBaseEmployeeInfoCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
        }

        public async Task<ApiResponse<List<GetBaseEmployeeResponseDTO>>> Handle(CreateBaseEmployeeInfoCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1️⃣ Token Validation
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = _config["Jwt:Key"];
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);
                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Invalid or expired token.");

                long empId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (empId < 1)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("User is not authorized.");
                }

                var tenantKey = tokenClaims.TenantEncriptionKey;
                var tenantEncryptedId = tokenClaims.TenantId;
                var employeeEncryptedId = tokenClaims.EmployeeId;

                if (string.IsNullOrEmpty(tenantKey) || string.IsNullOrEmpty(tenantEncryptedId))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Tenant information is missing in token.");
                }

                long tenantId = 0;
                long employeeId = 0;

                if (request.DTO.UserEmployeeId == employeeEncryptedId)
                {
                    tenantId = EncryptionHelper1.DecryptId(_encryptionService, tenantEncryptedId, tenantKey);
                    employeeId = EncryptionHelper1.DecryptId(_encryptionService, request.DTO.UserEmployeeId, tenantKey);
                }

                // 2️⃣ Permission Check
                var permissions = await _permissionService.GetPermissionsAsync(tokenClaims.RoleId);
                if (!permissions.Contains("AddBaseEmployeeInfo"))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("You do not have permission to add base employee info.");
                }

                // 3️⃣ DTO Configuration
                var entity = _mapper.Map<Employee>(request.DTO);
                entity.AddedById = employeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsInfoVerified = false;
                entity.IsEditAllowed = true;
                entity.TenantId = tenantId;
                

                // 4️⃣ Repository Operation
                var responseDTO = await _unitOfWork.Employees.CreateAsync(entity);

                // 5️⃣ Encrypt Result Data
                var encryptedList = ProjectionHelper.ToGetBaseInfoResponseDTOs(responseDTO.Items, _encryptionService, tenantKey);

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
                    Data = encryptedList
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

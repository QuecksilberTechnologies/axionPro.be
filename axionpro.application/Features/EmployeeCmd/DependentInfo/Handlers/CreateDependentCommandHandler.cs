using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Command;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers
{
    public class CreateDependentCommand : IRequest<ApiResponse<List<GetDependentResponseDTO>>>
    {
        public CreateDependentRequestDTO DTO { get; set; }

        public CreateDependentCommand(CreateDependentRequestDTO dto)
        {
            DTO = dto;
        }

    }
    public class CreateDependentCommandHandler : IRequestHandler<CreateDependentCommand, ApiResponse<List<GetDependentResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateDependentCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;

        public CreateDependentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateDependentCommandHandler> logger,
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

        public async Task<ApiResponse<List<GetDependentResponseDTO>>> Handle(CreateDependentCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1️⃣ Token & Tenant key validation
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetDependentResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = _config["Jwt:Key"];
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);
                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetDependentResponseDTO>>.Fail("Invalid or expired token.");

                long empId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (empId < 1)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetDependentResponseDTO>>.Fail("User is not authorized.");
                }

                var tenantKey = tokenClaims.TenantEncriptionKey;

                // 2️⃣ Permission check
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddDependentInfo"))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetDependentResponseDTO>>.Fail("You do not have permission to add dependent info.");
                }

              

                var dependentEntity = _mapper.Map<EmployeeDependent>(request.DTO);
                // 3️⃣ Prepare entity from DTO
                dependentEntity.AddedById = empId;
                dependentEntity.AddedDateTime = DateTime.UtcNow;
                dependentEntity.IsActive = true;
                dependentEntity.IsEditAllowed = true;
                dependentEntity.IsInfoVerified = false;

                PagedResponseDTO<GetDependentResponseDTO> responseDTO = await _unitOfWork.EmployeeDependentRepository.CreateAsync(dependentEntity);

                // 4️⃣ Encrypt Ids in result
                var encryptedList = ProjectionHelper.ToGetDependentResponseDTOs(responseDTO.Items, _encryptionService, tenantKey);

                // 5️⃣ Commit
                await _unitOfWork.CommitTransactionAsync();

                // 6️⃣ Return success response
                return new ApiResponse<List<GetDependentResponseDTO>>
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
                _logger.LogError(ex, "Error occurred while adding dependent info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
                return ApiResponse<List<GetDependentResponseDTO>>.Fail("Failed to add dependent info.", new List<string> { ex.Message });
            }
        }
    }

}

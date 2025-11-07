using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.Contact.Command;
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

namespace axionpro.application.Features.EmployeeCmd.Contact.Handlers
{
    public class CreateContactInfoCommandHandler : IRequestHandler<CreateContactInfoCommand, ApiResponse<List<GetContactResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateContactInfoCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;

        public CreateContactInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateContactInfoCommandHandler> logger,
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

        public async Task<ApiResponse<List<GetContactResponseDTO>>> Handle(CreateContactInfoCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Token & Tenant key
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = _config["Jwt:Key"];
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);
                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("Invalid or expired token.");

                long empId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (empId < 1)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("User is not authorized.");
                }

                var tenantKey = tokenClaims.TenantEncriptionKey;

                // 2. Permission check
                var permissions = await _permissionService.GetPermissionsAsync(tokenClaims.RoleId);
                if (!permissions.Contains("AddContactInfo"))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetContactResponseDTO>>.Fail("You do not have permission to add contact info.");
                }

                // 3. Prepare entity from DTO
                request.DTO.AddedById = empId;
                request.DTO.AddedDateTime = DateTime.UtcNow;
                request.DTO.IsActive = true;
                request.DTO.IsEditAllowed = true;
                request.DTO.IsInfoVerified = false;

                var contactEntity = _mapper.Map<EmployeeContact>(request.DTO);
                PagedResponseDTO<GetContactResponseDTO> responseDTO = await _unitOfWork.EmployeeContactRepository.CreateAsync(contactEntity);

                // 4. Encrypt Ids in result
                var encryptedList = ProjectionHelper.ToGetContactResponseDTOs(responseDTO.Items, _encryptionService, tenantKey);

                // 5. Commit
                await _unitOfWork.CommitTransactionAsync();

                // 6. Return
                return new ApiResponse<List<GetContactResponseDTO>>
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
                _logger.LogError(ex, "Error occurred while adding contact info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
                return ApiResponse<List<GetContactResponseDTO>>.Fail("Failed to add contact info.", new List<string> { ex.Message });
            }
        }
    }
}

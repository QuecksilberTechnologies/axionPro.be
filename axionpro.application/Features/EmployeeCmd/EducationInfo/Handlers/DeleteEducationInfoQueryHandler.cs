using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers;
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

namespace axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers
{
    public class DeleteEducationInfoQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteRequestDTO DTO { get; set; }

        public DeleteEducationInfoQuery(DeleteRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class DeleteEducationInfoQueryHandler : IRequestHandler<DeleteEducationInfoQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteEducationInfoQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;

        public DeleteEducationInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteEducationInfoQueryHandler> logger,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteEducationInfoQuery request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1️⃣ Token Validate
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<bool>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<bool>.Fail("Invalid or expired token.");

                // 2️⃣ Validate User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);

                if (loggedInEmpId <= 0)
                    return ApiResponse<bool>.Fail("Unauthorized or inactive user.");

                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;
                string sanitizedKey = EncryptionSanitizer.SuperSanitize(tenantKey);

                // Decode IDs
                long decryptedEmployeeId = _idEncoderService.DecodeId_long(
                    EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId), sanitizedKey);

                long decryptedRecordId = _idEncoderService.DecodeId_long(
                    EncryptionSanitizer.CleanEncodedInput(request.DTO.Id), sanitizedKey);

                if (decryptedEmployeeId != loggedInEmpId)
                    return ApiResponse<bool>.Fail("Unauthorized: Employee mismatch.");

                // 3️⃣ Permission Check
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("Education"))
                    return ApiResponse<bool>.Fail("Access denied: You do not have permission to delete education info.");

                // 4️⃣ Delete Record
                var isSuccess = await _unitOfWork.EmployeeEducationRepository.DeleteAsync(decryptedRecordId, decryptedEmployeeId);

                if (!isSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("No matching education record found.");
                }

                // 5️⃣ Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<bool>.Success(true, "Education record deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting education info.");
                return ApiResponse<bool>.Fail("Error deleting record.", new List<string> { ex.Message });
            }
        }
    
    
    }




}

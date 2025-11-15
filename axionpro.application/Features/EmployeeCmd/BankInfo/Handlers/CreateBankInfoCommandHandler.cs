using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Pagination;

using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.BankInfo.Handlers
{
    public class CreateBankInfoCommand : IRequest<ApiResponse<List<GetBankResponseDTO>>>
    {
        public CreateBankRequestDTO DTO { get; set; }

        public CreateBankInfoCommand(CreateBankRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class CreateBankInfoCommandHandler: IRequestHandler<CreateBankInfoCommand, ApiResponse<List<GetBankResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateBankInfoCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;

        public CreateBankInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateBankInfoCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService
            ,IFileStorageService fileStorageService
            )
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
            _fileStorageService = fileStorageService;

        }


        public async Task<ApiResponse<List<GetBankResponseDTO>>> Handle(CreateBankInfoCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            string? savedFullPath = null;  // 📂 File full path track karne ke liye


            try
            {

                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Decrypt Tenant and Employee
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("User invalid.");
                }

                 string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                //UserEmployeeId
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                long decryptedEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                //Token TenantId
                string tokenTenant = EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenTenant, finalKey);
                //Id              
                // Actual EmployeeId
                string actualEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.EmployeeId);
                long decryptedActualEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);

               // 🧩 STEP 4: Validate all employee references
                if (decryptedTenantId <= 0 )
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Tenant or employee information missing.");
                }


                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         decryptedEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }

                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data          

                // 🔹 STEP 4: File Upload
                string? docPath = null;
                string? docName = null;

                // 🔹 Tenant info from decoded values
                long tenantId = decryptedTenantId;
                bool HasChequeDocUploaded = false;
                if (string.IsNullOrWhiteSpace(request.DTO.BankName))
                {
                     return ApiResponse<List<GetBankResponseDTO>>.Fail("Bank name cannot be null.");
                }

                // ✅ check — sirf letters (A–Z, a–z) aur space allowed
                if (!Regex.IsMatch(request.DTO.BankName, @"^[a-zA-Z\s]+$"))
                {
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("Bank name cannot be null or sepcial character.");

                }

                string? docFileName = null;
             
                    // 🔹 File upload check
                    if (request.DTO.CancelledChequeFile != null && request.DTO.CancelledChequeFile.Length > 0)
                    {
                    docFileName = EncryptionSanitizer.CleanEncodedInput(request.DTO.BankName.Trim().Replace(" ", "").ToLower());
                    using (var ms = new MemoryStream())
                        {
                            await request.DTO.CancelledChequeFile.CopyToAsync(ms);
                            var fileBytes = ms.ToArray();

                            // 🔹 File naming convention (same pattern as asset)
                            string fileName = $"Cheque-{decryptedActualEmployeeId + "_" + docFileName}-{DateTime.UtcNow:yyMMddHHmmss}.png";
                            string fullFolderPath = _fileStorageService.GetEmployeeFolderPath(tenantId, decryptedActualEmployeeId, "bank");              

                            // 🔹 Store actual name for reference in DB
                            docName = fileName;

                            // 🔹 Save file physically
                            savedFullPath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, fullFolderPath);

                            // 🔹 If saved successfully, set relative path
                            if (!string.IsNullOrEmpty(savedFullPath))
                            {
                                docPath = _fileStorageService.GetRelativePath(savedFullPath);
                                 HasChequeDocUploaded = true;
                            }
                        }
                    }

                

                var bankEntity = _mapper.Map<EmployeeBankDetail>(request.DTO); // use mapper for create mapping
                bankEntity.AddedById = decryptedEmployeeId;
                bankEntity.AddedDateTime = DateTime.UtcNow;
                bankEntity.IsActive = true;
                bankEntity.IsEditAllowed = false;
                bankEntity.IsInfoVerified = false;
                bankEntity.IsPrimaryAccount = request.DTO.IsPrimaryAccount;
                bankEntity.EmployeeId = decryptedActualEmployeeId;
                bankEntity.FileType = 0;

                if (HasChequeDocUploaded)
                {
                    bankEntity.FileType = 1;//image
                    bankEntity.FilePath = docPath;
                    bankEntity.FileName= docName;
                    
                }
               bankEntity.HasChequeDocUploaded=HasChequeDocUploaded;


                    PagedResponseDTO<GetBankResponseDTO> responseDTO = await _unitOfWork.EmployeeBankRepository.CreateAsync(bankEntity);
                 
                // 4. Pre-map projection + encrypt Ids (fast)
                // If pagedResult.Items are entities:
                var encryptedList = ProjectionHelper.ToGetBankResponseDTOs(responseDTO, _idEncoderService, tenantKey);
               

                // 5. commit
                await _unitOfWork.CommitTransactionAsync();

                // 6. Return API response with pagination metadata preserved
                return new ApiResponse<List<GetBankResponseDTO>>
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
                _logger.LogError(ex, "Error occurred while adding bank info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
                return ApiResponse<List<GetBankResponseDTO>>.Fail("Failed to add bank info.", new List<string> { ex.Message });
            }
        }


    }
}

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Pagination;

using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
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
using System.Security.Claims;
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
        private readonly ICommonRequestService _commonRequestService;

        public CreateBankInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateBankInfoCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService
            ,IFileStorageService fileStorageService,
             ICommonRequestService commonRequestService)
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
            _commonRequestService = commonRequestService;

        }


        public async Task<ApiResponse<List<GetBankResponseDTO>>> Handle(CreateBankInfoCommand request, CancellationToken cancellationToken)
        {         


            try
            {
                await _unitOfWork.BeginTransactionAsync();
                string? savedFullPath = null;  // 📂 File full path track karne ke liye

                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetBankResponseDTO>>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId= RequestCommonHelper.DecodeOnlyEmployeeId(
                  request.DTO.EmployeeId,                 
                  validation.Claims.TenantEncriptionKey, _idEncoderService
              );

         
                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("AddBankInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data          

                // 🔹 STEP 4: File Upload
                string? docPath = null;
                string? docName = null;
 
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
                            string fileName = $"Cheque-{request.DTO.Prop.EmployeeId + "_" + docFileName}-{DateTime.UtcNow:yyMMddHHmmss}.png";
                          
                           string fullFolderPath = _fileStorageService.GetEmployeeFolderPath(request.DTO.Prop.TenantId, request.DTO.Prop.EmployeeId, "bank");              

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
                bankEntity.AddedById = request.DTO.Prop.UserEmployeeId;
                bankEntity.AddedDateTime = DateTime.UtcNow;
                bankEntity.IsActive = true;
                bankEntity.IsEditAllowed = true;
                bankEntity.IsInfoVerified = false;
                bankEntity.IsPrimaryAccount = request.DTO.IsPrimaryAccount;
                bankEntity.EmployeeId = request.DTO.Prop.EmployeeId;
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
                var encryptedList = ProjectionHelper.ToGetBankResponseDTOs(responseDTO, _idEncoderService, validation.Claims.TenantEncriptionKey, _config);
               

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

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace axionpro.application.Features.EmployeeCmd.BankInfo.Handlers
{
    public class UpdateBankCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateBankReqestDTO DTO { get; set; }

        public UpdateBankCommand(UpdateBankReqestDTO dto)
        {
            DTO = dto;
        }

    }
    public class UpdateBankCommandHandler : IRequestHandler<UpdateBankCommand, ApiResponse<bool>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBankCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncryptionService _encryptionService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IIdEncoderService _idEncoderService;
        public UpdateBankCommandHandler(
            IBaseEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateBankCommandHandler> logger,
            IMapper mapper,
            ITokenService tokenService,
            IPermissionService permissionRepository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IEncryptionService encryptionService ,  ICommonRequestService commonRequestService, IFileStorageService fileStorageService,
             IIdEncoderService idEncoderService
            )
        {

            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _tokenService = tokenService;
            _permissionService = permissionRepository;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _encryptionService = encryptionService;
            _commonRequestService = commonRequestService;
            _fileStorageService = fileStorageService;
            _idEncoderService = idEncoderService;

        }

        public async Task<ApiResponse<bool>> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // -------------------------------------------------
                // 1️⃣ COMMON VALIDATION
                // -------------------------------------------------
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                request.DTO.EmployeeId,
                validation.Claims.TenantEncriptionKey,
                _idEncoderService
            );

                // -------------------------------------------------
                // 2️⃣ PERMISSION CHECK
                // -------------------------------------------------
                var permissions = await _permissionService
                    .GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("UpdateBankInfo")){
                   // return ApiResponse<bool>.Fail("You do not have permission to update bank info.");
                   }

                // -------------------------------------------------
                // 3️⃣ FETCH EXISTING BANK RECORD
                // -------------------------------------------------
                var bank = await _unitOfWork.EmployeeBankRepository
                    .GetSingleRecordAsync(request.DTO.Id, true);

                if (bank == null)
                    return ApiResponse<bool>.Fail("Employee bank record not found.");

                var dto = request.DTO;

                // -------------------------------------------------
                // 4️⃣ PARTIAL FIELD UPDATES
                // -------------------------------------------------
                if (!string.IsNullOrWhiteSpace(dto.BankName))
                    bank.BankName = dto.BankName.Trim();

                if (!string.IsNullOrWhiteSpace(dto.AccountNumber))
                    bank.AccountNumber = dto.AccountNumber.Trim();

                if (!string.IsNullOrWhiteSpace(dto.IFSCCode))
                    bank.IFSCCode = dto.IFSCCode.Trim();

                if (!string.IsNullOrWhiteSpace(dto.BranchName))
                    bank.BranchName = dto.BranchName.Trim();

                if (!string.IsNullOrWhiteSpace(dto.AccountType))
                    bank.AccountType = dto.AccountType.Trim();

                if (!string.IsNullOrWhiteSpace(dto.UPIId))
                    bank.UPIId = dto.UPIId.Trim();

                // -------------------------------------------------
                // 5️⃣ PRIMARY ACCOUNT BUSINESS RULE
                // -------------------------------------------------
               if (dto.IsPrimaryAccount)
                 {
                //    // 🔴 Cancelled cheque mandatory
                //    if (dto.CancelledChequeFile == null && !bank.HasChequeDocUploaded)
                //        return ApiResponse<bool>.Fail("Cancelled cheque is mandatory for primary bank account.");

                //    //// 🔹 Reset all existing primaries
                //    //bool resetDone = await _unitOfWork.EmployeeBankRepository
                //    //    .ResetPrimaryAccountAsync(request.DTO.Prop.EmployeeId);

                //    //if (!resetDone)
                //    //    return ApiResponse<bool>.Fail("Failed to reset existing primary bank accounts.");

                     bank.IsPrimaryAccount = true;
                 }
                else { 
                     bank.IsPrimaryAccount = false;
                }

                // -------------------------------------------------
                // 6️⃣ FILE HANDLING (OPTIONAL)
                // -------------------------------------------------

                // ------------------------ FILE HANDLING (OPTIONAL) ------------------------
                if (request.DTO.CancelledChequeFile is { Length: > 0 })
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(bank.FileName))
                        {
                            string oldPath = bank.FilePath; // This may be URL or relative path

                            bool fileDeleted = false;

                            // Case 1: Physical local/server file path
                            string physicalFullPath = _fileStorageService.GetRelativePath(oldPath);

                            if (!string.IsNullOrWhiteSpace(physicalFullPath) && File.Exists(physicalFullPath))
                            {
                                File.Delete(physicalFullPath);
                                fileDeleted = true;
                                _logger.LogInformation("📌 Local/Server education document deleted: {File}", physicalFullPath);
                            }

                            // Case 2: Remote CDN/HTTP/Cloud File
                            if (!fileDeleted && Uri.TryCreate(oldPath, UriKind.Absolute, out Uri? uri)
                                && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
                            {
                                using var client = new HttpClient();
                                var response = await client.DeleteAsync(uri);

                                if (response.IsSuccessStatusCode)
                                {
                                    fileDeleted = true;
                                    _logger.LogInformation("🌍 Remote bank document deleted: {File}", uri);
                                }
                                else
                                {
                                    _logger.LogWarning("⚠️ Remote bank file delete attempt failed for: {File}", uri);
                                }
                            }

                            if (!fileDeleted)
                                _logger.LogWarning("⚠️ Delete attempted but file not found: {File}", oldPath);
                        }

                        // Now upload new file
                        using var ms = new MemoryStream();
                        await request.DTO.CancelledChequeFile.CopyToAsync(ms);

                        string newFileName = $"Cheque-{bank.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}.pdf";

                        string folderPath = _fileStorageService.GetEmployeeFolderPath(
                            request.DTO.Prop.TenantId,
                             request.DTO.Prop.EmployeeId,
                            "bank"
                        );

                        string savedPath = await _fileStorageService.SaveFileAsync(ms.ToArray(), newFileName, folderPath);

                        bank.FilePath = _fileStorageService.GetRelativePath(savedPath);
                        bank.FileName = newFileName;
                        bank.HasChequeDocUploaded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error replacing bank document for employee {Emp}", request.DTO.Prop.EmployeeId);
                        return ApiResponse<bool>.Fail("File upload failed, please try again.");
                    }

                }
 

                 var responseData = _mapper.Map<UpdateBankReqestDTO>(bank);
                   responseData.Prop.UserEmployeeId = request.DTO.Prop.UserEmployeeId;
                   responseData.Prop.EmployeeId = request.DTO.Prop.EmployeeId;
                
                //responseData.UpdatedDateTime = DateTime.UtcNow;

                // -------------------------------------------------
                // 7️⃣ SAVE CHANGES
                // -------------------------------------------------
                bool isSucess =   await _unitOfWork.EmployeeBankRepository.UpdateAsync(responseData);
                if (!isSucess)
                    return ApiResponse<bool>.Fail("Failed to update bank information.");
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<bool>.Success(true, "Bank information updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex,
                    "❌ Error updating bank info. BankId: {Id}",
                    request.DTO.Id);

                return ApiResponse<bool>.Fail(
                    "Unexpected error occurred while updating bank info.",
                    new List<string> { ex.Message }
                );
            }
        }

    }

}

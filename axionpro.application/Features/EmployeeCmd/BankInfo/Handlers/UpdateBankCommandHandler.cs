using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

        public async Task<ApiResponse<bool>> Handle(
      UpdateBankCommand request,
      CancellationToken cancellationToken)
        {
            string? uploadedFileKey = null;

            try
            {
                _logger.LogInformation("UpdateBank started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO?.UserEmployeeId);

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 3️⃣ PERMISSION CHECK
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to update bank info.");

                // ===============================
                // 4️⃣ FETCH EXISTING
                // ===============================
                var bank = await _unitOfWork.EmployeeBankRepository
                    .GetSingleRecordAsync(request.DTO.Id, true);

                if (bank == null)
                    throw new ApiException("Bank record not found.", 404);

                var dto = request.DTO;

                // ===============================
                // 5️⃣ UPDATE FIELDS
                // ===============================
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

                // ===============================
                // 6️⃣ PRIMARY ACCOUNT RULE
                // ===============================
                if (dto.IsPrimaryAccount)
                {
                    if (dto.CancelledChequeFile == null && !bank.HasChequeDocUploaded)
                        throw new ValidationErrorException("Cancelled cheque is mandatory.");

                    var resetDone =
                        await _unitOfWork.EmployeeBankRepository
                            .ResetPrimaryAccountAsync(
                                request.DTO.Prop.EmployeeId,
                                request.DTO.Prop.UserEmployeeId);

                    if (!resetDone)
                        throw new ApiException("Failed to reset primary accounts.", 500);

                    bank.IsPrimaryAccount = true;
                }
                else
                {
                    bank.IsPrimaryAccount = false;
                }

                // ===============================
                // 7️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 8️⃣ FILE UPLOAD
                // ===============================
                if (dto.CancelledChequeFile != null &&
                    dto.CancelledChequeFile.Length > 0)
                {
                    try
                    {
                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{request.DTO.Prop.TenantId}/" +
                            $"{ConstantValues.EmployeeFolder}/{request.DTO.Prop.EmployeeId}/" +
                            $"{ConstantValues.BankFolder}";

                        string newFileName =
                            $"{ConstantValues.BankFolder}-{request.DTO.Prop.EmployeeId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        uploadedFileKey = await _fileStorageService.UploadFileAsync(
                            dto.CancelledChequeFile,
                            folderPath,
                            newFileName);

                        bank.FilePath = uploadedFileKey;
                        bank.FileName = newFileName;
                        bank.HasChequeDocUploaded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Cheque upload failed");
                        throw new ApiException("File upload failed.", 500);
                    }
                }

                bank.UpdatedDateTime = DateTime.UtcNow;
                bank.UpdatedById = request.DTO.Prop.UserEmployeeId;

                var responseData = _mapper.Map<UpdateBankReqestDTO>(bank);
                responseData.Prop.UserEmployeeId = request.DTO.Prop.UserEmployeeId;
                responseData.Prop.EmployeeId = request.DTO.Prop.EmployeeId;
                bool isSuccess =
                    await _unitOfWork.EmployeeBankRepository.UpdateAsync(responseData);

                if (!isSuccess)
                    throw new ApiException("Failed to update bank info.", 500);

                // ===============================
                // 🔟 COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("UpdateBank success");

                return ApiResponse<bool>.Success(true, "Bank updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error updating bank info");

                // 🧹 FILE CLEANUP
                if (!string.IsNullOrEmpty(uploadedFileKey))
                {
                    try
                    {
                        await _fileStorageService.DeleteFileAsync(uploadedFileKey);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx, "File cleanup failed");
                    }
                }

                throw; // 🚨 MUST
            }
        }
    }

}

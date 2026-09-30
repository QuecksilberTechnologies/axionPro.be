// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles Create Bank Info Command Handler requests.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace axionpro.application.Features.EmployeeCmd.BankInfo.Handlers
{

    #region Command

    /// <summary>
    /// Represents the CreateBankInfoCommand application component.
    /// </summary>
    public class CreateBankInfoCommand : IRequest<ApiResponse<List<GetBankResponseDTO>>>
    {
        public CreateBankRequestDTO DTO { get; set; }

        public CreateBankInfoCommand(CreateBankRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    /// <summary>
    /// Handles CreateBankInfoCommand requests.
    /// </summary>
        #endregion

    #region Handler

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


        public async Task<ApiResponse<List<GetBankResponseDTO>>> Handle(
      CreateBankInfoCommand request,
      CancellationToken cancellationToken)
        {
            string? uploadedFileKey = null;

            try
            {
                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateTenantUserRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");
                var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (employeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");
                // 4️⃣ BUSINESS VALIDATION
                // ===============================
                if (string.IsNullOrWhiteSpace(request.DTO.BankName))
                    throw new ValidationErrorException("Bank name cannot be empty.");

                if (!Regex.IsMatch(request.DTO.BankName, @"^[a-zA-Z\s]+$"))
                    throw new ValidationErrorException("Invalid bank name format.");

                if (string.IsNullOrWhiteSpace(request.DTO.BranchName) ||
                    !Regex.IsMatch(request.DTO.BranchName, @"^[A-Za-z0-9 ,.-]{3,100}$"))
                    throw new ValidationErrorException("Branch name must contain 3 to 100 valid characters.");

                if (string.IsNullOrWhiteSpace(request.DTO.AccountNumber) ||
                    !Regex.IsMatch(request.DTO.AccountNumber, @"^\d{9,18}$"))
                    throw new ValidationErrorException("Account number must contain 9 to 18 digits.");

                if (string.IsNullOrWhiteSpace(request.DTO.IFSCCode) ||
                    !Regex.IsMatch(request.DTO.IFSCCode.Trim().ToUpperInvariant(), @"^[A-Z]{4}0[A-Z0-9]{6}$"))
                    throw new ValidationErrorException("IFSC code must use the format ABCD0XXXXXX.");

                if (!string.IsNullOrWhiteSpace(request.DTO.UPIId) &&
                    !Regex.IsMatch(request.DTO.UPIId, @"^[a-zA-Z0-9.\-_]{2,256}@[a-zA-Z]{2,64}$"))
                    throw new ValidationErrorException("UPI ID must use a valid name@provider format.");

                if (request.DTO.IsPrimaryAccount &&
                    (request.DTO.CancelledChequeFile == null || request.DTO.CancelledChequeFile.Length == 0))
                    throw new ValidationErrorException("Cancelled cheque is required for the primary account.");

                // ===============================
                // 5️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 6️⃣ FILE UPLOAD
                // ===============================
                string? docName = null;
                bool hasFile = false;

                if (request.DTO.CancelledChequeFile != null &&
                    request.DTO.CancelledChequeFile.Length > 0)
                {
                    string cleanName = EncryptionSanitizer
                        .CleanEncodedInput(request.DTO.BankName)
                        .Replace(" ", "")
                        .ToLower();

                    docName = $"Cheque-{cleanName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                    string folderPath =
                        $"{ConstantValues.TenantFolder}-{validation.TenantId}/" +
                        $"{ConstantValues.EmployeeFolder}/{employeeId}/" +
                        $"{ConstantValues.BankFolder}";

                    try
                    {
                        uploadedFileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.CancelledChequeFile,
                            folderPath,
                            docName);
                    }
                    catch (Exception uploadException)
                    {
                        _logger.LogError(uploadException, "Cancelled cheque upload failed");
                        throw new ApiException(
                            "Cancelled cheque upload failed. Check the file and storage configuration, then try again.",
                            500);
                    }

                    if (!string.IsNullOrEmpty(uploadedFileKey))
                        hasFile = true;
                }

                // ===============================
                // 7️⃣ MAP ENTITY
                // ===============================
                var bankEntity = _mapper.Map<EmployeeBankDetail>(request.DTO);

                bankEntity.EmployeeId = employeeId;
                bankEntity.AddedById = validation.LoggedInEmployeeId;
                bankEntity.AddedDateTime = DateTime.UtcNow;
                bankEntity.AccountType = AccountTypeHelper.Normalize(request.DTO.AccountType);
                bankEntity.IsActive = true;
                bankEntity.IsEditAllowed = true;
                bankEntity.IsInfoVerified = false;
                bankEntity.IsPrimaryAccount = request.DTO.IsPrimaryAccount;
                bankEntity.FileType = hasFile ? 1 : 0;
                bankEntity.FilePath = uploadedFileKey;
                bankEntity.FileName = docName;
                bankEntity.HasChequeDocUploaded = hasFile;

                // ===============================
                // 8️⃣ SAVE
                // ===============================
                var responseDTO =
                    await _unitOfWork.EmployeeBankRepository.CreateAsync(bankEntity);

                if (responseDTO == null)
                    throw new ApiException("Failed to add bank info.", 500);

                // ===============================
                // 9️⃣ RESPONSE BUILD
                // ===============================
                var encryptedList = ProjectionHelper.ToGetBankResponseDTOs(
                    responseDTO,
                    _idEncoderService,
                    validation.Claims.TenantEncriptionKey,
                    _config, _fileStorageService);

                // ===============================
                //  COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<List<GetBankResponseDTO>>.Success(
                    encryptedList,
                    "Bank info added successfully.");
            }
            catch (Exception ex)
            {
                // ===============================
                //  ROLLBACK
                // ===============================
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error adding bank info");

                //  FILE CLEANUP
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

                throw; //  MUST
            }
        }

    }
    #endregion
}

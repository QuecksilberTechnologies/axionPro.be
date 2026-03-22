using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; using MediatR;
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
                // =================================================
                // 1️⃣ COMMON VALIDATION
                // =================================================
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

                // =================================================
                // 2️⃣ FETCH EXISTING RECORD
                // =================================================
                var bank = await _unitOfWork.EmployeeBankRepository
                    .GetSingleRecordAsync(request.DTO.Id, true);

                if (bank == null)
                    return ApiResponse<bool>.Fail("Employee bank record not found.");

                var dto = request.DTO;

                // =================================================
                // 3️⃣ UPDATE FIELDS
                // =================================================
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

                // =================================================
                // 4️⃣ PRIMARY ACCOUNT RULE
                // =================================================
                if (dto.IsPrimaryAccount)
                {
                    if (dto.CancelledChequeFile == null && !bank.HasChequeDocUploaded)
                        return ApiResponse<bool>.Fail("Cancelled cheque is mandatory.");

                    bool resetDone = await _unitOfWork.EmployeeBankRepository
                        .ResetPrimaryAccountAsync(
                            request.DTO.Prop.EmployeeId,
                            request.DTO.Prop.UserEmployeeId);

                    if (!resetDone)
                        return ApiResponse<bool>.Fail("Failed to reset primary accounts.");

                    bank.IsPrimaryAccount = true;
                }
                else
                {
                    bank.IsPrimaryAccount = false;
                }

             
                // =================================================
                // FILE HANDLING (S3 - NO DELETE, AUDIT SAFE)
                // =================================================
                if (request.DTO.CancelledChequeFile != null &&
                    request.DTO.CancelledChequeFile.Length > 0)
                {
                    try
                    {
                       
                        // 🔹 BUILD FOLDER PATH
                        string folderPath =  $"{ConstantValues.TenantFolder}-{request.DTO.Prop.TenantId}/{ConstantValues.EmployeeFolder}/{request.DTO.Prop.EmployeeId}/{ConstantValues.BankFolder}";


                        // 🔹 NEW FILE NAME (UNIQUE)
                        string newFileName =
                            $"{ConstantValues.BankFolder}-{request.DTO.Prop.EmployeeId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        // 🔹 UPLOAD NEW FILE
                        var fileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.CancelledChequeFile,
                            folderPath,
                            newFileName);

                        // 🔹 UPDATE DB (ONLY NEW FILE)
                        bank.FilePath = fileKey;
                        bank.FileName = newFileName;
                        bank.HasChequeDocUploaded = true;
                        bank.UpdatedDateTime = DateTime.UtcNow;
                        bank.UpdatedById = request.DTO.Prop.UserEmployeeId;

                        _logger.LogInformation("New cheque uploaded. Old file preserved.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading cheque file");

                        return ApiResponse<bool>.Fail("File upload failed.");
                    }
                }
                var responseData = _mapper.Map<UpdateBankReqestDTO>(bank);
                responseData.Prop.UserEmployeeId = request.DTO.Prop.UserEmployeeId;
                responseData.Prop.EmployeeId = request.DTO.Prop.EmployeeId;

                //responseData.UpdatedDateTime = DateTime.UtcNow;

                // -------------------------------------------------
                // 7️⃣ SAVE CHANGES
                // -------------------------------------------------
                bool isSuccess = await _unitOfWork.EmployeeBankRepository.UpdateAsync(responseData);
                

                if (!isSuccess)
                    return ApiResponse<bool>.Fail("Failed to update bank info.");

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<bool>.Success(true, "Bank updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error updating bank info");

                return ApiResponse<bool>.Fail("Unexpected error.", new List<string> { ex.Message });
            }
        }
    }

}

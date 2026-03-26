using AutoMapper;
using axionpro.application.DTOS.Employee.Experience;
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

namespace axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers
{
    public class UpdateExperienceInfoCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateExperienceRequestDTO DTO { get; set; }

        public UpdateExperienceInfoCommand(UpdateExperienceRequestDTO dto)
        {
            DTO = dto;
        }

    }
    public class UpdateExperienceInfoCommandHandler : IRequestHandler<UpdateExperienceInfoCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateExperienceInfoCommand> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;

        public UpdateExperienceInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateExperienceInfoCommand> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService, ICommonRequestService commonRequestService

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
            _commonRequestService = commonRequestService;

        }

        public async Task<ApiResponse<bool>> Handle(
          UpdateExperienceInfoCommand request,
          CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🚀 UpdateExperience (Parent Only) started");

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request");

                

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
 

                // ===============================
                // 3️⃣ FETCH EXISTING (ONLY PARENT)
                // ===============================
                var existing = await _unitOfWork.EmployeeExperienceRepository.GetByEmployeeIdWithDetailsAsync(request.DTO.Prop.UserEmployeeId);

                if (existing == null)
                    throw new ApiException("Experience record not found.", 404);

                // ===============================
                // 4️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 5️⃣ UPDATE PARENT ONLY
                // ===============================
                // ===============================
                // 5️⃣ SAFE UPDATE (PARTIAL UPDATE)
                // ===============================

                // 🔹 CTC
                if (request.DTO.Ctc.HasValue)
                    existing.Ctc = request.DTO.Ctc.Value;

                // 🔹 Comment
                if (!string.IsNullOrWhiteSpace(request.DTO.Comment))
                    existing.Comment = request.DTO.Comment;

                // 🔹 HasEPFAccount (bool)
                if (request.DTO.HasEPFAccount != existing.HasEPFAccount)
                    existing.HasEPFAccount = request.DTO.HasEPFAccount;

                // 🔹 IsFresher (bool)
                if (request.DTO.IsFresher != existing.IsFresher)
                    existing.IsFresher = request.DTO.IsFresher;

                // 🔹 Audit Fields
                existing.UpdatedById = validation.UserEmployeeId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                /*
                    🔥 CURRENT STEP BEHAVIOR

                    ✔ Only parent updated
                    ✔ No child touched
                    ✔ No delete
                    ✔ No insert

                    👉 Safe step for testing
                */

                // ===============================
                // 6️⃣ SAVE
                // ===============================
                await _unitOfWork.SaveChangesAsync();

                // ===============================
                // 7️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Parent Experience updated successfully");

                return ApiResponse<bool>.Success(true, "Experience updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ UpdateExperience (Parent) failed");

                await _unitOfWork.RollbackTransactionAsync();

                throw;
            }
        }
    }
}

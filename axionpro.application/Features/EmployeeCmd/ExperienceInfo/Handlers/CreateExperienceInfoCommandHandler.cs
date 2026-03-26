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
    public class CreateExperienceInfoCommand : IRequest<ApiResponse<List<GetEmployeeExperienceResponseDTO>>>
    {
        public CreateExperienceRequestDTO DTO { get; set; }

        public CreateExperienceInfoCommand(CreateExperienceRequestDTO dTO)
        {
            DTO = dTO;
        }
    }

    public class CreateExperienceInfoCommandHandler
     : IRequestHandler<CreateExperienceInfoCommand, ApiResponse<List<GetEmployeeExperienceResponseDTO>>>
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
        public CreateExperienceInfoCommandHandler(
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
        public async Task<ApiResponse<List<GetEmployeeExperienceResponseDTO>>> Handle(
    CreateExperienceInfoCommand request,
    CancellationToken cancellationToken)
        {
            List<string> uploadedFiles = new();

            try
            {
                _logger.LogInformation("🚀 CreateExperience started");

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
                // 2️⃣ BUSINESS VALIDATION
                // ===============================
                if (request.DTO.IsFresher && request.DTO.ExperienceDetails?.Any() == true)
                    throw new ValidationErrorException("Fresher cannot have experience details.");

                if (!request.DTO.IsFresher && (request.DTO.ExperienceDetails == null || !request.DTO.ExperienceDetails.Any()))
                    throw new ValidationErrorException("Experience details required.");

                // ===============================
                // 3️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 4️⃣ CREATE PARENT
                // ===============================
                var exp = new EmployeeExperience
                {
                    EmployeeId = validation.UserEmployeeId,
                    Ctc = request.DTO.Ctc,
                    Comment = request.DTO.Comment,
                    HasEPFAccount = request.DTO.HasEPFAccount,
                    IsFresher = request.DTO.IsFresher,

                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    IsActive = true,
                    IsSoftDeleted = false
                };

                await _unitOfWork.EmployeeExperienceRepository.AddAsync(exp);

                // ===============================
                // 5️⃣ CREATE DETAILS + DOCS (IN MEMORY)
                // ===============================
                if (!request.DTO.IsFresher)
                {
                    foreach (var detailDto in request.DTO.ExperienceDetails!)
                    {
                        var detail = _mapper.Map<EmployeeExperienceDetail>(detailDto);

                        detail.EmployeeExperience = exp; // 🔥 IMPORTANT (no need of exp.Id yet)

                        detail.AddedById = validation.UserEmployeeId;
                        detail.AddedDateTime = DateTime.UtcNow;
                        detail.IsActive = true;
                        detail.IsSoftDeleted = false;

                        // 🔹 Documents
                        if (detailDto.Documents != null && detailDto.Documents.Any())
                        {
                            detail.EmployeeExperienceDocuments = new List<EmployeeExperienceDocument>();

                            foreach (var docDto in detailDto.Documents)
                            {
                                var doc = _mapper.Map<EmployeeExperienceDocument>(docDto);

                                doc.AddedById = validation.UserEmployeeId;
                                doc.AddedDateTime = DateTime.UtcNow;
                                doc.IsActive = true;
                                doc.IsSoftDeleted = false;

                                if (!string.IsNullOrWhiteSpace(doc.FilePath))
                                    uploadedFiles.Add(doc.FilePath);

                                detail.EmployeeExperienceDocuments.Add(doc);
                            }
                        }

                        exp.EmployeeExperienceDetails.Add(detail);
                    }
                }

                // ===============================
                // 6️⃣ SAVE ALL (🔥 SINGLE SAVE)
                // ===============================
                await _unitOfWork.SaveChangesAsync();

                // ===============================
                // 7️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Experience created successfully");

                return ApiResponse<List<GetEmployeeExperienceResponseDTO>>
                    .Success(new List<GetEmployeeExperienceResponseDTO>(), "Experience saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CreateExperience failed");

                await _unitOfWork.RollbackTransactionAsync();

                // 🔥 FILE CLEANUP
                foreach (var file in uploadedFiles)
                {
                    try
                    {
                        await _fileStorageService.DeleteFileAsync(file);
                    }
                    catch { }
                }

                throw;
            }
        }


    }




    }

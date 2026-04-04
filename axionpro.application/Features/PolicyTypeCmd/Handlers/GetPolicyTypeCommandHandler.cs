using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.PolicyTypeDocument;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    // ======================================================
    // 🔹 COMMAND
    // ======================================================
    public class GetPolicyTypeCommand
        : IRequest<ApiResponse<List<GetPolicyTypeResponseDTO>>>
    {
        public GetPolicyTypeRequestDTO DTO { get; set; }

        public GetPolicyTypeCommand(GetPolicyTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ======================================================
    // 🔹 HANDLER
    // ======================================================
    public class GetPolicyTypeCommandHandler
        : IRequestHandler<GetPolicyTypeCommand, ApiResponse<List<GetPolicyTypeResponseDTO>>>
    {
        private readonly IPolicyTypeRepository _policyTypeRepository;
        private readonly IPolicyTypeDocumentRepository _companyPolicyDocumentRepository;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IConfiguration _config;
        private readonly ILogger<CreatePolicyTypeCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        IPermissionService permissionService;

        public GetPolicyTypeCommandHandler(
            IPolicyTypeRepository policyTypeRepository,
            IPolicyTypeDocumentRepository companyPolicyDocumentRepository,
            ICommonRequestService commonRequestService,
            IConfiguration config, ILogger<CreatePolicyTypeCommandHandler> logger, IPermissionService permissionService, IFileStorageService fileStorageService)
        {
            _policyTypeRepository = policyTypeRepository;
            _companyPolicyDocumentRepository = companyPolicyDocumentRepository;
            _commonRequestService = commonRequestService;
            _config = config;
            _logger = logger;
            this.permissionService = permissionService;
            _fileStorageService = fileStorageService;   
        }

        public async Task<ApiResponse<List<GetPolicyTypeResponseDTO>>> Handle(
    GetPolicyTypeCommand request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetPolicyType started");

                // ===============================
                // 1️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                // ===============================
                // 2️⃣ AUTH VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 3️⃣ PERMISSION CHECK
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.PolicyType,
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 4️⃣ PAGING DEFAULTS
                // ===============================
                var pageNumber = request.DTO.PageNumber > 0 ? request.DTO.PageNumber : 1;
                var pageSize = request.DTO.PageSize > 0 ? request.DTO.PageSize : 10;
                var sortOrder = string.IsNullOrWhiteSpace(request.DTO.SortOrder)
                    ? "desc"
                    : request.DTO.SortOrder.ToLower();

                // ===============================
                // 5️⃣ FETCH DATA
                // ===============================
                var policyTypes = await _policyTypeRepository
                    .GetPolicyTypesAsync(validation.TenantId, request.DTO.IsActive);

                var policyTypeList = policyTypes?.ToList() ?? new List<GetPolicyTypeResponseDTO>();

                // ===============================
                // 6️⃣ SORTING
                // ===============================
                policyTypeList = sortOrder == "asc"
                    ? policyTypeList.OrderBy(x => x.PolicyName).ToList()
                    : policyTypeList.OrderByDescending(x => x.PolicyName).ToList();

                // ===============================
                // 7️⃣ PAGING
                // ===============================
                var totalRecords = policyTypeList.Count;

                var pagedList = policyTypeList
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // ===============================
                // 8️⃣ DOCUMENT ATTACH (OPTIONAL)
                // ===============================
            
                if (pagedList.Any())
                {
                    var policyTypeIds = pagedList.Select(x => x.Id).ToList();

                    var documents = await _companyPolicyDocumentRepository
                        .GetByPolicyTypeIdsAsync(policyTypeIds, validation.TenantId);

                    

                    foreach (var policy in pagedList)
                    {
                        var docs = documents
                            .Where(d => d.PolicyTypeId == policy.Id)
                            .ToList();

                        policy.DocDetails = docs.Select(doc => new GetPolicyTypeDocumentResponseDTO
                        {
                            Id = doc.Id,
                            PolicyTypeId = doc.PolicyTypeId,
                            DocumentTitle = doc.DocumentTitle,
                            FileName = doc.FileName,
                            IsActive = doc.IsActive,
                            FilePath = !string.IsNullOrWhiteSpace(doc.FilePath) ? _fileStorageService.GetFileUrl(doc.FilePath) : null
                        }).ToList();
                    }
                }

                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                _logger.LogInformation("✅ Retrieved {Count} policy types", pagedList.Count);

                // ===============================
                // 9️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetPolicyTypeResponseDTO>>
                    .SuccessPaginated(
                        data: pagedList,
                        pageNumber: pageNumber,
                        pageSize: pageSize,
                        totalRecords: totalRecords,
                        totalPages: totalPages,
                        message: "Policy Types fetched successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetPolicyType");

                throw; // ✅ CRITICAL
            }
        }
    }
}

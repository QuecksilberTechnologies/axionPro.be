using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly ICompanyPolicyDocumentRepository _companyPolicyDocumentRepository;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IConfiguration _config;

        public GetPolicyTypeCommandHandler(
            IPolicyTypeRepository policyTypeRepository,
            ICompanyPolicyDocumentRepository companyPolicyDocumentRepository,
            ICommonRequestService commonRequestService,
            IConfiguration config)
        {
            _policyTypeRepository = policyTypeRepository;
            _companyPolicyDocumentRepository = companyPolicyDocumentRepository;
            _commonRequestService = commonRequestService;
            _config = config;
        }

        public async Task<ApiResponse<List<GetPolicyTypeResponseDTO>>> Handle(
            GetPolicyTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // --------------------------------------------------
                // 1️⃣ Basic validation
                // --------------------------------------------------
                if (request.DTO == null)
                {
                    return ApiResponse<List<GetPolicyTypeResponseDTO>>
                        .Fail("Invalid request data.");
                }

                // --------------------------------------------------
                // 2️⃣ Common validation (Tenant / User context)
                // --------------------------------------------------
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                {
                    return ApiResponse<List<GetPolicyTypeResponseDTO>>
                        .Fail(validation.ErrorMessage);
                }

                // --------------------------------------------------
                // 3️⃣ Paging + Sorting defaults (CLIENT PATTERN)
                // --------------------------------------------------
                var pageNumber = request.DTO.PageNumber > 0
                    ? request.DTO.PageNumber
                    : 1;

                var pageSize = request.DTO.PageSize > 0
                    ? request.DTO.PageSize
                    : 10;

                var sortOrder = string.IsNullOrWhiteSpace(request.DTO.SortOrder)
                    ? "desc"
                    : request.DTO.SortOrder.ToLower();

                // --------------------------------------------------
                // 4️⃣ Prepare filter for repository
                // (Filtering DB side hoti hai)
                // --------------------------------------------------
               
                // --------------------------------------------------
                // 5️⃣ Fetch PolicyTypes
                // --------------------------------------------------
                var policyTypes =  await _policyTypeRepository.GetAllPolicyTypesAsync(validation.TenantId , request.DTO.IsActive);

                if (policyTypes == null || !policyTypes.Any())
                {
                    return ApiResponse<List<GetPolicyTypeResponseDTO>>
                        .Fail("No Policy Types found.");
                }

                var policyTypeList = policyTypes.ToList();

                // --------------------------------------------------
                // 6️⃣ Sorting (Application layer)
                // --------------------------------------------------
                policyTypeList = sortOrder == "asc"
                    ? policyTypeList.OrderBy(x => x.PolicyName).ToList()
                    : policyTypeList.OrderByDescending(x => x.PolicyName).ToList();

                // --------------------------------------------------
                // 7️⃣ Paging (Application layer)
                // --------------------------------------------------
                var totalRecords = policyTypeList.Count;

                policyTypeList = policyTypeList
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // --------------------------------------------------
                // 8️⃣ Attach CompanyPolicyDocument (ONLY if asked)
                // --------------------------------------------------
                if (request.DTO.HasDocumnet == true)
                {
                    var policyTypeIds = policyTypeList
                        .Select(x => x.Id)
                        .ToList();

                    var documents =
                        await _companyPolicyDocumentRepository
                            .GetByPolicyTypeIdsAsync(
                                policyTypeIds,
                                validation.TenantId);

                    string baseUrl = _config["FileSettings:BaseUrl"] ?? string.Empty;

                    foreach (var policy in policyTypeList)
                    {
                        var doc = documents
                            .FirstOrDefault(d => d.PolicyTypeId == policy.Id);

                        if (doc != null)
                        {
                            policy.DocDetails = new GetCompanyPolicyDocumentResponseDTO
                            {
                                Id = doc.Id,
                                PolicyTypeId = doc.PolicyTypeId,
                                DocumentTitle = doc.DocumentTitle,
                                FileName = doc.FileName,
                                IsActive = doc.IsActive,

                                // 🔥 Absolute URL build
                                URL = !string.IsNullOrWhiteSpace(doc.FilePath)
                                    ? $"{baseUrl.TrimEnd('/')}/{doc.FilePath.TrimStart('/')}"
                                    : null
                            };
                        }
                    }
                }
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                    

                return ApiResponse<List<GetPolicyTypeResponseDTO>>
                        .SuccessPaginated(
                          data:  policyTypeList,
                      pageNumber:   pageNumber,
                      pageSize:  pageSize,
                      totalRecords:  totalRecords,
                      totalPages:  totalPages,
                     message : "Policy Types fetched successfully."
                        );



            }
            catch (Exception ex)
            {
                return ApiResponse<List<GetPolicyTypeResponseDTO>>
                    .Fail($"An error occurred: {ex.Message}");
            }
        }
    }
}

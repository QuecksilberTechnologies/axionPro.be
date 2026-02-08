using axionpro.application.DTOs.PolicyType;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    // ======================================================
    // 🔹 COMMAND
    // ======================================================
    public class GetAllPolicyTypeCommand
        : IRequest<ApiResponse<List<GetAllPolicyTypeResponseDTO>>>
    {
        public GetAllPolicyTypeRequestDTO DTO { get; }

        public GetAllPolicyTypeCommand(GetAllPolicyTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ======================================================
    // 🔹 HANDLER
    // ======================================================
    public class GetAllPolicyTypeCommandHandler
        : IRequestHandler<
            GetAllPolicyTypeCommand,
            ApiResponse<List<GetAllPolicyTypeResponseDTO>>>
    {
        private readonly IPolicyTypeRepository _policyTypeRepository;
        private readonly ICommonRequestService _commonRequestService;

        public GetAllPolicyTypeCommandHandler(
            IPolicyTypeRepository policyTypeRepository,
            ICommonRequestService commonRequestService)
        {
            _policyTypeRepository = policyTypeRepository;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetAllPolicyTypeResponseDTO>>> Handle(
            GetAllPolicyTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // --------------------------------------------------
                // 1️⃣ Basic validation
                // --------------------------------------------------
                if (request.DTO == null)
                {
                    return ApiResponse<List<GetAllPolicyTypeResponseDTO>>
                        .Fail("Invalid request.");
                }

                // --------------------------------------------------
                // 2️⃣ Common validation (Tenant / User context)
                // --------------------------------------------------
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                {
                    return ApiResponse<List<GetAllPolicyTypeResponseDTO>>
                        .Fail(validation.ErrorMessage);
                }

                // --------------------------------------------------
                // 3️⃣ Fetch PolicyTypes (Repository handles filtering)
                // --------------------------------------------------
                var policyTypes =
                    await _policyTypeRepository.GetAllPolicyTypesAsync(
                        validation.TenantId,
                        request.DTO.IsActive
                    );

                // --------------------------------------------------
                // 4️⃣ No data → UI wants error
                // --------------------------------------------------
                if (policyTypes.Data == null || !policyTypes.Data.Any())
                {
                    return ApiResponse<List<GetAllPolicyTypeResponseDTO>>
                        .Fail("No policy types found.");
                }

                // --------------------------------------------------
                // 5️⃣ Success
                // --------------------------------------------------
                return ApiResponse<List<GetAllPolicyTypeResponseDTO>>
                    .Success(
                        policyTypes.Data,
                        "All policies fetched successfully.");
            }
            catch (Exception)
            {
                // ❗ Safe failure (no exception leak)
                return ApiResponse<List<GetAllPolicyTypeResponseDTO>>
                    .Fail("Something went wrong while fetching policy types.");
            }
        }
    }
}

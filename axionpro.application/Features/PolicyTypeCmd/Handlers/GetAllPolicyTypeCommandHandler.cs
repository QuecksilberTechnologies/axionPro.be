using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    // ======================================================
    // 🔹 COMMAND
    // ======================================================
    public class GetAllPolicyTypeCommand : IRequest<List<GetAllPolicyTypeResponseDTO>>
    {
        public GetAllPolicyTypeRequestDTO DTO { get; set; }

        public GetAllPolicyTypeCommand(GetAllPolicyTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ======================================================
    // 🔹 HANDLER
    // ======================================================
    public class GetAllPolicyTypeCommandHandler
     : IRequestHandler<GetAllPolicyTypeCommand, List<GetAllPolicyTypeResponseDTO>>
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

        public async Task<List<GetAllPolicyTypeResponseDTO>> Handle(
            GetAllPolicyTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // --------------------------------------------------
                // 1️⃣ Basic validation
                // --------------------------------------------------
                if (request.DTO == null)
                    return new List<GetAllPolicyTypeResponseDTO>();

                // --------------------------------------------------
                // 2️⃣ Common validation (Tenant / User context)
                // --------------------------------------------------
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return new List<GetAllPolicyTypeResponseDTO>();

                // --------------------------------------------------
                // 3️⃣ Fetch PolicyTypes (DB layer handles filtering)
                // --------------------------------------------------
                var policyTypes =
                    await _policyTypeRepository.GetAllPolicyTypesAsync(
                        validation.TenantId,
                        request.DTO.IsActive
                    );

                // --------------------------------------------------
                // 4️⃣ Safety return (never null)
                // --------------------------------------------------
                return policyTypes?.ToList() ?? new List<GetAllPolicyTypeResponseDTO>();
            }
            catch (Exception)
            {
                // ❗ No exception leak to controller
                // ❗ Return empty list – safe UI behavior
                return new List<GetAllPolicyTypeResponseDTO>();
            }
        }
    }

}
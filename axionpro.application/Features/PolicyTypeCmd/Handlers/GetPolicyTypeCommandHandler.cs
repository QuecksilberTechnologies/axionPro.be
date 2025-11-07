using AutoMapper;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.Features.PolicyTypeCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GetPolicyTypeCommandHandler : IRequestHandler<GetPolicyTypeCommand, ApiResponse<List<GetPolicyTypeResponseDTO>>>
{
    private readonly IPolicyTypeRepository _policyTypeRepository;
    private readonly IMapper _mapper;

    public GetPolicyTypeCommandHandler(IPolicyTypeRepository policyTypeRepository, IMapper mapper)
    {
        _policyTypeRepository = policyTypeRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<GetPolicyTypeResponseDTO>>> Handle(GetPolicyTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.DTO == null)
            {
                return new ApiResponse<List<GetPolicyTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Invalid request data.",
                    Data = new List<GetPolicyTypeResponseDTO>()
                };
            }

            // 🔹 Repository ke liye filter banate waqt
            var filterDTO = new GetPolicyTypeResponseDTO
            {
                IsActive = request.DTO.IsActive ?? false,  // null → false by default
                PolicyName = string.IsNullOrWhiteSpace(request.DTO.PolicyName)
                             ? string.Empty
                             : request.DTO.PolicyName.Trim(),
                TenantId = request.DTO.TenantId
            };

            var policyTypes = await _policyTypeRepository
                .GetAllPolicyTypesAsync(filterDTO);

            if (policyTypes == null || !policyTypes.Any())
            {
                return new ApiResponse<List<GetPolicyTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "No Policy Types found.",
                    Data = new List<GetPolicyTypeResponseDTO>()
                };
            }

            // Agar repository entity return karti hai to yaha mapping lagao
            // var policyTypeDTOs = _mapper.Map<List<GetPolicyTypeDTO>>(policyTypes);

            return new ApiResponse<List<GetPolicyTypeResponseDTO>>
            {
                IsSucceeded = true,
                Message = "Policy Types fetched successfully.",
                Data = policyTypes.ToList() // DTO hi aa raha hai to direct use karo
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<GetPolicyTypeResponseDTO>>
            {
                IsSucceeded = false,
                Message = $"An error occurred: {ex.Message}",
                Data = new List<GetPolicyTypeResponseDTO>()
            };
        }
    }
   

}

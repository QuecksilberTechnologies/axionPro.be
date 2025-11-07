using AutoMapper;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.TenantIndustry;
using axionpro.application.Features.TenantIndustryCmd.Queries;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TenantIndustryCmd.Handlers
{

    public class GetAllTenantIndustryQueryHandler
    : IRequestHandler<GetAllTenantIndustryQuery, ApiResponse<List<TenantIndustryResponseDTO>>>
    {
        private readonly ITenantIndustryRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllTenantIndustryQueryHandler> _logger;

        public GetAllTenantIndustryQueryHandler(
            ITenantIndustryRepository repository,
            IMapper mapper,
            ILogger<GetAllTenantIndustryQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<List<TenantIndustryResponseDTO>>> Handle(GetAllTenantIndustryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("📥 Fetching industries. PlanId: {PlanId}", request.PlanId);

                var industries = await _repository.GetAllActiveIndustriesAsync(); // Optional: filter by PlanId later

                var industryList = _mapper.Map<List<TenantIndustryResponseDTO>>(industries);

                return new ApiResponse<List<TenantIndustryResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "✅ Tenant industries fetched successfully.",
                    Data = industryList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching industries.");

                return new ApiResponse<List<TenantIndustryResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "❌ Failed to fetch tenant industries.",
                    Errors = new List<string> { ex.Message },
                    Data = null
                };
            }
        }
    }
}

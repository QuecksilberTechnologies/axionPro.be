using AutoMapper;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Features.AssetFeatures.Status.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Status.Handlers
{
    public class GetAllStatusCommandHandler : IRequestHandler<GetAllAssetStatusCommand, ApiResponse<List<GetStatusResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllStatusCommandHandler> _logger;

        public GetAllStatusCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllStatusCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetStatusResponseDTO>>> Handle(GetAllAssetStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {

                

                // Step 2: Fetch filtered data from repository
               List<GetStatusResponseDTO> assetStatusList = await _unitOfWork.AssetStatusRepository.GetAllAsync(request.DTO);

                if (assetStatusList.Count == 0) 
                {
                    return new ApiResponse<List<GetStatusResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Asset statuses not found!.",
                        Data = null
                    };

                }

                // Step 3: Map domain list to response DTO list
                 List<GetStatusResponseDTO> allAssetStatusResponseDTOs = _mapper.Map<List<GetStatusResponseDTO>>(assetStatusList);


                // Step 4: Logging
               _logger.LogInformation("Successfully retrieved {Count} asset status records.", allAssetStatusResponseDTOs.Count);


                // Step 5: Return wrapped response
                return new ApiResponse<List<GetStatusResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Asset statuses fetched successfully.",
                    Data = allAssetStatusResponseDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching asset status records.");

                return new ApiResponse<List<GetStatusResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching asset statuses.",
                    Data = null
                };
            }
        }
    }
}
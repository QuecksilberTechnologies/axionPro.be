using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Features.AssetFeatures.Type.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Type.Handlers
{
    public class AddTypeCommandHandler : IRequestHandler<AddTypeCommand, ApiResponse<List<GetTypeResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddTypeCommandHandler> _logger;

        public AddTypeCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<AddTypeCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetTypeResponseDTO>>> Handle(AddTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1️⃣: Validate request DTO
                if (request.DTO == null)
                {
                    _logger.LogWarning("AddTypeCommand received null DTO.");
                    return ApiResponse<List<GetTypeResponseDTO>>.Fail("Invalid request data.");
                }

                if (string.IsNullOrWhiteSpace(request.DTO.TypeName))
                {
                    _logger.LogWarning("TypeName is missing in AddTypeCommand.");
                    return ApiResponse<List<GetTypeResponseDTO>>.Fail("Type Name is required.");
                }

                if (request.DTO.AssetCategoryId <= 0)
                {
                    _logger.LogWarning("Invalid CategoryId provided in AddTypeCommand.");
                    return ApiResponse<List<GetTypeResponseDTO>>.Fail("Valid CategoryId is required.");
                }

                _logger.LogInformation("Adding Asset Type: {TypeName}", request.DTO.TypeName);

                // Step 2️⃣: Add asset type via repository
                var addedList = await _unitOfWork.AssetTypeRepository.AddAsync(request.DTO);

                if (addedList == null || addedList.Count == 0)
                {
                    _logger.LogWarning("Asset type insertion failed for TypeName: {TypeName}", request.DTO.TypeName);
                    return ApiResponse<List<GetTypeResponseDTO>>.Fail("Failed to add asset type.");
                }

                // Step 3️⃣: Logging success
                _logger.LogInformation("Successfully added asset type: {TypeName}", request.DTO.TypeName);

                // Step 4️⃣: Return success response
                return ApiResponse<List<GetTypeResponseDTO>>.Success(
                    addedList,
                    "Asset Type added successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding asset type: {TypeName}", request.DTO?.TypeName);
                return ApiResponse<List<GetTypeResponseDTO>>.Fail("An unexpected error occurred while adding asset type.");
            }
        }
    }
}

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
    public class AddStatusCommandHandler : IRequestHandler<AddStatusCommand, ApiResponse<GetStatusResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddStatusCommandHandler> _logger;
        private object entityDto;

        public AddStatusCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<AddStatusCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<GetStatusResponseDTO>> Handle(AddStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Validate request DTO
                if (request?.DTO == null)
                {
                    _logger.LogWarning("AddStatusCommand called with null DTO.");
                    return new ApiResponse<GetStatusResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = null
                    };
                }

                if (string.IsNullOrWhiteSpace(request.DTO.StatusName))
                {
                    _logger.LogWarning("StatusName is required.");
                    return new ApiResponse<GetStatusResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "StatusName is required.",
                        Data = null
                    };
                }

                if (request.DTO.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId: {TenantId}", request.DTO.TenantId);
                    return new ApiResponse<GetStatusResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Invalid TenantId.",
                        Data = null
                    };
                }

                // ✅ Map DTO to Entity & call repository
                var entityDto = await _unitOfWork.AssetStatusRepository.AddAsync(request.DTO);

                return new ApiResponse<GetStatusResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Asset Status created successfully.",
                    Data = entityDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding asset status for TenantId: {TenantId}", request.DTO?.TenantId);
                return new ApiResponse<GetStatusResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while adding asset status.",
                    Data = null
                };
            }
        }

    }

}
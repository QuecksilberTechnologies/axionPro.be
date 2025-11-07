using AutoMapper;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Designation;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    public class GetDesignationByIdQuery : IRequest<ApiResponse<GetSingleDesignationResponseDTO>>
    {
        public GetSingleDesignationRequestDTO Dto { get; set; }

        public GetDesignationByIdQuery(GetSingleDesignationRequestDTO dto)
        {
            Dto = dto;
        }
    }

    public class GetDesignationByIdQueryHandler : IRequestHandler<GetDesignationByIdQuery, ApiResponse<GetSingleDesignationResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDesignationByIdQueryHandler> _logger;

        public GetDesignationByIdQueryHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetDesignationByIdQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<GetSingleDesignationResponseDTO>> Handle(GetDesignationByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var designation = await _unitOfWork.DesignationRepository.GetByIdAsync(request.Dto);

                if (designation == null)
                {
                    _logger.LogWarning("❌ No designation found with the given ID: {DesignationId}", request.Dto.Id);

                    return new ApiResponse<GetSingleDesignationResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Designation not found.",
                        Data = null
                    };
                }

                _logger.LogInformation("✅ Successfully retrieved designation with ID: {DesignationId}", request.Dto.Id);

                return new ApiResponse<GetSingleDesignationResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Designation fetched successfully.",
                    Data = designation
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error while fetching designation with ID: {DesignationId}", request.Dto.Id);

                return new ApiResponse<GetSingleDesignationResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Failed to fetch designation due to an internal error.",
                    Data = null
                };
            }
        }
    }
}

using AutoMapper;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Department;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DepartmentCmd.Handlers
{


    public class GetDepartmentByIdQuery : IRequest<ApiResponse<GetSingleDepartmentResponseDTO>>
    {
        public GetSingleDepartmentRequestDTO Dto { get; set; }

        public GetDepartmentByIdQuery(GetSingleDepartmentRequestDTO dto)
        {
            this.Dto = dto;
        }
    }



    public class GetDepartmentByIdQueryHandled : IRequestHandler<GetDepartmentByIdQuery, ApiResponse<GetSingleDepartmentResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDepartmentByIdQueryHandled> _logger;

        public GetDepartmentByIdQueryHandled(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetDepartmentByIdQueryHandled> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<GetSingleDepartmentResponseDTO>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(request.Dto);

                if (department == null)
                {
                    _logger.LogWarning("No department found with the given ID: {DepartmentId}", request.Dto.Id);

                    return new ApiResponse<GetSingleDepartmentResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Department not found.",
                        Data = null
                    };
                }

                _logger.LogInformation("Successfully retrieved department with ID: {DepartmentId}", request.Dto.Id);

                return new ApiResponse<GetSingleDepartmentResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Department fetched successfully.",
                    Data = department
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching department with ID: {DepartmentId}", request.Dto.Id);

                return new ApiResponse<GetSingleDepartmentResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Failed to fetch department due to an internal error.",
                    Data = null
                };
            }
        }

    }
}

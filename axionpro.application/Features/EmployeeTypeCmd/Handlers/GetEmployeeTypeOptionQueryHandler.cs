using AutoMapper;
using axionpro.application.DTOs.EmployeeType;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Operation;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.EmployeeType;

using axionpro.application.Features.LeaveCmd.Handlers;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeTypeCmd.Handlers
{
    public class GetEmployeeTypeOptionQuery : IRequest<ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>>
    {
        public GetOptionRequestDTO DTO { get; set; }

        public GetEmployeeTypeOptionQuery(GetOptionRequestDTO dTO)
        {
            this.DTO = dTO;
        }
    }
    public class GetEmployeeTypeOptionQueryHandler : IRequestHandler<GetEmployeeTypeOptionQuery, ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetEmployeeTypeOptionQueryHandler> _logger;
        private readonly IEmployeeTypeRepository _employeeTypeRepository;
        public GetEmployeeTypeOptionQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetEmployeeTypeOptionQueryHandler> logger,IEmployeeTypeRepository employeeTypeRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _employeeTypeRepository = employeeTypeRepository;
        }

        public async Task<ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>> Handle(GetEmployeeTypeOptionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 Null check for request
                if (request?.DTO == null)
                {
                    return new ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. DTO cannot be null.",
                        Data = new List<GetEmployeeTypeResponseOptionDTO>()
                    };
                }

                // 🔹 IsActive default handling (agar null aaya to false kar do)
           //     bool isActive = request.DTO.IsActive;

                // 🔹 Repository se data fetch
                IEnumerable<GetEmployeeTypeResponseOptionDTO> employeeTypes   =      await _employeeTypeRepository.GetEmployeeTypesOptionAsync(request.DTO);

                // 🔹 Validation: Agar list null ya empty hai
                if (employeeTypes == null || !employeeTypes.Any())
                {
                    _logger.LogWarning("⚠️ No EmployeeType found in database.");

                    return new ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Employee Types found.",
                        Data = new List<GetEmployeeTypeResponseOptionDTO>()
                    };
                }

                // 🔹 Success response
                return new ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>
                {
                    IsSucceeded = true,
                    Message = "Employee Types fetched successfully.",
                    Data = employeeTypes.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching EmployeeTypes.");

                return new ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while fetching Employee Types: {ex.Message}",
                    Data = new List<GetEmployeeTypeResponseOptionDTO>() // null ke bajaye empty list bhejna better hai
                };
            }
        }

    }
}
    


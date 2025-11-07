using AutoMapper;
using axionpro.application.DTOs.EmployeeType;
using axionpro.application.DTOs.Leave;
using axionpro.application.Features.EmployeeTypeCmd.Queries;
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
    public class GetAllEmployeeTypeQueryHandler : IRequestHandler<GetAllEmployeeTypeQuery, ApiResponse<List<GetEmployeeTypeResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllLeaveRuleQueryHandler> _logger;
        private readonly IEmployeeTypeRepository _employeeTypeRepository;
        public GetAllEmployeeTypeQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllLeaveRuleQueryHandler> logger,IEmployeeTypeRepository employeeTypeRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _employeeTypeRepository = employeeTypeRepository;
        }

        public async Task<ApiResponse<List<GetEmployeeTypeResponseDTO>>> Handle(GetAllEmployeeTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 Null check for request
                if (request?.DTO == null)
                {
                    return new ApiResponse<List<GetEmployeeTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. DTO cannot be null.",
                        Data = new List<GetEmployeeTypeResponseDTO>()
                    };
                }

                // 🔹 IsActive default handling (agar null aaya to false kar do)
                bool isActive = request.DTO.IsActive;

                // 🔹 Repository se data fetch
                IEnumerable<GetEmployeeTypeResponseDTO> employeeTypes =
                    await _employeeTypeRepository.GetAllEmployeeTypesAsync(request.DTO);

                // 🔹 Validation: Agar list null ya empty hai
                if (employeeTypes == null || !employeeTypes.Any())
                {
                    _logger.LogWarning("⚠️ No EmployeeType found in database.");

                    return new ApiResponse<List<GetEmployeeTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Employee Types found.",
                        Data = new List<GetEmployeeTypeResponseDTO>()
                    };
                }

                // 🔹 Success response
                return new ApiResponse<List<GetEmployeeTypeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Employee Types fetched successfully.",
                    Data = employeeTypes.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching EmployeeTypes.");

                return new ApiResponse<List<GetEmployeeTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while fetching Employee Types: {ex.Message}",
                    Data = new List<GetEmployeeTypeResponseDTO>() // null ke bajaye empty list bhejna better hai
                };
            }
        }

    }
}
    


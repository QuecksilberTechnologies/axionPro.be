using AutoMapper;
 
 using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.DTOs.Leave;
using axionpro.application.Constants;

namespace axionpro.application.Features.LeaveCmd.Handlers
{
    public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, ApiResponse<List<GetLeaveTypResponseDTO>>>
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;

        public CreateLeaveTypeCommandHandler( ILeaveRepository leaveRepository,  IMapper mapper, IUnitOfWork unitOfWork, IStoreProcedureRepository commonRepository) // Inject CommonRepository
        {
            _leaveRepository = leaveRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _commonRepository = commonRepository;
        }

        public async Task<ApiResponse<List<GetLeaveTypResponseDTO>>> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validation
                if (request.createLeaveTypeDTO == null)
                {
                    return new ApiResponse<List<GetLeaveTypResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. LeaveType data is required.",
                        Data = new List<GetLeaveTypResponseDTO>()
                    };
                }

                if (string.IsNullOrWhiteSpace(request.createLeaveTypeDTO.LeaveName.Trim()))
                {
                    return new ApiResponse<List<GetLeaveTypResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Leave Name is required.",
                        Data = new List<GetLeaveTypResponseDTO>()
                    };
                }

                if (request.createLeaveTypeDTO.TenantId <= 0)
                {
                    return new ApiResponse<List<GetLeaveTypResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "TenantId must be valid.",
                        Data = new List<GetLeaveTypResponseDTO>()
                    };
                }

                // 2️⃣ DTO → Entity mapping
                LeaveType leaveTypeEntity = _mapper.Map<LeaveType>(request.createLeaveTypeDTO);
                leaveTypeEntity.AddedById = request.createLeaveTypeDTO.EmployeeId;
                leaveTypeEntity.AddedDateTime = DateTime.UtcNow;  
                leaveTypeEntity.IsActive = ConstantValues.IsByDefaultTrue;
                // 3️⃣ Repository Call
                List<LeaveType> leaveTypes = await _leaveRepository.CreateLeaveTypeAsync(leaveTypeEntity);
                if (leaveTypes == null || !leaveTypes.Any())
                {
                    return new ApiResponse<List<GetLeaveTypResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "LeaveType creation failed.",
                        Data = new List<GetLeaveTypResponseDTO>()
                    };
                }

                // 4️⃣ Mapping Entity → DTO
                List<GetLeaveTypResponseDTO> leaveTypeDTOs = _mapper.Map<List<GetLeaveTypResponseDTO>>(leaveTypes);

                return new ApiResponse<List<GetLeaveTypResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "LeaveType created successfully.",
                    Data = leaveTypeDTOs
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetLeaveTypResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while creating LeaveType: {ex.Message}",
                    Data = new List<GetLeaveTypResponseDTO>()
                };
            }
        }


    }


}

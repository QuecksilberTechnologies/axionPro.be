using AutoMapper;
using axionpro.application.DTOs.Leave;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.LeaveCmd.Handlers
{
    public class UpdateTicketTypeCommandHandler : IRequestHandler<UpdateLeaveTypeCommand, ApiResponse<bool>>
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateTicketTypeCommandHandler> _logger;

        public UpdateTicketTypeCommandHandler(
            ILeaveRepository leaveRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<UpdateTicketTypeCommandHandler> logger)
        {
            _leaveRepository = leaveRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            bool response = false;
            try
            {
                LeaveType leaveTypeEntity = _mapper.Map<LeaveType>(request.UpdateLeaveTypeDTO);

                response = await _leaveRepository.UpdateLeavTypeAsync(leaveTypeEntity, request.UpdateLeaveTypeDTO.EmployeeId);

                if (!response)
                {
                    _logger.LogWarning("⚠️ No LeaveType was updated. LeaveTypeId: {LeaveTypeId}, EmployeeId: {EmployeeId}",
                        request.UpdateLeaveTypeDTO.Id, request.UpdateLeaveTypeDTO.EmployeeId);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "No leave was updated.",
                        Data = false
                    };
                }

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("✅ LeaveType with ID {LeaveTypeId} updated successfully by EmployeeId {EmployeeId}.",
                    request.UpdateLeaveTypeDTO.Id, request.UpdateLeaveTypeDTO.EmployeeId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Leave updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating LeaveType with ID {LeaveTypeId}.",
                    request.UpdateLeaveTypeDTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}

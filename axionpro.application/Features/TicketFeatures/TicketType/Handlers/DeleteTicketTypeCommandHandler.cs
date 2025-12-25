using AutoMapper;
using axionpro.application.Constants;
 
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Features.TicketFeatures.TicketType.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketType.Handlers
{
    public class DeleteTicketTypeCommandHandler : IRequestHandler<DeleteTicketTypeCommand, ApiResponse<bool>>
    {
        private readonly ITicketTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;
        private readonly ILogger<DeleteTicketTypeCommandHandler> _logger;
        public DeleteTicketTypeCommandHandler(
            ITicketTypeRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IStoreProcedureRepository commonRepository,
            ILogger<DeleteTicketTypeCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<ApiResponse<bool>> Handle(DeleteTicketTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.DTO.Id <= 0)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid TicketType Id.",
                        Data = false
                    };
                }

                // Repository se entity fetch karo
                var entity = await _unitOfWork.TicketTypeRepository.GetByIdAsync(request.DTO.Id);

                if (entity == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = " TicketType not found.",
                        Data = false
                    };
                }

                 

                await _unitOfWork.TicketTypeRepository.DeleteAsync(request.DTO.Id, request.DTO.EmployeeId );

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("TicketType with Id {Id} deleted successfully.", request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "TicketType deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting TicketType Id {Id}", request.DTO.Id);
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while deleting TicketType: {ex.Message}",
                    Data = false
                };
            }
        }
    }

}

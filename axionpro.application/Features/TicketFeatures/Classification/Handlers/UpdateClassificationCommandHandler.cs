using AutoMapper;

using axionpro.application.DTOS.TicketDTO.Classification;
 using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Features.TicketFeatures.Classification.Commands;
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

namespace axionpro.application.Features.TicketFeatures.Classification.Handlers
{
    public class UpdateClassificationCommandHandler : IRequestHandler<UpdateClassificationCommand, ApiResponse<GetClassificationResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateClassificationCommandHandler> _logger;

        public UpdateClassificationCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<UpdateClassificationCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<GetClassificationResponseDTO>> Handle(UpdateClassificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null || request == null)
                {
                    return new ApiResponse<GetClassificationResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request or missing ticketclassification creation.",
                        Data = null
                    };
                }



                // Add ticketclassification and fetch updated ticketclassification list
                await _unitOfWork.BeginTransactionAsync();
                var ticketclassifications = await _unitOfWork.TicketClassificationRepository.UpdateAsync(request.DTO);


                if (ticketclassifications == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<GetClassificationResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Failed to add ticketclassification or no ticketclassifications found.",
                        Data = null
                    };
                }
                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<GetClassificationResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "ticketclassification created successfully.",
                    Data = ticketclassifications
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the ticketclassification creation request.");
                return new ApiResponse<GetClassificationResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while processing the request.",
                    Data = null
                };
            }
        }
    }


}
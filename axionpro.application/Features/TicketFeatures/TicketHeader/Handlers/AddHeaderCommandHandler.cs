using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Features.TicketFeatures.TicketHeader.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace axionpro.application.Features.TicketFeatures.TicketHeader.Handlers
{
    public class AddHeaderCommandHandler : IRequestHandler<AddHeaderCommand, ApiResponse<List<GetHeaderResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddHeaderCommandHandler> _logger;

        public AddHeaderCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<AddHeaderCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetHeaderResponseDTO>>> Handle(AddHeaderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null || request.dto == null)
                {
                    _logger.LogWarning("⚠️ Null request or DTO in AddHeaderCommandHandler.");
                    return new ApiResponse<List<GetHeaderResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request or header data.",
                        Data = null
                    };
                }

                var dto = request.dto;

                if (string.IsNullOrWhiteSpace(dto.HeaderName))
                {
                    return new ApiResponse<List<GetHeaderResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Header name is required.",
                        Data = null
                    };
                }

                if (dto.EmployeeId <= 0)
                {
                    return new ApiResponse<List<GetHeaderResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid EmployeeId.",
                        Data = null
                    };
                }

                // Begin transaction
                await _unitOfWork.BeginTransactionAsync();

                // Add header via repository
                var addedHeader = await _unitOfWork.TicketHeaderRepository.AddAsync(dto);

                if (addedHeader == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogWarning("❌ Failed to add header: {HeaderName}", dto.HeaderName);
                    return new ApiResponse<List<GetHeaderResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Failed to add header.",
                        Data = null
                    };
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Header '{HeaderName}' added successfully.", dto.HeaderName);
 
                return new ApiResponse<List<GetHeaderResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Header added successfully.",
                    Data = addedHeader
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "❌ Exception in AddHeaderCommandHandler.");
                return new ApiResponse<List<GetHeaderResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Error occurred while adding header.",
                    Data = null
                };
            }
        }
    }
}

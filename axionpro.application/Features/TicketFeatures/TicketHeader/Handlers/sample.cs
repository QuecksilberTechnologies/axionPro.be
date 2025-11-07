using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Features.TicketFeatures.TicketHeader.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketHeader.Handlers
{
    internal class sample
    {
        /*
        using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Features.TicketFeatures.TicketHeader.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketHeader.Handlers
    {
        /// <summary>
        /// Handles fetching of ticket headers with optional filtering by multiple fields.
        /// </summary>
        public class GetHeaderByIdsQueryHandler : IRequestHandler<GetHeaderByIdQuery, ApiResponse<List<GetHeaderResponseDTO>>>
        {
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILogger<GetHeaderByIdsQueryHandler> _logger;

            public GetHeaderByIdsQueryHandler(
                IMapper mapper,
                IUnitOfWork unitOfWork,
                ILogger<GetHeaderByIdsQueryHandler> logger)
            {
                _mapper = mapper;
                _unitOfWork = unitOfWork;
                _logger = logger;
            }

            public async Task<ApiResponse<List<GetHeaderResponseDTO>>> Handle(GetHeaderByIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    // Step 1: Validate request
                    if (request == null || request.DTO == null)
                    {
                        _logger.LogWarning("⚠️ GetHeaderByIdQuery received with null DTO.");
                        return new ApiResponse<List<GetHeaderResponseDTO>>
                        {
                            IsSucceeded = false,
                            Message = "Invalid request. DTO cannot be null.",
                            Data = null
                        };
                    }

                    await using var context = await _unitOfWork.con();

                    // Step 2: Get all headers as queryable
                    var query = context.TicketHeaders.AsQueryable();

                    // Step 3: Apply filters if provided
                    if (request.DTO.Id > 0)
                        query = query.Where(x => x.Id == request.DTO.Id);

                    if (!string.IsNullOrEmpty(request.DTO.HeaderName))
                        query = query.Where(x => x.HeaderName.Contains(request.DTO.HeaderName));

                    if (!string.IsNullOrEmpty(request.DTO.Description))
                        query = query.Where(x => x.Description.Contains(request.DTO.Description));

                    if (request.DTO.TicketClassificationId.HasValue)
                        query = query.Where(x => x.TicketClassificationId == request.DTO.TicketClassificationId.Value);

                    if (request.DTO.IsActive.HasValue)
                        query = query.Where(x => x.IsActive == request.DTO.IsActive.Value);

                    if (request.DTO.IsAssetRelated.HasValue)
                        query = query.Where(x => x.IsAssetRelated == request.DTO.IsAssetRelated.Value);

                    // Step 4: Execute query
                    var headersList = await query.ToListAsync(cancellationToken);

                    if (headersList == null || !headersList.Any())
                    {
                        _logger.LogInformation("ℹ️ No headers found with the provided filters.");
                        return new ApiResponse<List<GetHeaderResponseDTO>>
                        {
                            IsSucceeded = false,
                            Message = "No headers found with the provided filters.",
                            Data = null
                        };
                    }

                    // Step 5: Map entities to DTOs
                    var result = _mapper.Map<List<GetHeaderResponseDTO>>(headersList);

                    // Step 6: Return success response
                    _logger.LogInformation("✅ {Count} headers fetched successfully.", result.Count);
                    return new ApiResponse<List<GetHeaderResponseDTO>>
                    {
                        IsSucceeded = true,
                        Message = "Headers fetched successfully.",
                        Data = result
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error occurred while fetching headers.");
                    return new ApiResponse<List<GetHeaderResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "An unexpected error occurred while fetching headers.",
                        Data = null
                    };
                }
            }
        }
    }
        */
}
}

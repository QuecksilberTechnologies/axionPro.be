using AutoMapper;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Features.TicketFeatures.TicketType.Queries;
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

namespace axionpro.application.Features.TicketFeatures.TicketType.Handlers
{
    public class GetAllTicketTypeByModuleIdQueryHandler : IRequestHandler<GetAllTicketTypeByHeaderIdQuery, ApiResponse<List<GetTicketTypeResponseDTO>>>
    {
        private readonly ITicketTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRepository _commonRepository;
        private readonly ILogger<GetAllTicketTypeByModuleIdQueryHandler> _logger;

        public GetAllTicketTypeByModuleIdQueryHandler(
            ITicketTypeRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICommonRepository commonRepository,
            ILogger<GetAllTicketTypeByModuleIdQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public GetAllTicketTypeByModuleIdQueryHandler(
          IUnitOfWork unitOfWork,
          IMapper mapper,
          ILogger<GetAllTicketTypeByModuleIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<List<GetTicketTypeResponseDTO>>> Handle(GetAllTicketTypeByHeaderIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validation
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("⚠️ Invalid request received in GetAllTicketTypeByModuleIdQuery.");
                    return new ApiResponse<List<GetTicketTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. Please provide valid module information.",
                        Data = new List<GetTicketTypeResponseDTO>()
                    };
                }

                // 2️⃣ Extract ModuleId & Active filter
                var moduleId = request.DTO.TicketHeaderId;
               
                if (moduleId <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid ModuleId received: {ModuleId}", moduleId);
                    return new ApiResponse<List<GetTicketTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "ModuleId must be greater than zero.",
                        Data = new List<GetTicketTypeResponseDTO>()
                    };
                }

                // 3️⃣ Repository Call
                var ticketTypes = await _unitOfWork.TicketTypeRepository.AllByHeaderIdAsync(request.DTO);

                // 4️⃣ Check Result
                if (ticketTypes == null || !ticketTypes.Any())
                {
                    _logger.LogWarning("⚠️ No TicketTypes found for ModuleId = {ModuleId} ", moduleId);

                    return new ApiResponse<List<GetTicketTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No TicketTypes found for the given module filter.",
                        Data = new List<GetTicketTypeResponseDTO>()
                    };
                }

                // 5️⃣ Mapping
                var ticketTypeResponse = _mapper.Map<List<GetTicketTypeResponseDTO>>(ticketTypes);

                _logger.LogInformation("✅ Successfully retrieved {Count} TicketTypes for ModuleId = {ModuleId} .",
                    ticketTypeResponse.Count, moduleId);

                // 6️⃣ Success Response
                return new ApiResponse<List<GetTicketTypeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"Successfully fetched {ticketTypeResponse.Count} TicketTypes.",
                    Data = ticketTypeResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception occurred while fetching TicketTypes by ModuleId.");

                return new ApiResponse<List<GetTicketTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An unexpected error occurred while fetching TicketTypes: {ex.Message}",
                    Data = new List<GetTicketTypeResponseDTO>() // Empty list for safety
                };
            }
        }

       
    }

}

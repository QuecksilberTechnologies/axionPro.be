using AutoMapper;
using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TicckeAllCmd.TicketType.Handlers
{
    public class DDLTicketTypeQuery
        : IRequest<ApiResponse<List<GetDDLTicketTypeResponseDTO>>>
    {
        public GetDDLTicketTypeRequestDTO DTO { get; }

        public DDLTicketTypeQuery(GetDDLTicketTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class DDLTicketTypeQueryHandler : IRequestHandler<DDLTicketTypeQuery, ApiResponse<List<GetDDLTicketTypeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DDLTicketTypeQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public DDLTicketTypeQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<DDLTicketTypeQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetDDLTicketTypeResponseDTO>>> Handle(
     DDLTicketTypeQuery request,
     CancellationToken cancellationToken)
        {
            try
            {
                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ RBAC CHECK
                // ===============================
                //await _commonRequestService.HasAccessAsync(
                //    ModuleEnum.Ticket,
                //    OperationEnum.View);

                 
                var result = await _unitOfWork.TicketTypeRepository
                    .GetDDLAsync(request.DTO.IsActive,validation.TenantId);

                if (result == null)
                    throw new ApiException("Failed to fetch TicketTypes.", 500);

                // ===============================
                // 6️⃣ RETURN (YOUR PATTERN 🔥)
                // ===============================
                return ApiResponse<List<GetDDLTicketTypeResponseDTO>>
                    .Success(result, "TicketType dropdown fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TicketTypes");
                throw;
            }
        }
    }

}


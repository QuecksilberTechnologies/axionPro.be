using AutoMapper;
using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TickeAllCmd.TicketType.Handlers
{
    public class GetAllTicketTypeQuery
        : IRequest<ApiResponse<List<GetTicketTypeResponseDTO>>>
    {
        public GetTicketTypeRequestDTO DTO { get; }

        public GetAllTicketTypeQuery(GetTicketTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class GetAllTicketTypeQueryHandler: IRequestHandler<GetAllTicketTypeQuery, ApiResponse<List<GetTicketTypeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllTicketTypeQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public GetAllTicketTypeQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllTicketTypeQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetTicketTypeResponseDTO>>> Handle(
     GetAllTicketTypeQuery request,
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

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                request.DTO.Prop ??= new ExtraPropRequestDTO();
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 4️⃣ DEFAULT PAGINATION (IMPORTANT 🔥)
                // ===============================
                request.DTO.PageNumber = request.DTO.PageNumber <= 0 ? 1 : request.DTO.PageNumber;
                request.DTO.PageSize = request.DTO.PageSize <= 0 ? 10 : request.DTO.PageSize;

                // ===============================
                // 5️⃣ REPOSITORY CALL
                // ===============================
                var result = await _unitOfWork.TicketTypeRepository
                    .AllAsync(request.DTO);

                if (result == null)
                    throw new ApiException("Failed to fetch TicketTypes.", 500);

                // ===============================
                // 6️⃣ RETURN (YOUR PATTERN 🔥)
                // ===============================
                return ApiResponse<List<GetTicketTypeResponseDTO>>
                    .SuccessPaginatedOnly(
                        result.Data,
                        result.PageNumber,
                        result.PageSize,
                        result.TotalCount,
                        result.TotalPages,
                        "TicketTypes fetched successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TicketTypes");
                throw;
            }
        }
    }

}


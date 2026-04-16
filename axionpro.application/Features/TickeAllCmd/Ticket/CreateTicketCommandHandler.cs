using axionpro.application.DTOS.TicketDTO.Creation;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Features.TickeAllCmd.Ticket
{
    public class CreateTicketCommand : IRequest<ApiResponse<CreateTicketResponseDTO>>
    {
        public CreateTicketRequestDTO Request { get; set; }

        public CreateTicketCommand(CreateTicketRequestDTO request)
        {
            Request = request;
        }
    }
    //public class CreateTicketCommandHandler
    //: IRequestHandler<CreateTicketCommand, ApiResponse<CreateTicketResponseDTO>>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ICommonRequestService _commonRequestService;
    //    private readonly ILogger<CreateTicketCommandHandler> _logger;

    //    public CreateTicketCommandHandler(
    //        IUnitOfWork unitOfWork,
    //        ICommonRequestService commonRequestService,
    //        ILogger<CreateTicketCommandHandler> logger)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _commonRequestService = commonRequestService;
    //        _logger = logger;
    //    }

        //    public async Task<ApiResponse<CreateTicketResponseDTO>> Handle(
        //        CreateTicketCommand request,
        //        CancellationToken cancellationToken)
        //    {
        //        try
        //        {
        //            _logger.LogInformation("🟢 CreateTicket started");

        //            // ✅ 1. VALIDATION
        //            var validation = await _commonRequestService.ValidateRequestAsync();

        //            if (!validation.Success)
        //                throw new UnauthorizedAccessException(validation.ErrorMessage);

        //            if (request?.Request == null)
        //                throw new ValidationErrorException("Invalid request.");

        //            request.Request.Prop ??= new();

        //            request.Request.Prop.TenantId = validation.TenantId;
        //            request.Request.Prop.UserEmployeeId = validation.UserEmployeeId;

        //            // 🔐 Decode IDs
        //            long classificationId = request.Request.TicketClassificationId;
        //            long headerId = request.Request.TicketHeaderId;
        //            long typeId = request.Request.TicketTypeId;
        //            long requestedForUserId = request.Request.RequestedForUserId;

        //            // ✅ 2. GET TicketType → Role
        //            var ticketType = await _unitOfWork.TicketTypeRepository.GetByIdAsync(typeId);

        //            if (ticketType == null)
        //                throw new NotFoundException("TicketType not found.");

        //            var roleId = ticketType.ResponsibleRoleId;

        //            // ✅ 3. GET USERS FROM ROLE
        //            var users = await _unitOfWork.UserRoleRepository.GetUsersByRoleId(roleId);

        //            if (users == null || !users.Any())
        //                throw new NotFoundException("No users found for role.");

        //            // ✅ 4. LEAST LOAD LOGIC
        //            var userLoads = users.Select(u => new
        //            {
        //                UserId = u.UserId,
        //                Count = _unitOfWork.TicketRepository.Count(x =>
        //                    x.AssignedToUserId == u.UserId &&
        //                    x.Status != (int)TicketStatus.Closed)
        //            });

        //            var assignedUser = userLoads.OrderBy(x => x.Count).First();

        //            // ✅ 5. CREATE TICKET
        //            var ticket = new Ticket
        //            {
        //                TenantId = validation.TenantId,

        //                TicketNumber = $"TKT-{DateTime.UtcNow.Ticks}",

        //                TicketClassificationId = classificationId,
        //                TicketHeaderId = headerId,
        //                TicketTypeId = typeId,

        //                Description = request.Request.Description,
        //                Priority = request.Request.Priority,

        //                Status = (int)TicketStatus.Open,

        //                AssignedToUserId = assignedUser.UserId,

        //                RequestedByUserId = validation.UserId,
        //                RequestedForUserId = requestedForUserId,

        //                CreatedByUserId = validation.UserId,
        //                CreatedDateTime = DateTime.UtcNow,
        //                IsActive = true,
        //                IsSoftDeleted = false
        //            };

        //            await _unitOfWork.TicketRepository.AddAsync(ticket);
        //            await _unitOfWork.SaveChangesAsync();

        //            // ✅ 6. CREATE THREAD
        //            var thread = new TicketThread
        //            {
        //                TicketId = ticket.Id,
        //                CreatedDateTime = DateTime.UtcNow,
        //                IsActive = true
        //            };

        //            await _unitOfWork.TicketThreadRepository.AddAsync(thread);
        //            await _unitOfWork.SaveChangesAsync();

        //            // ✅ 7. CREATE FIRST COMMENT
        //            var comment = new TicketComment
        //            {
        //                TicketThreadId = thread.Id,
        //                Comment = request.Request.Description,
        //                CommentType = 1, // Internal
        //                CreatedByUserId = validation.UserId,
        //                CreatedDateTime = DateTime.UtcNow,
        //                IsActive = true,
        //                IsSoftDeleted = false
        //            };

        //            await _unitOfWork.TicketCommentRepository.AddAsync(comment);
        //            await _unitOfWork.SaveChangesAsync();

        //            _logger.LogInformation("✅ Ticket created successfully");

        //            return new ApiResponse<CreateTicketResponseDTO>
        //            {
        //                IsSucceeded = true,
        //                Message = "Ticket created successfully.",
        //                Data = new CreateTicketResponseDTO
        //                {
        //                    TicketId = ticket.Id.ToString(),
        //                    TicketNumber = ticket.TicketNumber,
        //                    Status = "Open",
        //                    Message = "Your ticket has been created."
        //                }
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "❌ Error while creating ticket");
        //            throw;
        //        }
        //    }
        //}
  //  }
}
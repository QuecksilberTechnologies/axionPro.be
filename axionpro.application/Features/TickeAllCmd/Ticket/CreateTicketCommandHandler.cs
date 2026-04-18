using axionpro.application.Common.Enums;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.TicketDTO.Ticket;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Features.TickeAllCmd.Ticket;

public class CreateTicketCommand : IRequest<ApiResponse<GetTicketResponseDTO>>
{
    public CreateTicketRequestDTO DTO { get; set; }

    public CreateTicketCommand(CreateTicketRequestDTO request)
    {
        DTO = request;
    }
}
    /*
    public class CreateTicketCommandHandler
   : IRequestHandler<CreateTicketCommand, ApiResponse<GetTicketResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateTicketCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public CreateTicketCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateTicketCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetTicketResponseDTO>> Handle(
            CreateTicketCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ RBAC
                // ===============================
                //await _commonRequestService.HasAccessAsync(
                //    ModuleEnum.Ticket,
                //    OperationEnum.Add);

                var dto = request.DTO;

                if (dto == null)
                    throw new ValidationErrorException("Invalid request");

                dto.Prop ??= new ExtraPropRequestDTO();
                dto.Prop.TenantId = validation.TenantId;

                if (string.IsNullOrWhiteSpace(dto.Description))
                    throw new ValidationErrorException("Description required");

                // ===============================
                // 3️⃣ GET TICKET TYPE (ENGINE 🔥)
                // ===============================
                var ticketType = await _unitOfWork.TicketTypeRepository
                    .GetByIdAsync(dto.TicketTypeId);

                if (ticketType == null)
                    throw new ValidationErrorException("Invalid TicketType");

                // ===============================
                // 4️⃣ ATTACHMENT VALIDATION
                // ===============================
                if (ticketType.IsAttachmentRequired &&
                    (dto.AttachmentUrls == null || !dto.AttachmentUrls.Any()))
                {
                    throw new ValidationErrorException("Attachment required for this ticket type");
                }

                // ===============================
                // 5️⃣ ASSIGNMENT + APPROVAL LOGIC
                // ===============================
                int status;
                int? assignedRoleId;
                bool isApproved;

                if (ticketType.IsApprovalRequired)
                {
                    Enum.IsDefined(typeof(TicketStatus), TicketStatus.PendingApproval);
                    status = (int)TicketStatus.PendingApproval;
                    assignedRoleId = ticketType.ApprovalId;
                    isApproved = false;
                }
                else
                {
                    status = (int)TicketStatus.Open;
                    assignedRoleId = ticketType.ResponsibleRoleId;
                    isApproved = true;
                }

                // ===============================
                // 6️⃣ CREATE ENTITY
                // ===============================
                var ticket = new domain.Entity.Ticket
                {
                    TenantId = validation.TenantId,

                    TicketNumber = await GenerateTicketNumber(),

                    TicketClassificationId = dto.TicketClassificationId,
                    TicketHeaderId = dto.TicketHeaderId,
                    TicketTypeId = dto.TicketTypeId,

                    Description = dto.Description,
                    Priority = dto.Priority,
                    Status = status,

                    AssignedToRoleId = assignedRoleId,

                    RequestedByUserId = validation.UserEmployeeId,
                    RequestedForUserId = dto.RequestedForUserId,

                    IsApproved = isApproved,

                    SLAHoursSnapshot = ticketType.SLAHours,
                    SLAStartTime = DateTime.UtcNow,
                    SLAEndTime = ticketType.SLAHours != null
                        ? DateTime.UtcNow.AddHours(ticketType.SLAHours.Value)
                        : null,

                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,

                    IsActive = true,
                    IsSoftDeleted = false
                };

                await _unitOfWork.TicketGenrationRepository.AddAsync(ticket);
                await _unitOfWork.SaveChangesAsync();

                // ===============================
                // 7️⃣ SAVE ATTACHMENTS
                // ===============================
                if (dto.AttachmentUrls != null && dto.AttachmentUrls.Any())
                {
                    foreach (var url in dto.AttachmentUrls)
                    {
                        var attachment = new TicketAttachment
                        {
                            TicketId = ticket.Id,
                            FilePath = url,
                            FileName = Path.GetFileName(url),

                            UploadedByUserId = validation.UserEmployeeId,
                            UploadedDateTime = DateTime.UtcNow,

                            IsActive = true,
                            IsSoftDeleted = false
                        };

                        await _unitOfWork.TicketAttachmentRepository.AddAsync(attachment);
                    }
                }

                // ===============================
                // 8️⃣ THREAD CREATE
                // ===============================
                var thread = new TicketThread
                {
                    EntityType = (int)ThreadEntityType.Ticket,
                    EntityId = ticket.Id,
                    TenantId = validation.TenantId,
                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    IsActive = true,
                    IsSoftDeleted = false
                };

                await _unitOfWork.TicketThreadRepository.AddAsync(thread);
                await _unitOfWork.SaveChangesAsync();

                // ===============================
                // 9️⃣ FIRST MESSAGE
                // ===============================
                var message = new ThreadMessage
                {
                    ThreadId = thread.Id,
                    Message = "Ticket created",
                    MessageType = (int)MessageType.System,
                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    IsActive = true,
                    IsSoftDeleted = false
                };

                await _unitOfWork.ThreadMessageRepository.AddEntityAsync(message);

                // ===============================
                // 🔟 HISTORY
                // ===============================
                var history = new TicketHistory
                {
                    TicketId = ticket.Id,
                    Action = "Created",
                    NewStatus = ticket.Status,
                    DoneByUserId = validation.UserEmployeeId,
                    DoneOn = DateTime.UtcNow
                };

                await _unitOfWork.TicketHistoryRepository.AddEntityAsync(history);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // ===============================
                // 11️⃣ RETURN RESPONSE
                // ===============================
                var result = await _unitOfWork.TicketGenrationRepository
                    .GetByIdAsync(ticket.Id);

                return ApiResponse<GetTicketResponseDTO>
                    .Success(result, "Ticket created successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating ticket");
                throw;
            }
        }
    
        private async Task<string> GenerateTicketNumber()
        {
            return $"TKT-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
    }   
    */
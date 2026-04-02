using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers
{
    // ===============================
    // COMMAND
    // ===============================
    public class DeleteExperienceDocCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteRequestDTO DTO { get; set; }

        public DeleteExperienceDocCommand(DeleteRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ===============================
    // HANDLER
    // ===============================
    public class DeleteExperienceDocCommandHandler
        : IRequestHandler<DeleteExperienceDocCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteExperienceDocCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public DeleteExperienceDocCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteExperienceDocCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeleteExperienceDocCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🚀 DeleteExperienceDoc started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                if (request.DTO.Id <= 0)
                    throw new ValidationErrorException("Invalid DocumentId.");

                var loggedInEmployeeId = validation.UserEmployeeId;

                // ===============================
                // 2️⃣ FETCH DOCUMENT
                // ===============================
                var existing = await _unitOfWork.EmployeeExperienceDocumentRepository
                    .GetSingleByDetailIdAsync(request.DTO.Id);

                if (existing == null)
                {
                    _logger.LogWarning("⚠️ Doc not found | Id: {Id}", request.DTO.Id);
                    throw new ApiException("Document not found.", 404);
                }

                //// ===============================
                //// 3️⃣ OWNERSHIP CHECK
                //// ===============================
                //if (existing.EmployeeId != loggedInEmployeeId)
                //{
                //    _logger.LogWarning("⚠️ Unauthorized delete | Id: {Id} | User: {UserId}",
                //        request.DTO.Id, loggedInEmployeeId);

                //    throw new UnauthorizedAccessException("Unauthorized access.");
                //}

                // ===============================
                // 4️⃣ ALREADY DELETED CHECK
                // ===============================
                if (existing.IsSoftDeleted)
                {
                    _logger.LogWarning("⚠️ Already deleted | Id: {Id}", request.DTO.Id);
                    throw new ApiException("Document already deleted.", 400);
                }

                // ===============================
                // 5️⃣ SOFT DELETE
                // ===============================
                existing.IsSoftDeleted = true;
                existing.IsActive = false;
                existing.SoftDeletedById = loggedInEmployeeId;
                existing.UpdatedById = loggedInEmployeeId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                // ===============================
                // 6️⃣ UPDATE (NO SAVE HERE)
                // ===============================
                await  _unitOfWork.EmployeeExperienceDocumentRepository.SoftDeleteAsync(existing);

                // ===============================
                // 7️⃣ SAVE (ONLY HERE)
                // ===============================
                var rows = await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (rows <= 0)
                    throw new ApiException("Delete failed.", 500);

                // ===============================
                // 8️⃣ SUCCESS
                // ===============================
                _logger.LogInformation("✅ Doc deleted | Id: {Id} | User: {UserId}",
                    request.DTO.Id, loggedInEmployeeId);

                return ApiResponse<bool>.Success(true, "Document deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ DeleteExperienceDoc failed | Id: {Id}", request.DTO?.Id);
                throw;
            }
        }
    }
}
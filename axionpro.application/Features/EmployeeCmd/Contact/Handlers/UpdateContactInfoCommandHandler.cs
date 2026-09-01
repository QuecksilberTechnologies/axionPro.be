// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles UpdateContactInfoCommandHandler requests using authenticated tenant context.
// ================================================================

using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;


public class UpdateEmployeeContactCommand : IRequest<ApiResponse<bool>> 
       { 
      public UpdateContactRequestDTO DTO { get; set; } 
      public UpdateEmployeeContactCommand(UpdateContactRequestDTO dto) { DTO = dto; } 
       
       }

/// <summary>
/// Handles authenticated tenant requests for this feature.
/// </summary>
public class UpdateContactInfoCommandHandler
   : IRequestHandler<UpdateEmployeeContactCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateContactInfoCommandHandler> _logger;
    private readonly ICommonRequestService _commonRequestService;

    public UpdateContactInfoCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateContactInfoCommandHandler> logger,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _commonRequestService = commonRequestService;
    }

    public async Task<ApiResponse<bool>> Handle(
    UpdateEmployeeContactCommand request,
    CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("UpdateContact started");

            // ===============================
            // 1️⃣ VALIDATION
            // ===============================
            #region Tenant Request Validation
            var validation =
                await _commonRequestService.ValidateTenantUserRequestAsync();
            #endregion

            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            // ===============================
            // 2️⃣ NULL SAFETY
            // ===============================
            if (request?.DTO == null)
                throw new ValidationErrorException("Invalid request.");

            if (request.DTO.Id <= 0)
                throw new ValidationErrorException("Invalid contact id.");

            long loggedInEmployeeId = validation.UserEmployeeId;
            // 4️⃣ FETCH EXISTING
            // ===============================
            var existing =
                await _unitOfWork.EmployeeContactRepository
                    .GetSingleRecordAsync(request.DTO.Id, true);

            if (existing == null)
                throw new ApiException("Contact record not found.", 404);

            // ===============================
            // 5️⃣ OWNERSHIP CHECK
            // ===============================
            if (existing.EmployeeId != loggedInEmployeeId)
                throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);

            // ===============================
            // 6️⃣ START TRANSACTION
            // ===============================
            await _unitOfWork.BeginTransactionAsync();

            var dto = request.DTO;

            // ===============================
            // 7️⃣ APPLY UPDATES
            // ===============================

            if (!string.IsNullOrWhiteSpace(dto.ContactName))
                existing.ContactName = dto.ContactName.Trim();

            if (dto.ContactType.HasValue && dto.ContactType > 0)
                existing.ContactType = dto.ContactType.Value;

            if (dto.Relation.HasValue && dto.Relation > 0)
                existing.Relation = dto.Relation.Value;

            if (!string.IsNullOrWhiteSpace(dto.ContactNumber))
            {
                var clean = dto.ContactNumber.Trim();
                if (!Regex.IsMatch(clean, @"^[0-9]{10}$"))
                    throw new ValidationErrorException("Invalid contact number.");

                existing.ContactNumber = clean;
            }

            if (!string.IsNullOrWhiteSpace(dto.AlternateNumber))
            {
                var clean = dto.AlternateNumber.Trim();
                if (!Regex.IsMatch(clean, @"^[0-9]{10}$"))
                    throw new ValidationErrorException("Invalid alternate number.");

                existing.AlternateNumber = clean;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var email = dto.Email.Trim();
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new ValidationErrorException("Invalid email format.");

                existing.Email = email;
            }

            if (dto.IsPrimary.HasValue)
                existing.IsPrimary = dto.IsPrimary.Value;

            if (dto.CountryId.HasValue)
                existing.CountryId = dto.CountryId.Value;

            if (dto.StateId.HasValue)
                existing.StateId = dto.StateId.Value;

            if (dto.DistrictId.HasValue)
                existing.DistrictId = dto.DistrictId.Value;

            if (dto.HouseNo != null)
                existing.HouseNo = dto.HouseNo.Trim();

            if (dto.Street != null)
                existing.Street = dto.Street.Trim();

            if (dto.LandMark != null)
                existing.LandMark = dto.LandMark.Trim();

            if (dto.Address != null)
                existing.Address = dto.Address.Trim();

            if (dto.Description != null)
                existing.Description = dto.Description.Trim();

            // ===============================
            // 🧾 AUDIT
            // ===============================
            existing.UpdatedById = loggedInEmployeeId;
            existing.UpdatedDateTime = DateTime.UtcNow;

            // ===============================
            // 8️⃣ SAVE
            // ===============================
            var updated =
                await _unitOfWork.EmployeeContactRepository
                    .UpdateContactAsync(existing);

            if (!updated)
                throw new ApiException("Failed to update contact.", 500);

            // ===============================
            // 9️⃣ COMMIT
            // ===============================
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("UpdateContact success");

            return ApiResponse<bool>.Success(true, "Contact updated successfully.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            _logger.LogError(ex, "Error updating contact");

            throw; // 🚨 MUST
        }
    }
}

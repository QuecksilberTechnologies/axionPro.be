using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;


public class UpdateEmployeeContactCommand : IRequest<ApiResponse<bool>> 
       { 
      public UpdateContactRequestDTO DTO { get; set; } 
      public UpdateEmployeeContactCommand(UpdateContactRequestDTO dto) { DTO = dto; } 
       
       }

public class UpdateContactInfoCommandHandler
   : IRequestHandler<UpdateEmployeeContactCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateContactInfoCommandHandler> _logger;
    private readonly IPermissionService _permissionService;
    private readonly ICommonRequestService _commonRequestService;

    public UpdateContactInfoCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateContactInfoCommandHandler> logger,
        IPermissionService permissionService,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _permissionService = permissionService;
        _commonRequestService = commonRequestService;
    }

    public async Task<ApiResponse<bool>> Handle(
        UpdateEmployeeContactCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // 🔐 STEP 1: Common validation (same as Create/Get)
            var validation =
                await _commonRequestService.ValidateRequestAsync(
                    request.DTO.UserEmployeeId);

            if (!validation.Success)
                return ApiResponse<bool>.Fail(validation.ErrorMessage);

            long loggedInEmployeeId = validation.UserEmployeeId;

            // 🔎 STEP 2: Validate Row Id
            if (request.DTO.Id <= 0)
                return ApiResponse<bool>.Fail("Invalid contact id.");

            // 🔑 STEP 3: Permission check
            var permissions =
                await _permissionService.GetPermissionsAsync(validation.RoleId);

            if (!permissions.Contains("UpdateContactInfo"))
            {
                // optional hard stop
                // return ApiResponse<bool>.Fail("Permission denied.");
            }

            // 📦 STEP 4: Fetch existing contact
            var existing =
                await _unitOfWork.EmployeeContactRepository
                    .GetSingleRecordAsync(request.DTO.Id, true);

            if (existing == null)
                return ApiResponse<bool>.Fail("Contact record not found.");

            // 🔒 STEP 5: Ownership check
            if (existing.EmployeeId != loggedInEmployeeId)
                return ApiResponse<bool>.Fail("Unauthorized access.");

            // =====================================================
            // 🔄 APPLY PARTIAL UPDATES (DTO SAFE)
            // =====================================================

            // Contact Name
            if (!string.IsNullOrWhiteSpace(request.DTO.ContactName))
                existing.ContactName = request.DTO.ContactName.Trim();

            // Contact Type
            if (request.DTO.ContactType.HasValue && request.DTO.ContactType > 0)
                existing.ContactType = request.DTO.ContactType.Value;

            // Relation
            if (request.DTO.Relation.HasValue && request.DTO.Relation > 0)
                existing.Relation = request.DTO.Relation.Value;

            // Contact Number
            if (!string.IsNullOrWhiteSpace(request.DTO.ContactNumber))
            {
                var clean = request.DTO.ContactNumber.Trim();
                if (!Regex.IsMatch(clean, @"^[0-9]{10}$"))
                    return ApiResponse<bool>.Fail("Invalid contact number.");

                existing.ContactNumber = clean;
            }

            // Alternate Number
            if (!string.IsNullOrWhiteSpace(request.DTO.AlternateNumber))
            {
                var clean = request.DTO.AlternateNumber.Trim();
                if (!Regex.IsMatch(clean, @"^[0-9]{10}$"))
                    return ApiResponse<bool>.Fail("Invalid alternate number.");

                existing.AlternateNumber = clean;
            }

            // Email
            if (!string.IsNullOrWhiteSpace(request.DTO.Email))
            {
                if (!Regex.IsMatch(request.DTO.Email.Trim(),
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    return ApiResponse<bool>.Fail("Invalid email format.");

                existing.Email = request.DTO.Email.Trim();
            }

            // 🔥 SINGLE PRIMARY RULE
            if (request.DTO.IsPrimary.HasValue)
            {
                existing.IsPrimary = request.DTO.IsPrimary.Value;
            }

            // Address IDs
            if (request.DTO.CountryId.HasValue)
                existing.CountryId = request.DTO.CountryId.Value;

            if (request.DTO.StateId.HasValue)
                existing.StateId = request.DTO.StateId.Value;

            if (request.DTO.DistrictId.HasValue)
                existing.DistrictId = request.DTO.DistrictId.Value;

            // Address text
            if (request.DTO.HouseNo != null)
                existing.HouseNo = request.DTO.HouseNo.Trim();

            if (request.DTO.Street != null)
                existing.Street = request.DTO.Street.Trim();

            if (request.DTO.LandMark != null)
                existing.LandMark = request.DTO.LandMark.Trim();

            if (request.DTO.Address != null)
                existing.Address = request.DTO.Address.Trim();

            if (request.DTO.Description != null)
                existing.Description = request.DTO.Description.Trim();

            // 🧾 AUDIT
            existing.UpdatedById = loggedInEmployeeId;
            existing.UpdatedDateTime = DateTime.UtcNow;

            // 💾 SAVE
            bool updated =
                await _unitOfWork.EmployeeContactRepository
                    .UpdateContactAsync(existing);

            if (!updated)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.Fail("Failed to update contact.");
            }

            await _unitOfWork.CommitTransactionAsync();
            return ApiResponse<bool>.Success(true, "Contact updated successfully.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating contact");

            return ApiResponse<bool>.Fail(
                "Unexpected error occurred.",
                new List<string> { ex.Message });
        }
    }
}

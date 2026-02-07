using axionpro.application.DTOs.PolicyType;
using axionpro.application.Features.PolicyTypeCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    // ================================
    // 🔹 COMMAND
    // ================================
    public class DeletePolicyTypeCommand : IRequest<ApiResponse<bool>>
    {
        public DeletePolicyTypeDTO DTO { get; set; }

        public DeletePolicyTypeCommand(DeletePolicyTypeDTO dto)
        {
            DTO = dto;
        }
    }

    // ================================
    // 🔹 HANDLER
    // ================================
    public class DeletePolicyTypeCommandHandler
        : IRequestHandler<DeletePolicyTypeCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;

        public DeletePolicyTypeCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeletePolicyTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 STEP 1: Basic validation
                if (request.DTO == null || request.DTO.PolicyId <= 0)
                {
                    return ApiResponse<bool>
                        .Fail("Invalid request. PolicyId is required.");
                }

                // 🔐 STEP 2: Common validation (Tenant / User info)
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // 🔄 STEP 3: Begin transaction
                await _unitOfWork.BeginTransactionAsync();

                // 🔍 STEP 4: Check PolicyType exists or not
                var policyType =
                    await _unitOfWork.PolicyTypeRepository
                        .GetPolicyTypeByIdAsync(request.DTO.PolicyId);

                if (policyType == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>
                        .Fail("Policy type not found.");
                }

                // 🗑️ STEP 5: Soft delete PolicyType (UPDATE only)
                policyType.IsSoftDelete = true;
                policyType.IsActive = false;
                policyType.SoftDeleteById = validation.UserEmployeeId;
                policyType.SoftDeleteDateTime = DateTime.UtcNow;

                bool policyUpdated =
                    await _unitOfWork.PolicyTypeRepository
                        .UpdatePolicyTypeAsync(policyType);

                if (!policyUpdated)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>
                        .Fail("Policy type deletion failed.");
                }

                // 🗑️ STEP 6: Soft delete related CompanyPolicyDocuments
                // NOTE: PolicyTypeId ke basis par sab documents update honge
                var companyDocs = await _unitOfWork.CompanyPolicyDocumentRepository .GetByIdAsync( request.DTO.PolicyId, validation.TenantId, true);

                if (companyDocs != null)
                {
                    companyDocs.IsSoftDeleted = true;
                    companyDocs.IsActive = false;
                    companyDocs.SoftDeletedById = validation.UserEmployeeId;
                    companyDocs.SoftDeletedDateTime = DateTime.UtcNow;

                    await _unitOfWork.CompanyPolicyDocumentRepository
                        .UpdateAsync(companyDocs);
                }

                // ✅ STEP 7: Commit transaction
                await _unitOfWork.CommitAsync();

                return ApiResponse<bool>
                    .Success(true, "Policy type and related documents deleted successfully.");
            }
            catch (Exception ex)
            {
                // ❌ Any failure → rollback
                await _unitOfWork.RollbackTransactionAsync();

                return ApiResponse<bool>
                    .Fail($"An error occurred while deleting policy type: {ex.Message}");
            }
        }
    }
}

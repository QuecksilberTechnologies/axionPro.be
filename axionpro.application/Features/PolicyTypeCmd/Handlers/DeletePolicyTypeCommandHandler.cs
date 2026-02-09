using axionpro.application.DTOs.PolicyType;
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
                // --------------------------------------------------
                // 1️⃣ Basic validation
                // --------------------------------------------------
                if (request.DTO == null || request.DTO.PolicyId <= 0)
                {
                    return ApiResponse<bool>
                        .Fail("Invalid request. PolicyId is required.");
                }

                // --------------------------------------------------
                // 2️⃣ Common validation (Tenant / User)
                // --------------------------------------------------
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                await _unitOfWork.BeginTransactionAsync();

                // --------------------------------------------------
                // 3️⃣ Fetch PolicyType
                // --------------------------------------------------
                var policyType =
                    await _unitOfWork.PolicyTypeRepository.GetPolicyTypeByIdAsync(request.DTO.PolicyId, null);

                if (policyType == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Policy type not found.");
                }

                // --------------------------------------------------
                // 4️⃣ Soft delete PolicyType
                // --------------------------------------------------
                policyType.IsSoftDelete = true;
                policyType.IsActive = false;
                policyType.SoftDeleteById = validation.UserEmployeeId;
                policyType.SoftDeleteDateTime = DateTime.UtcNow;

                // --------------------------------------------------
                // 4️⃣ Soft delete PolicyType (MANDATORY)
                // --------------------------------------------------
                var policyDeleted =
                    await _unitOfWork.PolicyTypeRepository
                        .SoftDeletePolicyTypeAsync(policyType);

                if (!policyDeleted)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>
                        .Fail("Policy type deletion failed.");
                }

                // --------------------------------------------------
                // 5️⃣ Soft delete related CompanyPolicyDocuments (OPTIONAL)
                // --------------------------------------------------
                var docsDeleted =
                    await _unitOfWork.CompanyPolicyDocumentRepository
                        .SoftDeleteByPolicyTypeIdAsync(
                            policyType.Id,
                            validation.UserEmployeeId);

                // ❗ Docs optional hain → failure pe rollback NA karo
                if (!docsDeleted)
                {
                    // 🔹 Log only (no rollback)
                   
                }

                // --------------------------------------------------
                // 6️⃣ Commit (ONLY depends on PolicyType)
                // --------------------------------------------------
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<bool>
                    .Success(true, "Policy type deleted successfully.");

               
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                return ApiResponse<bool>
                    .Fail($"An error occurred while deleting policy type: {ex.Message}");
            }
        }

    }

}



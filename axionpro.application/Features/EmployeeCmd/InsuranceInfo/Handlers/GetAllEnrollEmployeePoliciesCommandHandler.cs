using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.EnrolledPolicy;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.InsuranceInfo.Handlers
{
    public class GetAllEnrollEmployeePoliciesCommand : IRequest<ApiResponse<GetAllEnrolledEmployeeResponseDTO>>
    {
        public GetEnrolledEmployeeRequestDTO DTO { get; set; }

        public GetAllEnrollEmployeePoliciesCommand(GetEnrolledEmployeeRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class GetAllEnrolledEmployeeCommandHandler
   : IRequestHandler<GetAllEnrollEmployeePoliciesCommand, ApiResponse<GetAllEnrolledEmployeeResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ILogger<GetAllEnrolledEmployeeCommandHandler> _logger;

        public GetAllEnrolledEmployeeCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService,
            ILogger<GetAllEnrolledEmployeeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _logger = logger;
        }

        public async Task<ApiResponse<GetAllEnrolledEmployeeResponseDTO>> Handle(
     GetAllEnrollEmployeePoliciesCommand request,
     CancellationToken cancellationToken)
        {
            try
            {
                // ===============================
                // 🔐 AUTH VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 🔥 GET ENROLLMENTS
                // ===============================
                var enrollments = await _unitOfWork
                    .EmployeePolicyEnrollmentRepository
                    .GetByEmployeeIdAsync(validation.UserEmployeeId, validation.TenantId);

                var policies = new List<GetEmployeeEnrolledResponseDTO>();

                foreach (var enr in enrollments)
                {
                    // 🔹 GET DEPENDENTS (CORRECT METHOD)
                    var mappings = await _unitOfWork
                        .EmployeeDependentInsuranceMappingRepository
                        .GetByEnrollmentIdAsync(enr.Id, validation.TenantId);

                    var dependents = mappings.Select(d => new GetEmployeeDependentResponsePolicyDTO
                    {
                        Id = d.Id,
                        DependentId = d.DependentId, // ✅ FIXED
                        Relation = d.RelationType,
                        IsCovered = d.IsCovered
                    }).ToList();

                    policies.Add(new GetEmployeeEnrolledResponseDTO
                    {
                        Id = enr.Id,
                        EmployeeId = request.DTO.EmployeeId,
                        PolicyTypeId = enr.PolicyTypeId,
                        InsurancePolicyId = enr.InsurancePolicyId,
                        HasDependent = enr.HasDependent,
                        StartDate = enr.StartDate,
                        EndDate = enr.EndDate,
                        Dependents = dependents
                    });
                }

                // ===============================
                // 🔥 FINAL RESPONSE
                // ===============================
                var response = new GetAllEnrolledEmployeeResponseDTO
                {
                    EmployeeId = request.DTO.EmployeeId,
                    Policies = policies
                };

                return ApiResponse<GetAllEnrolledEmployeeResponseDTO>
                    .Success(response, "Employee policies fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching employee policies");
                throw;
            }
        }
    }

}
 
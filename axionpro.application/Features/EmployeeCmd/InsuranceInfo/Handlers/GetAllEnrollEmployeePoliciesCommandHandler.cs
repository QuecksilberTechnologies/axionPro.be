using axionpro.application.Common.Helpers.PercentageHelper;
using axionpro.application.Common.Helpers.RequestHelper;
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
                var validation = await _commonRequestService.ValidateTenantUserRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                    request.DTO.EmployeeId,
                    validation.Claims.TenantEncriptionKey,
                    _idEncoderService);

                if (employeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                if (!await _commonRequestService.CanAccessEmployeeDataAsync(
                        validation,
                        employeeId,
                        EmployeeDataAccessRequirement.PersonalDetails,
                        cancellationToken))
                    throw new ForbiddenAccessException("Employee insurance access denied.");

                // ===============================
                // 🔥 GET ENROLLMENTS
                // ===============================
                var enrollments = await _unitOfWork
                    .EmployeePolicyEnrollmentRepository
                    .GetByEmployeeIdAsync(employeeId, validation.TenantId);

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

                var apiResponse = ApiResponse<GetAllEnrolledEmployeeResponseDTO>
                    .Success(response, "Employee policies fetched successfully.");
                apiResponse.CompletionPercentage = policies.Count == 0
                    ? 0
                    : Math.Round(policies.Average(policy =>
                        EmployeeProfileCompletionCalculator.CalculateInsuranceRow(
                            policy.PolicyTypeId,
                            policy.InsurancePolicyId,
                            policy.StartDate,
                            policy.EndDate)), 0);

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching employee policies");
                throw;
            }
        }
    }

}
 

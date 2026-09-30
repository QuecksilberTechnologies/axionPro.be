using axionpro.application.DTOs.Module;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.EnrolledPolicy;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.Exceptions;
using axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace axionpro.application.Features.EmployeeCmd.InsuranceInfo.Handlers
{
    public class CreateEmployeeInsuranceEnrollCommand : IRequest<ApiResponse<GetEmployeeEnrolledResponseDTO>>
    {
        public CreateEmployeeEnrolledRequestDTO DTO { get; set; }

        public CreateEmployeeInsuranceEnrollCommand(CreateEmployeeEnrolledRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class CreateEmployeeInsuranceEnrollCommandHandler
    : IRequestHandler<CreateEmployeeInsuranceEnrollCommand, ApiResponse<GetEmployeeEnrolledResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateEmployeeInsuranceEnrollCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IIdEncoderService _idEncoderService;

        public CreateEmployeeInsuranceEnrollCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateEmployeeInsuranceEnrollCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IFileStorageService fileStorageService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _fileStorageService = fileStorageService;
            _idEncoderService = idEncoderService;
        }
        public async Task<ApiResponse<GetEmployeeEnrolledResponseDTO>> Handle(
     CreateEmployeeInsuranceEnrollCommand request,
     CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 CreateEmployeeInsuranceEnroll started");

                // ===============================
                // 🔐 STEP 1: AUTH VALIDATION
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

                if (request.DTO.PolicyTypeId <= 0)
                    throw new ValidationErrorException("Policy type is required.");

                if (request.DTO.InsurancePolicyId <= 0)
                    throw new ValidationErrorException("Insurance policy is required.");

                EmployeeProfileValidationHelper.ValidateDateRange(
                    request.DTO.StartDate,
                    request.DTO.EndDate);

                if (request.DTO.HasDependent &&
                    (request.DTO.Dependents == null || request.DTO.Dependents.Count == 0))
                    throw new ValidationErrorException("Select at least one dependent.");

                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 🔥 STEP 2: ENROLLMENT (INSERT OR USE EXISTING)
                // ===============================
                EmployeePolicyEnrollment createdEnrollment;

                var existingEnrollment = request.DTO.EmployeeInsuranceEnrollmentId.HasValue
                    ? await _unitOfWork.EmployeePolicyEnrollmentRepository.GetByIdAsync(
                        request.DTO.EmployeeInsuranceEnrollmentId.Value,
                        validation.TenantId)
                    : await _unitOfWork.EmployeePolicyEnrollmentRepository.GetExistingAsync(
                        employeeId,
                        request.DTO.PolicyTypeId,
                        request.DTO.InsurancePolicyId,
                        validation.TenantId);

                if (existingEnrollment != null)
                {
                    if (existingEnrollment.EmployeeId != employeeId)
                        throw new ForbiddenAccessException("Employee insurance access denied.");

                    createdEnrollment = existingEnrollment;

                    createdEnrollment.PolicyTypeId = request.DTO.PolicyTypeId;
                    createdEnrollment.InsurancePolicyId = request.DTO.InsurancePolicyId;
                    createdEnrollment.HasDependent = request.DTO.HasDependent;
                    createdEnrollment.StartDate = request.DTO.StartDate;
                    createdEnrollment.EndDate = request.DTO.EndDate;
                    createdEnrollment.UpdatedById = validation.UserEmployeeId;
                    createdEnrollment.UpdatedDateTime = DateTime.UtcNow;

                    await _unitOfWork.EmployeePolicyEnrollmentRepository
                        .UpdateAsync(createdEnrollment);

                    _logger.LogInformation("⚠️ Enrollment already exists. Using existing record.");
                }
                else
                {
                    createdEnrollment = new EmployeePolicyEnrollment
                    {
                        TenantId = validation.TenantId,
                        EmployeeId = employeeId,
                        PolicyTypeId = request.DTO.PolicyTypeId,
                        InsurancePolicyId = request.DTO.InsurancePolicyId,
                        HasDependent = request.DTO.HasDependent,
                        StartDate = request.DTO.StartDate,
                        EndDate = request.DTO.EndDate,
                        IsActive = true,
                        IsSoftDeleted = false,
                        AddedById = validation.UserEmployeeId,
                        AddedDateTime = DateTime.UtcNow
                    };

                    await _unitOfWork.EmployeePolicyEnrollmentRepository.AddAsync(createdEnrollment);
                     

                    _logger.LogInformation("✅ New enrollment inserted successfully");
                }

                // ===============================
                // 🔥 STEP 3: DEPENDENT MAPPING
                // ===============================
                List<GetEmployeeDependentResponsePolicyDTO> dependentList = new();

                var existingMappings = await _unitOfWork
                    .EmployeeDependentInsuranceMappingRepository
                    .GetByEnrollmentIdAsync(createdEnrollment.Id, validation.TenantId);

                if (existingMappings.Count > 0)
                {
                    foreach (var mapping in existingMappings)
                    {
                        mapping.IsActive = false;
                        mapping.IsSoftDeleted = true;
                        mapping.SoftDeletedById = validation.UserEmployeeId;
                        mapping.DeletedDateTime = DateTime.UtcNow;
                    }

                    await _unitOfWork.EmployeeDependentInsuranceMappingRepository
                        .SoftDeleteByEnrollmentIdAsync(existingMappings);
                }

                var requestedDependents = request.DTO.HasDependent
                    ? request.DTO.Dependents ?? new List<CreateEmployeeDependentRequestPolicyDTO>()
                    : new List<CreateEmployeeDependentRequestPolicyDTO>();

                if (requestedDependents.Count > 0)
                {
                    var mappings = requestedDependents.Select(dependent =>
                        new EmployeePolicyDependentMapping
                        {
                            TenantId = validation.TenantId,
                            EmployeePolicyEnrollmentId = createdEnrollment.Id,
                            DependentId = dependent.DependentId,
                            RelationType = dependent.Relation,
                            IsCovered = dependent.IsCovered,
                            IsActive = true,
                            IsSoftDeleted = false,
                            AddedById = validation.UserEmployeeId,
                            AddedDateTime = DateTime.UtcNow
                        }).ToList();

                    await _unitOfWork.EmployeeDependentInsuranceMappingRepository
                        .AddRangeAsync(mappings);

                    dependentList = mappings.Select(mapping =>
                        new GetEmployeeDependentResponsePolicyDTO
                        {
                            Id = mapping.Id,
                            DependentId = mapping.DependentId,
                            Relation = mapping.RelationType,
                            IsCovered = mapping.IsCovered
                        }).ToList();
                }

                await _unitOfWork.CommitTransactionAsync();

                // ===============================
                // 📤 FINAL RESPONSE
                // ===============================
                var response = new GetEmployeeEnrolledResponseDTO
                {
                    Id = createdEnrollment.Id,
                    EmployeeId = request.DTO.EmployeeId,
                    PolicyTypeId = createdEnrollment.PolicyTypeId,
                    InsurancePolicyId = createdEnrollment.InsurancePolicyId,
                    HasDependent = createdEnrollment.HasDependent,
                    StartDate = createdEnrollment.StartDate,
                    EndDate = createdEnrollment.EndDate,
                    Dependents = dependentList
                };

                _logger.LogInformation("✅ Enrollment process completed");

                return ApiResponse<GetEmployeeEnrolledResponseDTO>
                    .Success(response, "Policy enrollment processed successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "❌ Critical failure in enrollment");
                throw;
            }
        }

    }

    }


 


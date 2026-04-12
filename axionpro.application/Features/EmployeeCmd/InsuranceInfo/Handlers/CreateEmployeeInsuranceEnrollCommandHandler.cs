using axionpro.application.DTOs.Module;
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
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                // ===============================
                // 🔥 STEP 2: ENROLLMENT (INSERT OR USE EXISTING)
                // ===============================
                EmployeePolicyEnrollment createdEnrollment;

                var existingEnrollment = await _unitOfWork
                    .EmployeePolicyEnrollmentRepository.GetExistingAsync(
                        validation.UserEmployeeId,
                        request.DTO.PolicyTypeId,
                        request.DTO.InsurancePolicyId,
                        validation.TenantId);

                if (existingEnrollment != null)
                {
                    createdEnrollment = existingEnrollment;

                    _logger.LogInformation("⚠️ Enrollment already exists. Using existing record.");
                }
                else
                {
                    createdEnrollment = new EmployeePolicyEnrollment
                    {
                        TenantId = validation.TenantId,
                        EmployeeId = validation.UserEmployeeId,
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

                if (request.DTO.HasDependent && request.DTO.Dependents != null)
                {
                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();

                        var existingMappings = await _unitOfWork
                            .EmployeeDependentInsuranceMappingRepository
                            .GetByEnrollmentIdAsync(createdEnrollment.Id, validation.TenantId);

                        var existingDependentIds = existingMappings
                            .Select(x => x.DependentId)
                            .ToHashSet();

                        var newDependents = request.DTO.Dependents
                            .Where(d => !existingDependentIds.Contains(d.DependentId))
                            .ToList();

                        // 🔥 INSERT NEW MAPPINGS
                        if (newDependents.Any())
                        {
                            var mappings = newDependents.Select(dep => new EmployeePolicyDependentMapping
                            {
                                TenantId = validation.TenantId,
                                EmployeePolicyEnrollmentId = createdEnrollment.Id,
                                DependentId = dep.DependentId,
                                RelationType = dep.Relation,
                                IsCovered = true,
                                IsActive = true,
                                IsSoftDeleted = false,
                                AddedById = validation.UserEmployeeId,
                                AddedDateTime = DateTime.UtcNow
                            }).ToList();

                            await _unitOfWork.EmployeeDependentInsuranceMappingRepository
                                .AddRangeAsync(mappings);
                        }

                        // 🔥 UPDATE DEPENDENTS
                        var dependentIdsToUpdate = newDependents
                            .Select(d => d.DependentId)
                            .ToList();

                        if (dependentIdsToUpdate.Any())
                        {
                            var dependents = await _unitOfWork.EmployeeDependentRepository
                                .GetBulkInfo(dependentIdsToUpdate);

                            var updateList = dependents.Select(d => new EmployeeDependent
                            {
                                Id = d.Id,
                                IsCoveredInPolicy = true,
                                UpdatedById = validation.UserEmployeeId,
                                UpdatedDateTime = DateTime.UtcNow
                            }).ToList();

                            if (updateList.Any())
                            {
                                await _unitOfWork.EmployeeDependentRepository
                                    .UpdateAsyncRangeAsync(updateList);
                            }
                        }

                        await _unitOfWork.CommitTransactionAsync();

                        // 🔥 FINAL FETCH
                        var finalMappings = await _unitOfWork
                            .EmployeeDependentInsuranceMappingRepository
                            .GetByEnrollmentIdAsync(createdEnrollment.Id, validation.TenantId);

                        dependentList = finalMappings.Select(d => new GetEmployeeDependentResponsePolicyDTO
                        {
                            Id = d.Id,
                            DependentId = d.DependentId,
                            Relation = d.RelationType,
                            IsCovered = d.IsCovered
                        }).ToList();
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackTransactionAsync();

                        _logger.LogError(ex, "⚠️ Dependent mapping failed but enrollment saved");
                    }
                }

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
                _logger.LogError(ex, "❌ Critical failure in enrollment");
                throw;
            }
        }

    }

    }


 
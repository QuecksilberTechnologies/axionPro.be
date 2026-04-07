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
        public async Task<ApiResponse<GetEmployeeEnrolledResponseDTO>> Handle( CreateEmployeeInsuranceEnrollCommand request, CancellationToken cancellationToken)
        {
            string? uploadedFileKey = null;

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
                // 🔐 STEP 2: PERMISSION CHECK
                // ===============================
                //var hasAccess = await _unitOfWork.PermissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.EmployeeInsurance,
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 🔥 STEP 3: ENROLLMENT SAVE (FIRST COMMIT)
                // ===============================
                EmployeePolicyEnrollment createdEnrollment = null;

                // 🔥 CHECK EXISTING
                var existingEnrollment = await _unitOfWork
                    .EmployeePolicyEnrollmentRepository.GetExistingAsync( validation.UserEmployeeId,request.DTO.PolicyTypeId,request.DTO.InsurancePolicyId,validation.TenantId);

                if (existingEnrollment != null)
                {
                    // ✅ USE EXISTING (NO INSERT)
                    createdEnrollment = existingEnrollment;

                    _logger.LogInformation("⚠️ Enrollment already exists. Skipping insert.");
                }

                // ===============================
                // 🔥 STEP 4: DEPENDENT SAVE (SECOND COMMIT - OPTIONAL)
                // ===============================
                List<GetEmployeeDependentResponsePolicyDTO> dependentList = new();

                if (request.DTO.HasDependent && request.DTO.Dependents != null)
                {
                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();

                        // ===============================
                        // 🔥 STEP 1: EXISTING MAPPINGS FETCH
                        // ===============================
                        var existingMappings = await _unitOfWork
                            .EmployeeDependentInsuranceMappingRepository
                            .GetByEnrollmentIdAsync(createdEnrollment.Id, validation.TenantId);

                        var existingDependentIds = existingMappings
                            .Select(x => x.DependentId)
                            .ToHashSet();   // 🔥 FAST LOOKUP

                        // ===============================
                        // 🔥 STEP 2: FILTER ONLY NEW DEPENDENTS
                        // ===============================
                        var newDependents = request.DTO.Dependents
                            .Where(d => !existingDependentIds.Contains(d.DependentId))
                            .ToList();

                        if (newDependents.Any())
                        {
                            // ===============================
                            // 🔥 STEP 3: INSERT ONLY NEW
                            // ===============================
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

                        // ===============================
                        // 🔥 STEP 4: UPDATE DEPENDENTS (ONLY NEW ONES)
                        // ===============================
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

                        // ===============================
                        // 🔹 RESPONSE BUILD (ALL DEPENDENTS)
                        // ===============================
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


 
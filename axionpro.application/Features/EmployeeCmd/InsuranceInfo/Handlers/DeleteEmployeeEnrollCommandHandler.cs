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
    public class DeleteEmployeeEnrollCommand  : IRequest<ApiResponse<bool>>
    {
        public DeleteEnrolledEmployeePolicyRequestDTO DTO { get; set; }

        public DeleteEmployeeEnrollCommand(DeleteEnrolledEmployeePolicyRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class DeleteEmployeeEnrollCommandHandler
  : IRequestHandler<DeleteEmployeeEnrollCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteEmployeeEnrollCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public DeleteEmployeeEnrollCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteEmployeeEnrollCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeleteEmployeeEnrollCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 DeleteEmployeeEnroll started");

                // ===============================
                // 🔐 AUTH VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                // ===============================
                // 🔐 PERMISSION CHECK
                // ===============================
                //var hasAccess = await _unitOfWork.PermissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.EmployeeInsurance,
                //    Operations.Delete);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 🔥 CASE 1: DELETE BY EMPLOYEE (FULL DELETE)
                // ===============================
                if (!string.IsNullOrWhiteSpace(request.DTO.EmployeeId))
                {
                    var employeeId =  validation.UserEmployeeId;

                    var enrollments = await _unitOfWork
                        .EmployeePolicyEnrollmentRepository
                        .GetByEmployeeIdAsync(employeeId, validation.TenantId);

                    foreach (var enr in enrollments)
                    {
                        // 🔹 GET DEPENDENT MAPPINGS
                        var mappings = await _unitOfWork
                            .EmployeeDependentInsuranceMappingRepository
                            .GetByEnrollmentIdAsync(enr.Id, validation.TenantId);

                        // 🔹 UPDATE DEPENDENT TABLE
                        var dependentIds = mappings.Select(x => x.DependentId).ToList();

                        if (dependentIds.Any())
                        {
                            var dependents = await _unitOfWork
                                .EmployeeDependentRepository
                                .GetBulkInfo(dependentIds);

                            var updateList = dependents.Select(d => new EmployeeDependent
                            {
                                Id = d.Id,
                                IsCoveredInPolicy = false,
                                UpdatedById = validation.UserEmployeeId,
                                UpdatedDateTime = DateTime.UtcNow
                            }).ToList();

                            await _unitOfWork.EmployeeDependentRepository
                                .UpdateAsyncRangeAsync(updateList);
                        }

                        // 🔹 SOFT DELETE MAPPING
                        foreach (var map in mappings)
                        {
                            map.IsActive = false;
                            map.IsSoftDeleted = true;
                            map.SoftDeletedById = validation.UserEmployeeId;
                            map.DeletedDateTime = DateTime.UtcNow;
                        }

                        await _unitOfWork.EmployeeDependentInsuranceMappingRepository
                            .SoftDeleteByEnrollmentIdAsync(mappings);

                        // 🔹 SOFT DELETE ENROLLMENT
                        enr.IsActive = false;
                        enr.IsSoftDeleted = true;
                        enr.SoftDeletedById = validation.UserEmployeeId;
                        enr.DeletedDateTime = DateTime.UtcNow;

                        await _unitOfWork.EmployeePolicyEnrollmentRepository
                            .UpdateAsync(enr);
                    }
                }

                // ===============================
                // 🔥 CASE 2: DELETE SINGLE ENROLLMENT
                // ===============================
                else if (request.DTO.EmployeeInsuranceMappingId.HasValue)
                {
                    var enr = await _unitOfWork
                        .EmployeePolicyEnrollmentRepository
                        .GetByIdAsync(request.DTO.EmployeeInsuranceMappingId.Value, validation.TenantId);

                    if (enr == null)
                        throw new ApiException("Enrollment not found.",404);

                    // 🔹 GET DEPENDENTS
                    var mappings = await _unitOfWork
                        .EmployeeDependentInsuranceMappingRepository
                        .GetByEnrollmentIdAsync(enr.Id, validation.TenantId);

                    var dependentIds = mappings.Select(x => x.DependentId).ToList();

                    if (dependentIds.Any())
                    {
                        var dependents = await _unitOfWork
                            .EmployeeDependentRepository
                            .GetBulkInfo(dependentIds);

                        var updateList = dependents.Select(d => new EmployeeDependent
                        {
                            Id = d.Id,
                            IsCoveredInPolicy = false,
                            UpdatedById = validation.UserEmployeeId,
                            UpdatedDateTime = DateTime.UtcNow
                        }).ToList();

                        await _unitOfWork.EmployeeDependentRepository
                            .UpdateAsyncRangeAsync(updateList);
                    }

                    // 🔹 SOFT DELETE MAPPINGS
                    foreach (var map in mappings)
                    {
                        map.IsActive = false;
                        map.IsSoftDeleted = true;
                        map.SoftDeletedById = validation.UserEmployeeId;
                        map.DeletedDateTime = DateTime.UtcNow;
                    }

                    await _unitOfWork.EmployeeDependentInsuranceMappingRepository
                        .SoftDeleteByEnrollmentIdAsync(mappings);

                    // 🔹 SOFT DELETE ENROLLMENT
                    enr.IsActive = false;
                    enr.IsSoftDeleted = true;
                    enr.SoftDeletedById = validation.UserEmployeeId;
                    enr.DeletedDateTime = DateTime.UtcNow;

                    await _unitOfWork.EmployeePolicyEnrollmentRepository
                        .UpdateAsync(enr);
                }

                else
                {
                    throw new ValidationErrorException("Provide EmployeeId or EnrollmentId.");
                }

                // ===============================
                // ✅ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Delete successful");

                return ApiResponse<bool>.Success(true, "Deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ DeleteEmployeeEnroll failed");

                throw;
            }
        }
    }

}


 
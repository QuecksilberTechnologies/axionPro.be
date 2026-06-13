using axionpro.application.DTOS.Compliances.ComplianceRule;
using axionpro.application.Exceptions;
using axionpro.application.Features.ComplianceCmd;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Features.ComplianceCmd
{
    public class UpdateComplianceRuleCommand : IRequest<ApiResponse<UpdateComplianceRuleReponseDTO>>
    {
        public required UpdateComplianceRuleRequestDTO DTO { get; set; }
    }
}
    //public class UpdateComplianceRuleCommandHandler: IRequestHandler<UpdateComplianceRuleCommand, ApiResponse<UpdateComplianceRuleReponseDTO>>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ILogger<UpdateComplianceRuleCommandHandler> _logger;
    //    private readonly ICommonRequestService _commonRequestService;

    //    public UpdateComplianceRuleCommandHandler(
    //        IUnitOfWork unitOfWork,
    //        ILogger<UpdateComplianceRuleCommandHandler> logger,
    //        ICommonRequestService commonRequestService)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _logger = logger;
    //        _commonRequestService = commonRequestService;
    //    }

    //    public async Task<ApiResponse<UpdateComplianceRuleReponseDTO>> Handle(
    //        UpdateComplianceRuleCommand request,
    //        CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            var validation = await _commonRequestService.ValidateRequestAsync();
    //            if (!validation.Success)
    //                throw new UnauthorizedAccessException(validation.ErrorMessage);

    //            if (request?.DTO == null)
    //                throw new ValidationErrorException("Invalid request");

              

    //            var entity = await _unitOfWork.CompilanceRuleRepository
    //                .GetByIdAsync(id);

    //            if (entity == null)
    //                throw new NotFoundException("Compliance rule not found");

    //            // 🔥 Update fields
    //            entity.ComplianceTypeId = dto.ComplianceTypeId;
    //            entity.CountryId = dto.CountryId;
    //            entity.StateId = dto.StateId;
    //            entity.RuleJson = dto.RuleJson;
    //            entity.Priority = dto.Priority;
    //            entity.TenantId = dto.TenantId;
    //            entity.EffectiveFrom = dto.EffectiveFrom;
    //            entity.EffectiveTo = dto.EffectiveTo;
    //            entity.IsActive = dto.IsActive;

    //            entity.UpdatedById = validation.UserEmployeeId;
    //            entity.UpdatedDateTime = DateTime.UtcNow;

    //            await _unitOfWork.ComplianceRuleRepository.UpdateAsync(entity);
    //            await _unitOfWork.SaveChangesAsync();

    //            var response = new UpdateComplianceRuleResponseDTO
    //            {
    //                Id = dto.Id,
    //                ComplianceTypeId = entity.ComplianceTypeId,
    //                CountryId = entity.CountryId,
    //                StateId = entity.StateId,
    //                RuleJson = entity.RuleJson,
    //                Priority = entity.Priority,
    //                TenantId = entity.TenantId,
    //                EffectiveFrom = entity.EffectiveFrom,
    //                EffectiveTo = entity.EffectiveTo,
    //                IsActive = entity.IsActive
    //            };

    //            return ApiResponse<UpdateComplianceRuleResponseDTO>.Success(response);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error updating ComplianceRule");
    //            throw;
    //        }
    //    }
    //}


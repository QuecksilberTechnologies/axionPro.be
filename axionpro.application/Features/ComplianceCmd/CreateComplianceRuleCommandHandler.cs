using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Compliances.ComplianceRule;
using axionpro.application.Exceptions;
using axionpro.application.Features.PolicyTypeCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace axionpro.application.Features.ComplianceCmd
{
    public class CreateComplianceRuleCommand
        : IRequest<ApiResponse<GetComplianceRuleResponseDTO>>
    {
        public CreateComplianceRuleRequestDTO DTO { get; set; }

        public CreateComplianceRuleCommand(CreateComplianceRuleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateComplianceRuleCommandHandler
        : IRequestHandler<CreateComplianceRuleCommand, ApiResponse<GetComplianceRuleResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateComplianceRuleCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public CreateComplianceRuleCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateComplianceRuleCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetComplianceRuleResponseDTO>> Handle(
            CreateComplianceRuleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request");

                var dto = request.DTO;

                dto.Prop ??= new ExtraPropRequestDTO();
                dto.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ BUSINESS VALIDATION
                // ===============================

                //if (dto.EffectiveTo != null && dto.EffectiveFrom > dto.EffectiveTo)
                //    throw new ValidationErrorException("EffectiveFrom cannot be greater than EffectiveTo");

                bool exists = await _unitOfWork.CompilanceRuleRepository
                    .ExistsAsync(
                        dto.ComplianceTypeId,
                        dto.CountryId,
                        dto.StateId,
                        dto.Prop.TenantId,
                        dto.EffectiveFrom);

                if (exists)
                    throw new ValidationErrorException("Compliance rule already exists for same date");
               // dto.EffectiveFrom.AddDays(-1);
                // ===============================
                // 3️⃣ TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // 🔥 CLOSE OLD RULE (VERSIONING)
                var existingRule = await _unitOfWork.CompilanceRuleRepository
                    .GetRuleAsync(
                        dto.ComplianceTypeId,
                        dto.CountryId,
                        dto.StateId,
                        dto.Prop.TenantId,
                        dto.EffectiveFrom);
                ComplianceRule complianceRule = new ComplianceRule();
                if (existingRule != null)
                {
                    existingRule.EffectiveTo = dto.EffectiveFrom.AddDays(-1);
                    existingRule.UpdatedDateTime = DateTime.UtcNow;
                    existingRule.UpdatedById = validation.LoggedInEmployeeId;
                  
                    await _unitOfWork.CompilanceRuleRepository.UpdateAsync(complianceRule);
                }

                // 🔥 CREATE NEW RULE
                var entity = new ComplianceRule
                {
                    ComplianceTypeId = dto.ComplianceTypeId,
                    CountryId = dto.CountryId,
                    StateId = dto.StateId,
                    TenantId = dto.Prop.TenantId,

                    RuleJson = JsonSerializer.Serialize(dto.RuleJson),

                    Priority = dto.Priority,
                    EffectiveFrom = dto.EffectiveFrom,
                    EffectiveTo = null,

                    IsActive = true,
                    IsSoftDeleted = false,

                    AddedById = validation.LoggedInEmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };

                await _unitOfWork.CompilanceRuleRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "ComplianceRule created | Type: {Type}, Country: {Country}, Tenant: {Tenant}",
                    dto.ComplianceTypeId,
                    dto.CountryId,
                    dto.Prop.TenantId);

                // ===============================
                // 4️⃣ RESPONSE
                // ===============================
                var response = new GetComplianceRuleResponseDTO
                {
                    Id = entity.Id,
                    ComplianceTypeId = entity.ComplianceTypeId,
                    CountryId = entity.CountryId,
                    StateId = entity.StateId,
                    TenantId = entity.TenantId,
                    RuleJson = entity.RuleJson,
                    Priority = entity.Priority ?? 0,
                    EffectiveFrom = entity.EffectiveFrom,
                    EffectiveTo = entity.EffectiveTo
                  
                };

                return ApiResponse<GetComplianceRuleResponseDTO>
                    .Success(response, "Compliance rule created successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error while creating compliance rule");
                throw;
            }
        }
    }
}


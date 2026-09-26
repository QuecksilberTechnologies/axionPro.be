using System.Text.Json;
using axionpro.application.Constants;
using axionpro.application.DTOS.Policy;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories;

public sealed class GenericPolicyRepository(WorkforceDbContext context) : IGenericPolicyRepository
{
    private const short Draft = 1;
    private const short UnderReview = 2;
    private const short Approved = 3;
    private const short Published = 4;
    private const short Archived = 6;
    private const short Rejected = 7;

    public async Task<IReadOnlyList<PolicyLookupResponseDTO>> GetCategoriesAsync(CancellationToken cancellationToken) =>
        await context.PolicyCategories.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.CategoryName)
            .Select(x => new PolicyLookupResponseDTO(x.Id, x.CategoryCode, x.CategoryName, x.Description)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PolicyLookupResponseDTO>> GetStatusesAsync(CancellationToken cancellationToken) =>
        await context.PolicyStatuses.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Id)
            .Select(x => new PolicyLookupResponseDTO(x.Id, x.StatusCode, x.StatusName)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PolicyLookupResponseDTO>> GetRuleTypesAsync(CancellationToken cancellationToken) =>
        await context.PolicyRuleTypes.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.RuleTypeName)
            .Select(x => new PolicyLookupResponseDTO(x.Id, x.RuleTypeCode, x.RuleTypeName, x.Description)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PolicyLookupResponseDTO>> GetDocumentTypesAsync(CancellationToken cancellationToken) =>
        await context.PolicyDocumentTypes.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Id)
            .Select(x => new PolicyLookupResponseDTO(x.Id, x.DocumentTypeCode, x.DocumentTypeName)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PolicyTypeResponseDTO>> GetPolicyTypesAsync(long tenantId, bool isActive, CancellationToken cancellationToken) =>
        await context.PolicyTypes.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.IsActive == isActive && x.IsSoftDelete != true)
            .OrderBy(x => x.PolicyName)
            .Select(x => MapType(x)).ToListAsync(cancellationToken);

    public async Task<PolicyTypeResponseDTO> CreatePolicyTypeAsync(long tenantId, long actorId, CreateGenericPolicyTypeRequestDTO dto, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(dto.PolicyTypeCode);
        await EnsureCategoryAsync(dto.PolicyCategoryId, cancellationToken);
        if (await context.PolicyTypes.AnyAsync(x => x.TenantId == tenantId && x.PolicyTypeCode == code && x.IsSoftDelete != true, cancellationToken))
        {
            throw new ConflictException("Policy type code already exists for this tenant.");
        }

        var entity = new PolicyType
        {
            TenantId = tenantId,
            PolicyTypeCode = code,
            PolicyName = dto.PolicyName.Trim(),
            Description = dto.Description?.Trim(),
            PolicyCategoryId = dto.PolicyCategoryId,
            DefaultCurrencyCode = NormalizeCurrency(dto.DefaultCurrencyCode),
            IsActive = true,
            IsSoftDelete = false,
            IsStructured = true,
            AddedById = actorId,
            AddedDateTime = DateTime.UtcNow
        };
        context.PolicyTypes.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return MapType(entity);
    }

    public async Task<PolicyTypeResponseDTO> UpdatePolicyTypeAsync(long tenantId, long actorId, UpdateGenericPolicyTypeRequestDTO dto, CancellationToken cancellationToken)
    {
        var entity = await context.PolicyTypes.FirstOrDefaultAsync(x => x.Id == dto.Id && x.TenantId == tenantId && x.IsSoftDelete != true, cancellationToken)
            ?? throw new NotFoundException("Policy type was not found.");
        var code = NormalizeCode(dto.PolicyTypeCode);
        await EnsureCategoryAsync(dto.PolicyCategoryId, cancellationToken);
        if (await context.PolicyTypes.AnyAsync(x => x.Id != dto.Id && x.TenantId == tenantId && x.PolicyTypeCode == code && x.IsSoftDelete != true, cancellationToken))
        {
            throw new ConflictException("Policy type code already exists for this tenant.");
        }
        entity.PolicyTypeCode = code;
        entity.PolicyName = dto.PolicyName.Trim();
        entity.Description = dto.Description?.Trim();
        entity.PolicyCategoryId = dto.PolicyCategoryId;
        entity.DefaultCurrencyCode = NormalizeCurrency(dto.DefaultCurrencyCode);
        entity.IsActive = dto.IsActive;
        entity.UpdateById = actorId;
        entity.UpdateDateTime = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return MapType(entity);
    }

    public async Task<bool> ChangePolicyTypeStatusAsync(long tenantId, long actorId, ChangePolicyTypeStatusRequestDTO dto, CancellationToken cancellationToken)
    {
        var entity = await context.PolicyTypes.FirstOrDefaultAsync(x => x.Id == dto.Id && x.TenantId == tenantId && x.IsSoftDelete != true, cancellationToken)
            ?? throw new NotFoundException("Policy type was not found.");
        entity.IsActive = dto.IsActive;
        entity.UpdateById = actorId;
        entity.UpdateDateTime = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(IReadOnlyList<PolicySummaryResponseDTO> Items, int Total)> GetPoliciesAsync(long tenantId, PolicyListRequestDTO dto, CancellationToken cancellationToken)
    {
        var query = context.Policies.AsNoTracking().Where(x => x.TenantId == tenantId && !x.IsSoftDeleted);
        if (dto.PolicyTypeId.HasValue) query = query.Where(x => x.PolicyTypeId == dto.PolicyTypeId);
        if (dto.StatusId.HasValue) query = query.Where(x => context.PolicyVersions.Any(version => version.PolicyId == x.Id && version.TenantId == tenantId && version.PolicyStatusId == dto.StatusId));
        if (!string.IsNullOrWhiteSpace(dto.Search))
        {
            var search = dto.Search.Trim().ToLower();
            query = query.Where(x => x.PolicyCode.ToLower().Contains(search) || x.PolicyName.ToLower().Contains(search));
        }
        var total = await query.CountAsync(cancellationToken);
        var baseItems = await query.OrderBy(x => x.PolicyName).Skip((dto.PageNumber - 1) * dto.PageSize).Take(dto.PageSize).ToListAsync(cancellationToken);
        var policyIds = baseItems.Select(x => x.Id).ToArray();
        var versions = await context.PolicyVersions.AsNoTracking().Where(x => policyIds.Contains(x.PolicyId))
            .OrderByDescending(x => x.VersionNumber).ToListAsync(cancellationToken);
        var statuses = await context.PolicyStatuses.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.StatusName, cancellationToken);
        var items = baseItems.Select(p =>
        {
            var v = versions.FirstOrDefault(x => x.PolicyId == p.Id
                && (!dto.StatusId.HasValue || x.PolicyStatusId == dto.StatusId));
            return new PolicySummaryResponseDTO(p.Id, p.PolicyCode, p.PolicyName, p.PolicyTypeId, p.IsActive, v?.Id, v?.VersionNumber, v != null && statuses.TryGetValue(v.PolicyStatusId, out var status) ? status : null);
        }).Where(x => !dto.StatusId.HasValue || x.CurrentVersionId.HasValue).ToList();
        return (items, total);
    }

    public Task<PolicyDetailResponseDTO> GetPolicyAsync(long tenantId, long policyId, CancellationToken cancellationToken) => GetDetailAsync(tenantId, policyId, null, cancellationToken);

    public async Task<PolicyDetailResponseDTO> CreatePolicyAsync(long tenantId, long actorId, CreatePolicyRequestDTO dto, CancellationToken cancellationToken)
    {
        ValidateDates(dto.EffectiveFrom, dto.EffectiveTo);
        ValidateRulesAndScopes(dto.Rules, dto.Applicability);
        await ValidateScopeReferencesAsync(tenantId, dto.Applicability, cancellationToken);
        await ValidatePolicyReferencesAsync(tenantId, dto.OwnerDepartmentId, dto.Rules, cancellationToken);
        await ValidateAttendanceConfigurationAsync(tenantId, dto.PolicyTypeId, dto.AttendanceConfiguration, cancellationToken);
        var code = NormalizeCode(dto.PolicyCode);
        if (!await context.PolicyTypes.AnyAsync(x => x.Id == dto.PolicyTypeId && x.TenantId == tenantId && x.IsActive == true && x.IsSoftDelete != true, cancellationToken))
            throw new ValidationErrorException("PolicyTypeId is not active for this tenant.");
        if (await context.Policies.AnyAsync(x => x.TenantId == tenantId && x.PolicyCode == code && !x.IsSoftDeleted, cancellationToken))
            throw new ConflictException("Policy code already exists for this tenant.");

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var policy = new Policy { TenantId = tenantId, PolicyTypeId = dto.PolicyTypeId, PolicyCode = code, PolicyName = dto.PolicyName.Trim(), Summary = dto.Summary?.Trim(), OwnerDepartmentId = dto.OwnerDepartmentId, DefaultCurrencyCode = NormalizeCurrency(dto.DefaultCurrencyCode), IsActive = true, AddedById = actorId, AddedDateTime = now };
        context.Policies.Add(policy);
        await context.SaveChangesAsync(cancellationToken);
        var version = new PolicyVersion { TenantId = tenantId, PolicyId = policy.Id, VersionNumber = 1, PolicyStatusId = Draft, EffectiveFrom = dto.EffectiveFrom, EffectiveTo = dto.EffectiveTo, ChangeSummary = dto.ChangeSummary?.Trim(), RuleSchemaVersion = 1, IsActive = true, AddedById = actorId, AddedDateTime = now };
        context.PolicyVersions.Add(version);
        await context.SaveChangesAsync(cancellationToken);
        await UpsertAttendanceConfigurationAsync(tenantId, actorId, version.Id, dto.AttendanceConfiguration, now, cancellationToken);
        AddRulesAndApplicability(tenantId, actorId, version.Id, dto.Rules, dto.Applicability, now);
        AddAudit(tenantId, actorId, policy.Id, version.Id, "Policy", policy.Id, "CREATE", null, JsonSerializer.Serialize(new { policy.PolicyCode, policy.PolicyName }));
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return await GetDetailAsync(tenantId, policy.Id, version.Id, cancellationToken);
    }

    public async Task<PolicyDetailResponseDTO> UpdateDraftAsync(long tenantId, long actorId, UpdatePolicyDraftRequestDTO dto, CancellationToken cancellationToken)
    {
        ValidateDates(dto.EffectiveFrom, dto.EffectiveTo);
        ValidateRulesAndScopes(dto.Rules, dto.Applicability);
        await ValidateScopeReferencesAsync(tenantId, dto.Applicability, cancellationToken);
        await ValidatePolicyReferencesAsync(tenantId, dto.OwnerDepartmentId, dto.Rules, cancellationToken);
        await ValidateAttendanceConfigurationAsync(tenantId, dto.PolicyTypeId, dto.AttendanceConfiguration, cancellationToken);
        var policy = await context.Policies.FirstOrDefaultAsync(x => x.Id == dto.PolicyId && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken) ?? throw new NotFoundException("Policy was not found.");
        var version = await context.PolicyVersions.FirstOrDefaultAsync(x => x.Id == dto.PolicyVersionId && x.PolicyId == policy.Id && x.TenantId == tenantId, cancellationToken) ?? throw new NotFoundException("Policy version was not found.");
        if (version.PolicyStatusId != Draft && version.PolicyStatusId != Rejected) throw new ConflictException("Only draft or rejected versions can be edited.");
        var code = NormalizeCode(dto.PolicyCode);
        if (!await context.PolicyTypes.AnyAsync(x => x.Id == dto.PolicyTypeId && x.TenantId == tenantId && x.IsActive == true && x.IsSoftDelete != true, cancellationToken)) throw new ValidationErrorException("PolicyTypeId is not active for this tenant.");
        if (await context.Policies.AnyAsync(x => x.Id != policy.Id && x.TenantId == tenantId && x.PolicyCode == code && !x.IsSoftDeleted, cancellationToken)) throw new ConflictException("Policy code already exists for this tenant.");
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        policy.PolicyTypeId = dto.PolicyTypeId; policy.PolicyCode = code; policy.PolicyName = dto.PolicyName.Trim(); policy.Summary = dto.Summary?.Trim(); policy.OwnerDepartmentId = dto.OwnerDepartmentId; policy.DefaultCurrencyCode = NormalizeCurrency(dto.DefaultCurrencyCode); policy.UpdatedById = actorId; policy.UpdatedDateTime = DateTime.UtcNow;
        version.EffectiveFrom = dto.EffectiveFrom; version.EffectiveTo = dto.EffectiveTo; version.ChangeSummary = dto.ChangeSummary?.Trim(); version.UpdatedById = actorId; version.UpdatedDateTime = DateTime.UtcNow; version.PolicyStatusId = Draft;
        context.PolicyRules.RemoveRange(context.PolicyRules.Where(x => x.PolicyVersionId == version.Id && x.TenantId == tenantId));
        context.PolicyApplicabilities.RemoveRange(context.PolicyApplicabilities.Where(x => x.PolicyVersionId == version.Id && x.TenantId == tenantId));
        await UpsertAttendanceConfigurationAsync(tenantId, actorId, version.Id, dto.AttendanceConfiguration, DateTime.UtcNow, cancellationToken);
        AddRulesAndApplicability(tenantId, actorId, version.Id, dto.Rules, dto.Applicability, DateTime.UtcNow);
        AddAudit(tenantId, actorId, policy.Id, version.Id, "PolicyVersion", version.Id, "UPDATE_DRAFT", null, null);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return await GetDetailAsync(tenantId, policy.Id, version.Id, cancellationToken);
    }

    public async Task<PolicyDetailResponseDTO> CloneVersionAsync(long tenantId, long actorId, ClonePolicyVersionRequestDTO dto, CancellationToken cancellationToken)
    {
        var policy = await context.Policies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.PolicyId && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken) ?? throw new NotFoundException("Policy was not found.");
        var source = await context.PolicyVersions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.SourceVersionId && x.PolicyId == policy.Id && x.TenantId == tenantId, cancellationToken) ?? throw new NotFoundException("Source version was not found.");
        if (await context.PolicyVersions.AnyAsync(x => x.PolicyId == policy.Id && x.PolicyStatusId == Draft, cancellationToken)) throw new ConflictException("A draft version already exists.");
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var number = await context.PolicyVersions.Where(x => x.PolicyId == policy.Id).MaxAsync(x => x.VersionNumber, cancellationToken) + 1;
        var version = new PolicyVersion { TenantId = tenantId, PolicyId = policy.Id, VersionNumber = number, PolicyStatusId = Draft, EffectiveFrom = dto.EffectiveFrom, ChangeSummary = dto.ChangeSummary?.Trim(), RuleSchemaVersion = source.RuleSchemaVersion, IsActive = true, AddedById = actorId, AddedDateTime = DateTime.UtcNow };
        context.PolicyVersions.Add(version); await context.SaveChangesAsync(cancellationToken);
        var rules = await context.PolicyRules.AsNoTracking().Where(x => x.PolicyVersionId == source.Id).ToListAsync(cancellationToken);
        var scopes = await context.PolicyApplicabilities.AsNoTracking().Where(x => x.PolicyVersionId == source.Id).ToListAsync(cancellationToken);
        var attendanceConfiguration = await context.AttendancePolicyVersionConfigurations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PolicyVersionId == source.Id && x.TenantId == tenantId, cancellationToken);
        context.PolicyRules.AddRange(rules.Select(x => new PolicyRule { TenantId = tenantId, PolicyVersionId = version.Id, PolicyRuleTypeId = x.PolicyRuleTypeId, RuleName = x.RuleName, RuleOrder = x.RuleOrder, RuleConfiguration = x.RuleConfiguration, IsActive = x.IsActive, AddedById = actorId, AddedDateTime = DateTime.UtcNow }));
        context.PolicyApplicabilities.AddRange(scopes.Select(x => CloneScope(x, version.Id, actorId)));
        if (attendanceConfiguration != null)
        {
            context.AttendancePolicyVersionConfigurations.Add(MapAttendanceConfiguration(
                tenantId,
                actorId,
                version.Id,
                ToDto(attendanceConfiguration),
                DateTime.UtcNow));
        }
        AddAudit(tenantId, actorId, policy.Id, version.Id, "PolicyVersion", version.Id, "CLONE", null, JsonSerializer.Serialize(new { SourceVersionId = source.Id }));
        await context.SaveChangesAsync(cancellationToken); await transaction.CommitAsync(cancellationToken);
        return await GetDetailAsync(tenantId, policy.Id, version.Id, cancellationToken);
    }

    public async Task<PolicyDetailResponseDTO> TransitionAsync(long tenantId, long actorId, PolicyTransitionRequestDTO dto, CancellationToken cancellationToken)
    {
        var version = await context.PolicyVersions
            .FirstOrDefaultAsync(x => x.Id == dto.PolicyVersionId && x.TenantId == tenantId, cancellationToken)
            ?? throw new NotFoundException("Policy version was not found.");
        var action = dto.Action.Trim().ToUpperInvariant();

        if (version.PolicyStatusId == UnderReview && action is "APPROVE" or "REJECT")
        {
            return await RecordApprovalDecisionAsync(tenantId, actorId, version, action, dto.Comments, cancellationToken);
        }

        var target = (version.PolicyStatusId, action) switch
        {
            (Draft, "SUBMIT") => UnderReview,
            (Rejected, "SUBMIT") => UnderReview,
            (Approved, "PUBLISH") => Published,
            (Published, "ARCHIVE") => Archived,
            _ => throw new ConflictException($"Action {action} is invalid for the current policy status.")
        };

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        version.PolicyStatusId = target;
        version.UpdatedById = actorId;
        version.UpdatedDateTime = DateTime.UtcNow;
        if (target == Published)
        {
            var categoryId = await GetPolicyCategoryIdAsync(tenantId, version.PolicyId, cancellationToken);
            var categoryCode = categoryId.HasValue
                ? await context.PolicyCategories.AsNoTracking()
                    .Where(x => x.Id == categoryId.Value)
                    .Select(x => x.CategoryCode)
                    .FirstOrDefaultAsync(cancellationToken)
                : null;
            if (string.Equals(categoryCode, AppConstants.PolicyCategoryCodes.Attendance, StringComparison.OrdinalIgnoreCase)
                && !await context.AttendancePolicyVersionConfigurations.AsNoTracking()
                    .AnyAsync(x => x.TenantId == tenantId && x.PolicyVersionId == version.Id, cancellationToken))
            {
                throw new ConflictException(AppConstants.ErrorMessages.AttendancePolicyConfigurationRequired);
            }

            var mandatoryStages = await context.PolicyApprovalStages.AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.IsActive && x.IsMandatory
                    && (x.PolicyCategoryId == null || x.PolicyCategoryId == categoryId))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
            if (mandatoryStages.Count > 0 && version.ApprovedDateTime == null)
            {
                throw new ConflictException("All mandatory approval stages must be completed before publishing.");
            }

            await context.PolicyVersions
                .Where(x => x.PolicyId == version.PolicyId && x.Id != version.Id && x.IsCurrent)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsCurrent, false), cancellationToken);
            version.PublishedById = actorId;
            version.PublishedDateTime = DateTime.UtcNow;
            version.IsCurrent = true;
        }
        if (target == Archived)
        {
            version.IsCurrent = false;
        }

        AddAudit(tenantId, actorId, version.PolicyId, version.Id, "PolicyVersion", version.Id, action, null,
            JsonSerializer.Serialize(new { StatusId = target, dto.Comments }));
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return await GetDetailAsync(tenantId, version.PolicyId, version.Id, cancellationToken);
    }

    private async Task<PolicyDetailResponseDTO> RecordApprovalDecisionAsync(
        long tenantId,
        long actorId,
        PolicyVersion version,
        string action,
        string? comments,
        CancellationToken cancellationToken)
    {
        var categoryId = await GetPolicyCategoryIdAsync(tenantId, version.PolicyId, cancellationToken);
        var stages = await context.PolicyApprovalStages
            .Where(x => x.TenantId == tenantId && x.IsActive && x.IsMandatory
                && (x.PolicyCategoryId == null || x.PolicyCategoryId == categoryId))
            .OrderBy(x => x.StageOrder)
            .ToListAsync(cancellationToken);

        if (stages.Count == 0)
        {
            version.PolicyStatusId = action == "APPROVE" ? Approved : Rejected;
            version.UpdatedById = actorId;
            version.UpdatedDateTime = DateTime.UtcNow;
            if (action == "APPROVE")
            {
                version.ApprovedById = actorId;
                version.ApprovedDateTime = DateTime.UtcNow;
            }
            AddAudit(tenantId, actorId, version.PolicyId, version.Id, "PolicyVersion", version.Id, action, null,
                JsonSerializer.Serialize(new { version.PolicyStatusId, Comments = comments }));
            await context.SaveChangesAsync(cancellationToken);
            return await GetDetailAsync(tenantId, version.PolicyId, version.Id, cancellationToken);
        }

        var history = await context.PolicyApprovalHistories
            .Where(x => x.TenantId == tenantId && x.PolicyVersionId == version.Id)
            .OrderBy(x => x.SequenceNumber)
            .ToListAsync(cancellationToken);
        var lastRejectionSequence = history.Where(x => x.ActionType == 2)
            .Select(x => x.SequenceNumber)
            .DefaultIfEmpty(0)
            .Max();
        var currentCycle = history.Where(x => x.SequenceNumber > lastRejectionSequence).ToList();
        var currentStage = stages.FirstOrDefault(stage =>
            currentCycle.Count(x => x.PolicyApprovalStageId == stage.Id && x.ActionType == 1) < stage.MinimumApprovals);
        if (currentStage == null)
        {
            throw new ConflictException("All mandatory approval stages are already complete.");
        }

        if (currentStage.ApproverRoleId.HasValue)
        {
            var hasRole = await context.UserRoles.AsNoTracking().AnyAsync(x =>
                x.EmployeeId == actorId && x.RoleId == currentStage.ApproverRoleId && x.IsActive
                && x.IsSoftDeleted != true && x.Role != null && x.Role.TenantId == tenantId,
                cancellationToken);
            if (!hasRole)
            {
                throw new ForbiddenAccessException("The current approval stage requires a different approver role.");
            }
        }
        if (currentCycle.Any(x => x.PolicyApprovalStageId == currentStage.Id && x.ActionById == actorId && x.ActionType == 1))
        {
            throw new ConflictException("This approver has already approved the current stage.");
        }

        var nextSequence = history.Select(x => x.SequenceNumber).DefaultIfEmpty(0).Max() + 1;
        context.PolicyApprovalHistories.Add(new PolicyApprovalHistory
        {
            TenantId = tenantId,
            PolicyVersionId = version.Id,
            PolicyApprovalStageId = currentStage.Id,
            ActionType = action == "APPROVE" ? (short)1 : (short)2,
            ActionById = actorId,
            ActionDateTime = DateTime.UtcNow,
            Comments = comments?.Trim(),
            SequenceNumber = nextSequence
        });

        if (action == "REJECT")
        {
            version.PolicyStatusId = Rejected;
        }
        else
        {
            var approvalsAfterDecision = currentCycle.Count(x =>
                x.PolicyApprovalStageId == currentStage.Id && x.ActionType == 1) + 1;
            var stageCompleted = approvalsAfterDecision >= currentStage.MinimumApprovals;
            var laterStageExists = stages.Any(x => x.StageOrder > currentStage.StageOrder);
            if (stageCompleted && !laterStageExists)
            {
                version.PolicyStatusId = Approved;
                version.ApprovedById = actorId;
                version.ApprovedDateTime = DateTime.UtcNow;
            }
        }
        version.UpdatedById = actorId;
        version.UpdatedDateTime = DateTime.UtcNow;
        AddAudit(tenantId, actorId, version.PolicyId, version.Id, "PolicyApprovalHistory", null, action, null,
            JsonSerializer.Serialize(new { StageId = currentStage.Id, currentStage.StageOrder, Comments = comments }));
        await context.SaveChangesAsync(cancellationToken);
        return await GetDetailAsync(tenantId, version.PolicyId, version.Id, cancellationToken);
    }

    public async Task<PolicyAssignmentResultDTO> AssignAsync(long tenantId, long actorId, AssignPolicyRequestDTO dto, CancellationToken cancellationToken)
    {
        ValidateDates(dto.EffectiveFrom, dto.EffectiveTo);
        var version = await context.PolicyVersions.FirstOrDefaultAsync(x => x.Id == dto.PolicyVersionId
            && x.TenantId == tenantId && x.PolicyStatusId == Published, cancellationToken)
            ?? throw new ConflictException("Only a published policy version can be assigned.");
        var employeeIds = dto.EmployeeIds.Distinct().ToList();
        var validEmployeeIds = await context.Employees
            .Where(x => x.TenantId == tenantId && employeeIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        if (validEmployeeIds.Count != employeeIds.Count)
        {
            throw new ValidationErrorException("One or more employees do not belong to the authenticated tenant.");
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var matching = await context.PolicyAssignments.Where(x => x.TenantId == tenantId
            && x.PolicyVersionId == dto.PolicyVersionId && validEmployeeIds.Contains(x.EmployeeId)
            && x.EffectiveFrom == dto.EffectiveFrom).ToListAsync(cancellationToken);
        var activeEmployeeIds = matching.Where(x => x.IsActive).Select(x => x.EmployeeId).ToHashSet();
        var inactiveByEmployee = matching.Where(x => !x.IsActive).GroupBy(x => x.EmployeeId)
            .ToDictionary(x => x.Key, x => x.First());
        var insert = new List<PolicyAssignment>();
        foreach (var employeeId in validEmployeeIds.Where(x => !activeEmployeeIds.Contains(x)))
        {
            if (inactiveByEmployee.TryGetValue(employeeId, out var inactive))
            {
                inactive.IsActive = true;
                inactive.EffectiveTo = dto.EffectiveTo;
                inactive.IsMandatory = dto.IsMandatory;
                inactive.AssignedById = actorId;
                inactive.AssignedDateTime = DateTime.UtcNow;
                inactive.RemovedById = null;
                inactive.RemovedDateTime = null;
            }
            else
            {
                insert.Add(new PolicyAssignment
                {
                    TenantId = tenantId,
                    PolicyVersionId = dto.PolicyVersionId,
                    EmployeeId = employeeId,
                    AssignmentSource = 1,
                    EffectiveFrom = dto.EffectiveFrom,
                    EffectiveTo = dto.EffectiveTo,
                    IsMandatory = dto.IsMandatory,
                    IsActive = true,
                    AssignedById = actorId,
                    AssignedDateTime = DateTime.UtcNow
                });
            }
        }
        context.PolicyAssignments.AddRange(insert);
        var acknowledgementEmployeeIds = await context.PolicyAcknowledgements
            .Where(x => x.TenantId == tenantId && x.PolicyVersionId == dto.PolicyVersionId
                && validEmployeeIds.Contains(x.EmployeeId))
            .Select(x => x.EmployeeId)
            .ToListAsync(cancellationToken);
        context.PolicyAcknowledgements.AddRange(validEmployeeIds.Except(acknowledgementEmployeeIds)
            .Select(employeeId => new PolicyAcknowledgement
            {
                TenantId = tenantId,
                PolicyVersionId = dto.PolicyVersionId,
                EmployeeId = employeeId,
                AcknowledgementStatus = 1,
                AssignedDateTime = DateTime.UtcNow
            }));
        AddAudit(tenantId, actorId, version.PolicyId, version.Id, "PolicyAssignment", null, "ASSIGN", null,
            JsonSerializer.Serialize(new { EmployeeIds = validEmployeeIds, dto.EffectiveFrom, dto.EffectiveTo }));
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new PolicyAssignmentResultDTO(insert.Count + inactiveByEmployee.Keys.Count(x => validEmployeeIds.Contains(x)), activeEmployeeIds.Count);
    }

    public async Task<bool> RemoveAssignmentAsync(long tenantId, long actorId, long assignmentId, CancellationToken cancellationToken)
    {
        var entity = await context.PolicyAssignments.FirstOrDefaultAsync(x => x.Id == assignmentId && x.TenantId == tenantId, cancellationToken) ?? throw new NotFoundException("Policy assignment was not found.");
        entity.IsActive = false; entity.RemovedById = actorId; entity.RemovedDateTime = DateTime.UtcNow; await context.SaveChangesAsync(cancellationToken); return true;
    }

    public async Task<long> CreateExceptionAsync(long tenantId, long actorId, CreatePolicyExceptionRequestDTO dto, CancellationToken cancellationToken)
    {
        ValidateDates(dto.EffectiveFrom, dto.EffectiveTo); ValidateJson(new[] { dto.OverrideConfiguration });
        if (!await context.PolicyVersions.AnyAsync(x => x.Id == dto.PolicyVersionId && x.TenantId == tenantId, cancellationToken) || !await context.Employees.AnyAsync(x => x.Id == dto.EmployeeId && x.TenantId == tenantId, cancellationToken)) throw new ValidationErrorException("Policy version or employee is invalid for this tenant.");
        var entity = new PolicyException { TenantId = tenantId, PolicyVersionId = dto.PolicyVersionId, EmployeeId = dto.EmployeeId, ExceptionType = dto.ExceptionType, OverrideConfiguration = dto.OverrideConfiguration, Reason = dto.Reason.Trim(), EffectiveFrom = dto.EffectiveFrom, EffectiveTo = dto.EffectiveTo, ApprovalStatusId = UnderReview, IsActive = true, AddedById = actorId, AddedDateTime = DateTime.UtcNow };
        context.PolicyExceptions.Add(entity); await context.SaveChangesAsync(cancellationToken); return entity.Id;
    }

    public async Task<bool> ApproveExceptionAsync(long tenantId, long actorId, ApprovePolicyExceptionRequestDTO dto, CancellationToken cancellationToken)
    {
        var entity = await context.PolicyExceptions.FirstOrDefaultAsync(x => x.Id == dto.ExceptionId && x.TenantId == tenantId, cancellationToken) ?? throw new NotFoundException("Policy exception was not found.");
        entity.ApprovalStatusId = dto.Approve ? Approved : Rejected; entity.ApprovedById = actorId; entity.ApprovedDateTime = DateTime.UtcNow; await context.SaveChangesAsync(cancellationToken); return true;
    }

    public async Task<bool> AcknowledgeAsync(long tenantId, long employeeId, AcknowledgePolicyRequestDTO dto, CancellationToken cancellationToken)
    {
        if (!await context.PolicyAssignments.AnyAsync(x => x.TenantId == tenantId && x.EmployeeId == employeeId && x.PolicyVersionId == dto.PolicyVersionId && x.IsActive, cancellationToken)) throw new ForbiddenAccessException("The policy is not assigned to this employee.");
        if (dto.EvidenceJson != null) ValidateJson(new[] { dto.EvidenceJson });
        var entity = await context.PolicyAcknowledgements.FirstOrDefaultAsync(x => x.PolicyVersionId == dto.PolicyVersionId && x.EmployeeId == employeeId, cancellationToken);
        if (entity == null)
        {
            entity = new PolicyAcknowledgement
            {
                TenantId = tenantId,
                PolicyVersionId = dto.PolicyVersionId,
                EmployeeId = employeeId,
                AssignedDateTime = DateTime.UtcNow
            };
            context.PolicyAcknowledgements.Add(entity);
        }
        entity.AcknowledgementStatus = 3; entity.ViewedDateTime ??= DateTime.UtcNow; entity.AcknowledgedDateTime = DateTime.UtcNow; entity.EvidenceJson = dto.EvidenceJson;
        await context.SaveChangesAsync(cancellationToken); return true;
    }

    public async Task<IReadOnlyList<ResolvedPolicyResponseDTO>> ResolveAsync(long tenantId, long employeeId, ResolveEmployeePoliciesRequestDTO dto, CancellationToken cancellationToken)
    {
        var employee = await context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == employeeId && x.TenantId == tenantId, cancellationToken) ?? throw new NotFoundException("Employee was not found.");
        var date = dto.EffectiveDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var versions = await (from v in context.PolicyVersions.AsNoTracking() join p in context.Policies.AsNoTracking() on v.PolicyId equals p.Id where v.TenantId == tenantId && v.PolicyStatusId == Published && v.IsCurrent && v.IsActive && p.IsActive && !p.IsSoftDeleted && v.EffectiveFrom <= date && (v.EffectiveTo == null || v.EffectiveTo >= date) select new { Version = v, Policy = p }).ToListAsync(cancellationToken);
        var ids = versions.Select(x => x.Version.Id).ToArray();
        var scopes = await context.PolicyApplicabilities.AsNoTracking().Where(x => x.TenantId == tenantId && ids.Contains(x.PolicyVersionId) && x.IsActive && x.EffectiveFrom <= date && (x.EffectiveTo == null || x.EffectiveTo >= date)).ToListAsync(cancellationToken);
        var manual = await context.PolicyAssignments.AsNoTracking().Where(x => x.TenantId == tenantId && x.EmployeeId == employee.Id && ids.Contains(x.PolicyVersionId) && x.IsActive && x.EffectiveFrom <= date && (x.EffectiveTo == null || x.EffectiveTo >= date)).Select(x => x.PolicyVersionId).ToListAsync(cancellationToken);
        var locations = await (from assignment in context.EmployeeLocationAssignments.AsNoTracking()
                               join location in context.TenantLocations.AsNoTracking() on assignment.TenantLocationId equals location.Id
                               where assignment.TenantId == tenantId && assignment.EmployeeId == employee.Id && assignment.IsActive && !assignment.IsSoftDeleted && assignment.EffectiveFrom <= date && (assignment.EffectiveTo == null || assignment.EffectiveTo >= date)
                               select location).ToListAsync(cancellationToken);
        var workMode = await context.EmployeeWorkArrangements.AsNoTracking().Where(x => x.TenantId == tenantId && x.EmployeeId == employee.Id && x.IsActive && !x.IsSoftDeleted && x.EffectiveFrom <= date && (x.EffectiveTo == null || x.EffectiveTo >= date)).OrderByDescending(x => x.EffectiveFrom).Select(x => (short?)x.WorkMode).FirstOrDefaultAsync(cancellationToken);
        var result = new List<ResolvedPolicyResponseDTO>();
        foreach (var item in versions)
        {
            if (manual.Contains(item.Version.Id))
            {
                result.Add(new ResolvedPolicyResponseDTO(item.Policy.Id, item.Version.Id,
                    item.Policy.PolicyCode, item.Policy.PolicyName, int.MinValue, "MANUAL_ASSIGNMENT"));
                continue;
            }
            var matching = scopes
                .Where(x => x.PolicyVersionId == item.Version.Id && ScopeMatches(x, employee, locations, workMode, date))
                .ToList();
            if (matching.Count == 0)
            {
                continue;
            }
            var specificity = matching.Max(ScopeSpecificity);
            var mostSpecific = matching.Where(x => ScopeSpecificity(x) == specificity).ToList();
            var priority = mostSpecific.Min(x => x.Priority);
            var winners = mostSpecific.Where(x => x.Priority == priority).ToList();
            if (winners.Any(x => x.ApplicabilityMode == 2))
            {
                continue;
            }
            var include = winners.FirstOrDefault(x => x.ApplicabilityMode == 1);
            if (include != null)
            {
                result.Add(new ResolvedPolicyResponseDTO(item.Policy.Id, item.Version.Id,
                    item.Policy.PolicyCode, item.Policy.PolicyName, include.Priority, "APPLICABILITY"));
            }
        }
        return result.OrderBy(x => x.Priority).ThenBy(x => x.PolicyName).ToList();
    }

    public async Task<long> AddDocumentAsync(long tenantId, long actorId, UploadPolicyDocumentRequestDTO dto, string objectKey, string checksum, CancellationToken cancellationToken)
    {
        var version = await context.PolicyVersions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.PolicyVersionId && x.TenantId == tenantId, cancellationToken) ?? throw new NotFoundException("Policy version was not found.");
        if (version.PolicyStatusId is Published or Archived) throw new ConflictException("Documents cannot be changed on published or archived versions.");
        if (!await context.PolicyDocumentTypes.AnyAsync(x => x.Id == dto.PolicyDocumentTypeId && x.IsActive, cancellationToken)) throw new ValidationErrorException("PolicyDocumentTypeId is invalid.");
        var entity = new PolicyDocument { TenantId = tenantId, PolicyVersionId = dto.PolicyVersionId, PolicyDocumentTypeId = dto.PolicyDocumentTypeId, DocumentTitle = dto.DocumentTitle.Trim(), OriginalFileName = Path.GetFileName(dto.File.FileName), StorageProvider = "S3", ObjectKey = objectKey, ContentType = dto.File.ContentType, FileSizeBytes = dto.File.Length, ChecksumSha256 = checksum, LanguageCode = dto.LanguageCode?.Trim(), IsEmployeeVisible = dto.IsEmployeeVisible, IsActive = true, AddedById = actorId, AddedDateTime = DateTime.UtcNow };
        context.PolicyDocuments.Add(entity);
        AddAudit(tenantId, actorId, version.PolicyId, version.Id, "PolicyDocument", null, "UPLOAD", null, JsonSerializer.Serialize(new { entity.DocumentTitle, entity.OriginalFileName, entity.ChecksumSha256 }));
        await context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<IReadOnlyList<PolicyDocument>> GetDocumentsAsync(long tenantId, long policyVersionId, CancellationToken cancellationToken)
    {
        return await context.PolicyDocuments.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.PolicyVersionId == policyVersionId
                && x.IsActive && !x.IsSoftDeleted)
            .OrderBy(x => x.DocumentTitle)
            .ToListAsync(cancellationToken);
    }

    public async Task<string> DeleteDocumentAsync(long tenantId, long actorId, long documentId, CancellationToken cancellationToken)
    {
        var entity = await context.PolicyDocuments.FirstOrDefaultAsync(x => x.Id == documentId && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken) ?? throw new NotFoundException("Policy document was not found.");
        var version = await context.PolicyVersions.FirstAsync(x => x.Id == entity.PolicyVersionId && x.TenantId == tenantId, cancellationToken);
        if (version.PolicyStatusId is Published or Archived) throw new ConflictException("Documents cannot be changed on published or archived versions.");
        entity.IsActive = false; entity.IsSoftDeleted = true; entity.SoftDeletedById = actorId; entity.SoftDeletedDateTime = DateTime.UtcNow;
        AddAudit(tenantId, actorId, version.PolicyId, version.Id, "PolicyDocument", entity.Id, "DELETE", JsonSerializer.Serialize(new { entity.DocumentTitle, entity.ObjectKey }), null);
        await context.SaveChangesAsync(cancellationToken); return entity.ObjectKey;
    }

    public async Task<IReadOnlyList<PolicyApprovalStageResponseDTO>> GetApprovalStagesAsync(
        long tenantId,
        PolicyApprovalStageListRequestDTO dto,
        CancellationToken cancellationToken)
    {
        var query = context.PolicyApprovalStages.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.IsActive == dto.IsActive);
        if (dto.PolicyCategoryId.HasValue)
        {
            query = query.Where(x => x.PolicyCategoryId == null || x.PolicyCategoryId == dto.PolicyCategoryId);
        }
        return await query.OrderBy(x => x.StageOrder)
            .Select(x => MapApprovalStage(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<PolicyApprovalStageResponseDTO> CreateApprovalStageAsync(
        long tenantId,
        long actorId,
        CreatePolicyApprovalStageRequestDTO dto,
        CancellationToken cancellationToken)
    {
        await ValidateApprovalStageReferencesAsync(tenantId, dto.PolicyCategoryId, dto.ApproverRoleId, cancellationToken);
        if (await context.PolicyApprovalStages.AnyAsync(x => x.TenantId == tenantId
            && x.PolicyCategoryId == dto.PolicyCategoryId && x.StageOrder == dto.StageOrder, cancellationToken))
        {
            throw new ConflictException("An approval stage already exists at this order for the selected category.");
        }
        var entity = new PolicyApprovalStage
        {
            TenantId = tenantId,
            PolicyCategoryId = dto.PolicyCategoryId,
            StageName = dto.StageName.Trim(),
            StageOrder = dto.StageOrder,
            ApproverRoleId = dto.ApproverRoleId,
            MinimumApprovals = dto.MinimumApprovals,
            IsMandatory = dto.IsMandatory,
            IsActive = true,
            AddedById = actorId,
            AddedDateTime = DateTime.UtcNow
        };
        context.PolicyApprovalStages.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return MapApprovalStage(entity);
    }

    public async Task<PolicyApprovalStageResponseDTO> UpdateApprovalStageAsync(
        long tenantId,
        long actorId,
        UpdatePolicyApprovalStageRequestDTO dto,
        CancellationToken cancellationToken)
    {
        var entity = await context.PolicyApprovalStages.FirstOrDefaultAsync(
            x => x.Id == dto.Id && x.TenantId == tenantId, cancellationToken)
            ?? throw new NotFoundException("Policy approval stage was not found.");
        await ValidateApprovalStageReferencesAsync(tenantId, dto.PolicyCategoryId, dto.ApproverRoleId, cancellationToken);
        if (await context.PolicyApprovalStages.AnyAsync(x => x.Id != dto.Id && x.TenantId == tenantId
            && x.PolicyCategoryId == dto.PolicyCategoryId && x.StageOrder == dto.StageOrder, cancellationToken))
        {
            throw new ConflictException("An approval stage already exists at this order for the selected category.");
        }
        entity.PolicyCategoryId = dto.PolicyCategoryId;
        entity.StageName = dto.StageName.Trim();
        entity.StageOrder = dto.StageOrder;
        entity.ApproverRoleId = dto.ApproverRoleId;
        entity.MinimumApprovals = dto.MinimumApprovals;
        entity.IsMandatory = dto.IsMandatory;
        entity.IsActive = dto.IsActive;
        await context.SaveChangesAsync(cancellationToken);
        return MapApprovalStage(entity);
    }

    public async Task<bool> DeleteApprovalStageAsync(
        long tenantId,
        long actorId,
        long id,
        CancellationToken cancellationToken)
    {
        var entity = await context.PolicyApprovalStages.FirstOrDefaultAsync(
            x => x.Id == id && x.TenantId == tenantId, cancellationToken)
            ?? throw new NotFoundException("Policy approval stage was not found.");
        entity.IsActive = false;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<PolicyApprovalProgressResponseDTO>> GetApprovalProgressAsync(
        long tenantId,
        long policyVersionId,
        CancellationToken cancellationToken)
    {
        var version = await context.PolicyVersions.AsNoTracking().FirstOrDefaultAsync(
            x => x.Id == policyVersionId && x.TenantId == tenantId, cancellationToken)
            ?? throw new NotFoundException("Policy version was not found.");
        var categoryId = await GetPolicyCategoryIdAsync(tenantId, version.PolicyId, cancellationToken);
        var stages = await context.PolicyApprovalStages.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.IsActive && x.IsMandatory
                && (x.PolicyCategoryId == null || x.PolicyCategoryId == categoryId))
            .OrderBy(x => x.StageOrder)
            .ToListAsync(cancellationToken);
        var history = await context.PolicyApprovalHistories.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.PolicyVersionId == policyVersionId)
            .ToListAsync(cancellationToken);
        var lastRejectionSequence = history.Where(x => x.ActionType == 2)
            .Select(x => x.SequenceNumber).DefaultIfEmpty(0).Max();
        return stages.Select(stage =>
        {
            var count = history.Count(x => x.SequenceNumber > lastRejectionSequence
                && x.PolicyApprovalStageId == stage.Id && x.ActionType == 1);
            return new PolicyApprovalProgressResponseDTO(stage.Id, stage.StageName, stage.StageOrder,
                stage.MinimumApprovals, count, count >= stage.MinimumApprovals);
        }).ToList();
    }

    public async Task<IReadOnlyList<PolicyAssignmentResponseDTO>> GetAssignmentsAsync(
        long tenantId,
        long policyVersionId,
        CancellationToken cancellationToken)
    {
        await EnsureVersionAsync(tenantId, policyVersionId, cancellationToken);
        return await context.PolicyAssignments.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.PolicyVersionId == policyVersionId)
            .OrderByDescending(x => x.AssignedDateTime)
            .Select(x => new PolicyAssignmentResponseDTO(x.Id, x.PolicyVersionId, x.EmployeeId,
                x.AssignmentSource, x.EffectiveFrom, x.EffectiveTo, x.IsMandatory, x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PolicyExceptionResponseDTO>> GetExceptionsAsync(
        long tenantId,
        long policyVersionId,
        CancellationToken cancellationToken)
    {
        await EnsureVersionAsync(tenantId, policyVersionId, cancellationToken);
        return await context.PolicyExceptions.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.PolicyVersionId == policyVersionId)
            .OrderByDescending(x => x.AddedDateTime)
            .Select(x => new PolicyExceptionResponseDTO(x.Id, x.PolicyVersionId, x.EmployeeId,
                x.ExceptionType, x.OverrideConfiguration, x.Reason, x.EffectiveFrom, x.EffectiveTo,
                x.ApprovalStatusId, x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PolicyAcknowledgementResponseDTO>> GetAcknowledgementsAsync(
        long tenantId,
        long policyVersionId,
        CancellationToken cancellationToken)
    {
        await EnsureVersionAsync(tenantId, policyVersionId, cancellationToken);
        return await context.PolicyAcknowledgements.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.PolicyVersionId == policyVersionId)
            .OrderBy(x => x.EmployeeId)
            .Select(x => new PolicyAcknowledgementResponseDTO(x.Id, x.PolicyVersionId, x.EmployeeId,
                x.AcknowledgementStatus, x.AssignedDateTime, x.ViewedDateTime, x.AcknowledgedDateTime))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PolicyAuditResponseDTO>> GetAuditAsync(long tenantId, long policyId, CancellationToken cancellationToken)
    {
        if (!await context.Policies.AnyAsync(x => x.Id == policyId && x.TenantId == tenantId, cancellationToken)) throw new NotFoundException("Policy was not found.");
        return await context.PolicyChangeAudits.AsNoTracking().Where(x => x.TenantId == tenantId && x.PolicyId == policyId).OrderByDescending(x => x.ChangedDateTime).Select(x => new PolicyAuditResponseDTO(x.Id, x.PolicyId, x.PolicyVersionId, x.EntityName, x.EntityId, x.ActionName, x.BeforeData, x.AfterData, x.ChangedById, x.ChangedDateTime, x.CorrelationId)).ToListAsync(cancellationToken);
    }

    private async Task<int?> GetPolicyCategoryIdAsync(long tenantId, long policyId, CancellationToken cancellationToken)
    {
        return await (from policy in context.Policies.AsNoTracking()
                      join policyType in context.PolicyTypes.AsNoTracking() on policy.PolicyTypeId equals policyType.Id
                      where policy.Id == policyId && policy.TenantId == tenantId && policyType.TenantId == tenantId
                      select policyType.PolicyCategoryId).FirstOrDefaultAsync(cancellationToken);
    }

    private async Task ValidateApprovalStageReferencesAsync(
        long tenantId,
        int? categoryId,
        int? roleId,
        CancellationToken cancellationToken)
    {
        if (categoryId.HasValue && !await context.PolicyCategories.AsNoTracking()
            .AnyAsync(x => x.Id == categoryId && x.IsActive, cancellationToken))
        {
            throw new ValidationErrorException("PolicyCategoryId is invalid.");
        }
        if (roleId.HasValue && !await context.Roles.AsNoTracking()
            .AnyAsync(x => x.Id == roleId && x.TenantId == tenantId && x.IsActive && x.IsSoftDeleted != true,
                cancellationToken))
        {
            throw new ValidationErrorException("ApproverRoleId is invalid for this tenant.");
        }
    }

    private async Task EnsureVersionAsync(long tenantId, long policyVersionId, CancellationToken cancellationToken)
    {
        if (!await context.PolicyVersions.AsNoTracking()
            .AnyAsync(x => x.Id == policyVersionId && x.TenantId == tenantId, cancellationToken))
        {
            throw new NotFoundException("Policy version was not found.");
        }
    }

    private static PolicyApprovalStageResponseDTO MapApprovalStage(PolicyApprovalStage x)
    {
        return new PolicyApprovalStageResponseDTO(x.Id, x.PolicyCategoryId, x.StageName, x.StageOrder,
            x.ApproverRoleId, x.MinimumApprovals, x.IsMandatory, x.IsActive);
    }

    private async Task<PolicyDetailResponseDTO> GetDetailAsync(long tenantId, long policyId, long? versionId, CancellationToken cancellationToken)
    {
        var policy = await context.Policies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == policyId && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken) ?? throw new NotFoundException("Policy was not found.");
        var versionQuery = context.PolicyVersions.AsNoTracking().Where(x => x.PolicyId == policyId && x.TenantId == tenantId);
        var version = versionId.HasValue ? await versionQuery.FirstOrDefaultAsync(x => x.Id == versionId, cancellationToken) : await versionQuery.OrderByDescending(x => x.IsCurrent).ThenByDescending(x => x.VersionNumber).FirstOrDefaultAsync(cancellationToken);
        if (version == null) throw new NotFoundException("Policy version was not found.");
        var status = await context.PolicyStatuses.AsNoTracking().Where(x => x.Id == version.PolicyStatusId).Select(x => x.StatusName).FirstAsync(cancellationToken);
        var rules = await context.PolicyRules.AsNoTracking().Where(x => x.PolicyVersionId == version.Id && x.IsActive).OrderBy(x => x.RuleOrder).Select(x => new PolicyRuleResponseDTO(x.Id, x.PolicyRuleTypeId, x.RuleName, x.RuleOrder, x.RuleConfiguration)).ToListAsync(cancellationToken);
        var scopes = await context.PolicyApplicabilities.AsNoTracking()
            .Where(x => x.PolicyVersionId == version.Id && x.IsActive)
            .OrderBy(x => x.Priority)
            .Select(x => new PolicyApplicabilityResponseDTO(x.Id, x.ApplicabilityMode, x.Priority,
                x.CountryId, x.StateId, x.DistrictId, x.LocalityId, x.TenantLocationId,
                x.EmployeeTypeId, x.DepartmentId, x.DesignationId, x.EmployeeId, x.GenderId,
                x.WorkArrangementType, x.EmploymentStatus, x.MinimumServiceDays,
                x.EffectiveFrom, x.EffectiveTo))
            .ToListAsync(cancellationToken);
        var attendanceConfiguration = await context.AttendancePolicyVersionConfigurations.AsNoTracking()
            .Where(x => x.PolicyVersionId == version.Id && x.TenantId == tenantId)
            .Select(x => new AttendancePolicyVersionConfigurationDTO
            {
                AttendanceLocationScope = (AttendanceLocationScope)x.AttendanceLocationScope,
                AllowBiometric = x.AllowBiometric,
                AllowMobile = x.AllowMobile,
                AllowWeb = x.AllowWeb,
                AllowManualAttendance = x.AllowManualAttendance,
                AllowWorkFromHome = x.AllowWorkFromHome,
                RequireGeoFenceForOffice = x.RequireGeoFenceForOffice,
                RequireGpsForRemote = x.RequireGpsForRemote,
                AllowOutsideLocationWithApproval = x.AllowOutsideLocationWithApproval
            })
            .FirstOrDefaultAsync(cancellationToken);
        return new PolicyDetailResponseDTO(policy.Id, policy.PolicyCode, policy.PolicyName,
            policy.Summary, policy.PolicyTypeId, policy.OwnerDepartmentId,
            policy.DefaultCurrencyCode, version.Id, version.VersionNumber,
            version.PolicyStatusId, status, version.EffectiveFrom, version.EffectiveTo,
            version.ChangeSummary, rules, scopes, attendanceConfiguration);
    }

    private async Task ValidateAttendanceConfigurationAsync(
        long tenantId,
        int policyTypeId,
        AttendancePolicyVersionConfigurationDTO? configuration,
        CancellationToken cancellationToken)
    {
        var categoryCode = await (from policyType in context.PolicyTypes.AsNoTracking()
                                  join category in context.PolicyCategories.AsNoTracking()
                                      on policyType.PolicyCategoryId equals category.Id
                                  where policyType.Id == policyTypeId && policyType.TenantId == tenantId
                                  select category.CategoryCode)
            .FirstOrDefaultAsync(cancellationToken);
        var isAttendance = string.Equals(categoryCode, AppConstants.PolicyCategoryCodes.Attendance, StringComparison.OrdinalIgnoreCase);
        if (isAttendance && configuration == null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.AttendancePolicyConfigurationRequired);
        }
        if (!isAttendance && configuration != null)
        {
            throw new ValidationErrorException("AttendanceConfiguration is allowed only for an Attendance policy type.");
        }
        if (configuration == null)
        {
            return;
        }
        if (!Enum.IsDefined(configuration.AttendanceLocationScope))
        {
            throw new ValidationErrorException("AttendanceLocationScope is invalid.");
        }
        if (!configuration.AllowBiometric && !configuration.AllowMobile
            && !configuration.AllowWeb && !configuration.AllowManualAttendance)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.AttendancePolicyChannelRequired);
        }
    }

    private async Task UpsertAttendanceConfigurationAsync(long tenantId, long actorId, long policyVersionId,
        AttendancePolicyVersionConfigurationDTO? dto, DateTime now, CancellationToken cancellationToken)
    {
        var existing = context.AttendancePolicyVersionConfigurations.Local
            .FirstOrDefault(x => x.PolicyVersionId == policyVersionId && x.TenantId == tenantId)
            ?? await context.AttendancePolicyVersionConfigurations
                .FirstOrDefaultAsync(x => x.PolicyVersionId == policyVersionId && x.TenantId == tenantId, cancellationToken);
        if (dto == null)
        {
            if (existing != null)
            {
                context.AttendancePolicyVersionConfigurations.Remove(existing);
            }
            return;
        }
        if (existing == null)
        {
            context.AttendancePolicyVersionConfigurations.Add(
                MapAttendanceConfiguration(tenantId, actorId, policyVersionId, dto, now));
            return;
        }
        ApplyAttendanceConfiguration(existing, dto);
        existing.UpdatedById = actorId;
        existing.UpdatedDateTime = now;
    }

    private static AttendancePolicyVersionConfiguration MapAttendanceConfiguration(long tenantId, long actorId,
        long policyVersionId, AttendancePolicyVersionConfigurationDTO dto, DateTime now)
    {
        var entity = new AttendancePolicyVersionConfiguration
        {
            TenantId = tenantId,
            PolicyVersionId = policyVersionId,
            AddedById = actorId,
            AddedDateTime = now
        };
        ApplyAttendanceConfiguration(entity, dto);
        return entity;
    }

    private static void ApplyAttendanceConfiguration(AttendancePolicyVersionConfiguration entity,
        AttendancePolicyVersionConfigurationDTO dto)
    {
        entity.AttendanceLocationScope = (short)dto.AttendanceLocationScope;
        entity.AllowBiometric = dto.AllowBiometric;
        entity.AllowMobile = dto.AllowMobile;
        entity.AllowWeb = dto.AllowWeb;
        entity.AllowManualAttendance = dto.AllowManualAttendance;
        entity.AllowWorkFromHome = dto.AllowWorkFromHome;
        entity.RequireGeoFenceForOffice = dto.RequireGeoFenceForOffice;
        entity.RequireGpsForRemote = dto.RequireGpsForRemote;
        entity.AllowOutsideLocationWithApproval = dto.AllowOutsideLocationWithApproval;
    }

    private static AttendancePolicyVersionConfigurationDTO ToDto(AttendancePolicyVersionConfiguration entity) => new()
    {
        AttendanceLocationScope = (AttendanceLocationScope)entity.AttendanceLocationScope,
        AllowBiometric = entity.AllowBiometric,
        AllowMobile = entity.AllowMobile,
        AllowWeb = entity.AllowWeb,
        AllowManualAttendance = entity.AllowManualAttendance,
        AllowWorkFromHome = entity.AllowWorkFromHome,
        RequireGeoFenceForOffice = entity.RequireGeoFenceForOffice,
        RequireGpsForRemote = entity.RequireGpsForRemote,
        AllowOutsideLocationWithApproval = entity.AllowOutsideLocationWithApproval
    };

    private void AddRulesAndApplicability(long tenantId, long actorId, long versionId, IEnumerable<PolicyRuleInputDTO> rules, IEnumerable<PolicyApplicabilityInputDTO> scopes, DateTime now)
    {
        context.PolicyRules.AddRange(rules.Select(x => new PolicyRule { TenantId = tenantId, PolicyVersionId = versionId, PolicyRuleTypeId = x.PolicyRuleTypeId, RuleName = x.RuleName.Trim(), RuleOrder = x.RuleOrder, RuleConfiguration = x.RuleConfiguration, IsActive = true, AddedById = actorId, AddedDateTime = now }));
        context.PolicyApplicabilities.AddRange(scopes.Select(x => new PolicyApplicability { TenantId = tenantId, PolicyVersionId = versionId, ApplicabilityMode = x.ApplicabilityMode, CountryId = x.CountryId, StateId = x.StateId, DistrictId = x.DistrictId, LocalityId = x.LocalityId, TenantLocationId = x.TenantLocationId, EmployeeTypeId = x.EmployeeTypeId, DepartmentId = x.DepartmentId, DesignationId = x.DesignationId, EmployeeId = x.EmployeeId, GenderId = x.GenderId, WorkArrangementType = x.WorkArrangementType, EmploymentStatus = x.EmploymentStatus, MinimumServiceDays = x.MinimumServiceDays, Priority = x.Priority, EffectiveFrom = x.EffectiveFrom, EffectiveTo = x.EffectiveTo, IsActive = true, AddedById = actorId, AddedDateTime = now }));
    }

    private void AddAudit(long tenantId, long actorId, long policyId, long? versionId,
        string entityName, long? entityId, string action, string? before, string? after)
    {
        context.PolicyChangeAudits.Add(new PolicyChangeAudit
        {
            TenantId = tenantId,
            PolicyId = policyId,
            PolicyVersionId = versionId,
            EntityName = entityName,
            EntityId = entityId,
            ActionName = action,
            BeforeData = before,
            AfterData = after,
            ChangedById = actorId,
            ChangedDateTime = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        });
    }
    private static PolicyTypeResponseDTO MapType(PolicyType x) => new(x.Id, x.PolicyTypeCode ?? string.Empty, x.PolicyName, x.Description, x.PolicyCategoryId, x.DefaultCurrencyCode, x.IsActive == true);
    private static string NormalizeCode(string value) => value.Trim().ToUpperInvariant().Replace(' ', '_');
    private static string? NormalizeCurrency(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    private static void ValidateDates(DateOnly from, DateOnly? to)
    {
        if (from == default || to.HasValue && to < from)
        {
            throw new ValidationErrorException("Effective date range is invalid.");
        }
    }

    private static void ValidateJson(IEnumerable<string> values)
    {
        foreach (var value in values)
        {
            try
            {
                using var document = JsonDocument.Parse(value);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new JsonException();
                }
            }
            catch (JsonException)
            {
                throw new ValidationErrorException("Rule and override configuration must be a JSON object.");
            }
        }
    }
    private static void ValidateRulesAndScopes(IReadOnlyCollection<PolicyRuleInputDTO> rules, IReadOnlyCollection<PolicyApplicabilityInputDTO> scopes)
    {
        ValidateJson(rules.Select(x => x.RuleConfiguration));
        if (rules.GroupBy(x => x.RuleOrder).Any(group => group.Count() > 1)) throw new ValidationErrorException("RuleOrder must be unique within a policy version.");
        foreach (var scope in scopes) ValidateDates(scope.EffectiveFrom, scope.EffectiveTo);
    }
    private async Task EnsureCategoryAsync(int id, CancellationToken token)
    {
        if (!await context.PolicyCategories.AnyAsync(x => x.Id == id && x.IsActive, token))
        {
            throw new ValidationErrorException("PolicyCategoryId is invalid.");
        }
    }
    private async Task ValidatePolicyReferencesAsync(
        long tenantId,
        int? ownerDepartmentId,
        IReadOnlyCollection<PolicyRuleInputDTO> rules,
        CancellationToken cancellationToken)
    {
        if (ownerDepartmentId.HasValue && !await context.Departments.AsNoTracking().AnyAsync(
            x => x.Id == ownerDepartmentId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted,
            cancellationToken))
        {
            throw new ValidationErrorException("OwnerDepartmentId is invalid for this tenant.");
        }
        var ruleTypeIds = rules.Select(x => x.PolicyRuleTypeId).Distinct().ToList();
        var validRuleTypeCount = await context.PolicyRuleTypes.AsNoTracking()
            .CountAsync(x => ruleTypeIds.Contains(x.Id) && x.IsActive, cancellationToken);
        if (validRuleTypeCount != ruleTypeIds.Count)
        {
            throw new ValidationErrorException("One or more PolicyRuleTypeId values are invalid.");
        }
    }

    private async Task ValidateScopeReferencesAsync(
        long tenantId,
        IReadOnlyCollection<PolicyApplicabilityInputDTO> scopes,
        CancellationToken cancellationToken)
    {
        foreach (var scope in scopes)
        {
            State? state = null;
            District? district = null;
            Locality? locality = null;
            TenantLocation? location = null;
            if (scope.CountryId.HasValue && !await context.Countries.AsNoTracking()
                .AnyAsync(x => x.Id == scope.CountryId && x.IsActive == true, cancellationToken))
            {
                throw new ValidationErrorException("CountryId is invalid.");
            }
            if (scope.StateId.HasValue)
            {
                state = await context.States.AsNoTracking().FirstOrDefaultAsync(
                    x => x.Id == scope.StateId && x.IsActive == true, cancellationToken)
                    ?? throw new ValidationErrorException("StateId is invalid.");
                if (scope.CountryId.HasValue && state.CountryId != scope.CountryId)
                {
                    throw new ValidationErrorException("StateId does not belong to CountryId.");
                }
            }
            if (scope.DistrictId.HasValue)
            {
                district = await context.Districts.AsNoTracking().FirstOrDefaultAsync(
                    x => x.Id == scope.DistrictId && x.IsActive, cancellationToken)
                    ?? throw new ValidationErrorException("DistrictId is invalid.");
                if (scope.StateId.HasValue && district.StateId != scope.StateId)
                {
                    throw new ValidationErrorException("DistrictId does not belong to StateId.");
                }
            }
            if (scope.LocalityId.HasValue)
            {
                locality = await context.Localities.AsNoTracking().FirstOrDefaultAsync(
                    x => x.Id == scope.LocalityId && x.IsActive == true, cancellationToken)
                    ?? throw new ValidationErrorException("LocalityId is invalid.");
                if (scope.DistrictId.HasValue && locality.DistrictId != scope.DistrictId)
                {
                    throw new ValidationErrorException("LocalityId does not belong to DistrictId.");
                }
                if (scope.StateId.HasValue && locality.StateId != scope.StateId)
                {
                    throw new ValidationErrorException("LocalityId does not belong to StateId.");
                }
            }
            if (scope.TenantLocationId.HasValue)
            {
                location = await context.TenantLocations.AsNoTracking().FirstOrDefaultAsync(
                    x => x.Id == scope.TenantLocationId && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted,
                    cancellationToken) ?? throw new ValidationErrorException("TenantLocationId is invalid for this tenant.");
                if ((scope.CountryId.HasValue && location.CountryId != scope.CountryId)
                    || (scope.StateId.HasValue && location.StateId != scope.StateId)
                    || (scope.DistrictId.HasValue && location.DistrictId != scope.DistrictId)
                    || (scope.LocalityId.HasValue && location.LocalityId != scope.LocalityId))
                {
                    throw new ValidationErrorException("TenantLocationId does not match the supplied geography.");
                }
            }
            if (scope.DepartmentId.HasValue && !await context.Departments.AsNoTracking().AnyAsync(
                x => x.Id == scope.DepartmentId && x.TenantId == tenantId && !x.IsSoftDeleted, cancellationToken))
            {
                throw new ValidationErrorException("DepartmentId is invalid for this tenant.");
            }
            if (scope.DesignationId.HasValue)
            {
                var designation = await context.Designations.AsNoTracking().FirstOrDefaultAsync(
                    x => x.Id == scope.DesignationId && x.TenantId == tenantId && !x.IsSoftDeleted,
                    cancellationToken) ?? throw new ValidationErrorException("DesignationId is invalid for this tenant.");
                if (scope.DepartmentId.HasValue && designation.DepartmentId != scope.DepartmentId)
                {
                    throw new ValidationErrorException("DesignationId does not belong to DepartmentId.");
                }
            }
            if (scope.EmployeeTypeId.HasValue && !await context.EmployeeTypes.AsNoTracking().AnyAsync(
                x => x.Id == scope.EmployeeTypeId && x.TenantId == tenantId && x.IsSoftDeleted != true,
                cancellationToken))
            {
                throw new ValidationErrorException("EmployeeTypeId is invalid for this tenant.");
            }
            if (scope.EmployeeId.HasValue && !await context.Employees.AsNoTracking().AnyAsync(
                x => x.Id == scope.EmployeeId && x.TenantId == tenantId, cancellationToken))
            {
                throw new ValidationErrorException("EmployeeId is invalid for this tenant.");
            }
            if (scope.GenderId.HasValue && !await context.Genders.AsNoTracking().AnyAsync(
                x => x.Id == scope.GenderId, cancellationToken))
            {
                throw new ValidationErrorException("GenderId is invalid.");
            }
        }
    }

    private static int ScopeSpecificity(PolicyApplicability scope)
    {
        if (scope.EmployeeId.HasValue) return 700;
        if (scope.TenantLocationId.HasValue || scope.LocalityId.HasValue) return 600;
        if (scope.DistrictId.HasValue) return 500;
        if (scope.StateId.HasValue) return 400;
        if (scope.CountryId.HasValue) return 300;
        if (scope.EmployeeTypeId.HasValue) return 200;
        if (scope.DepartmentId.HasValue || scope.DesignationId.HasValue) return 100;
        return 0;
    }

    private static bool ScopeMatches(PolicyApplicability x, Employee e, IReadOnlyCollection<TenantLocation> locations, short? workMode, DateOnly date)
    {
        var serviceDays = e.DateOfOnBoarding.HasValue
            ? Math.Max(0, date.DayNumber - DateOnly.FromDateTime(e.DateOfOnBoarding.Value).DayNumber)
            : 0;
        var geographyMatches = (!x.TenantLocationId.HasValue || locations.Any(location => location.Id == x.TenantLocationId))
            && (!x.CountryId.HasValue || e.CountryId == x.CountryId || locations.Any(location => location.CountryId == x.CountryId))
            && (!x.StateId.HasValue || locations.Any(location => location.StateId == x.StateId))
            && (!x.DistrictId.HasValue || locations.Any(location => location.DistrictId == x.DistrictId))
            && (!x.LocalityId.HasValue || locations.Any(location => location.LocalityId == x.LocalityId));
        return geographyMatches
            && (!x.EmployeeId.HasValue || x.EmployeeId == e.Id)
            && (!x.EmployeeTypeId.HasValue || x.EmployeeTypeId == e.EmployeeTypeId)
            && (!x.DepartmentId.HasValue || x.DepartmentId == e.DepartmentId)
            && (!x.DesignationId.HasValue || x.DesignationId == e.DesignationId)
            && (!x.GenderId.HasValue || x.GenderId == e.GenderId)
            && (!x.WorkArrangementType.HasValue || x.WorkArrangementType == workMode)
            && (!x.EmploymentStatus.HasValue || (x.EmploymentStatus == 1) == e.IsActive)
            && (!x.MinimumServiceDays.HasValue || serviceDays >= x.MinimumServiceDays);
    }
    private static PolicyApplicability CloneScope(PolicyApplicability x, long versionId, long actorId) => new() { TenantId = x.TenantId, PolicyVersionId = versionId, ApplicabilityMode = x.ApplicabilityMode, CountryId = x.CountryId, StateId = x.StateId, DistrictId = x.DistrictId, LocalityId = x.LocalityId, TenantLocationId = x.TenantLocationId, EmployeeTypeId = x.EmployeeTypeId, DepartmentId = x.DepartmentId, DesignationId = x.DesignationId, EmployeeId = x.EmployeeId, GenderId = x.GenderId, WorkArrangementType = x.WorkArrangementType, EmploymentStatus = x.EmploymentStatus, MinimumServiceDays = x.MinimumServiceDays, Priority = x.Priority, EffectiveFrom = x.EffectiveFrom, EffectiveTo = x.EffectiveTo, IsActive = x.IsActive, AddedById = actorId, AddedDateTime = DateTime.UtcNow };
}

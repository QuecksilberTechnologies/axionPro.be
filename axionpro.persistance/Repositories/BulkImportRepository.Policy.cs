using System.Globalization;
using System.Text.Json;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Policy;
using axionpro.application.Exceptions;
using axionpro.domain.Entity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace axionpro.persistance.Repositories;

public sealed partial class BulkImportRepository
{
    private static readonly JsonSerializerOptions PolicyJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<BulkImportPreviewResponseDTO> PreviewPolicyAsync(
        BulkImportMaster master,
        BulkImportTableDTO table,
        string? mappingJson,
        CommonDecodedResult actor,
        CancellationToken cancellationToken)
    {
        if (master is not (BulkImportMaster.PolicyType or BulkImportMaster.PolicyDefinition
            or BulkImportMaster.PolicyAssignment))
        {
            throw new ValidationErrorException("Unsupported policy import target.");
        }
        return await BuildPolicyPreviewAsync(master, table, mappingJson, actor.TenantId, cancellationToken);
    }

    private async Task<BulkImportPreviewResponseDTO> BuildPolicyPreviewAsync(
        BulkImportMaster master,
        BulkImportTableDTO table,
        string? mappingJson,
        long tenantId,
        CancellationToken cancellationToken)
    {
        var columns = PolicyColumns(master);
        var required = master switch
        {
            BulkImportMaster.PolicyType => new[] { "PolicyTypeCode", "PolicyName", "PolicyCategoryCode" },
            BulkImportMaster.PolicyDefinition => new[]
            {
                "PolicyCode", "PolicyName", "PolicyTypeCode", "EffectiveFrom", "RulesJson", "ApplicabilityJson"
            },
            _ => new[] { "PolicyCode", "VersionNumber", "EmployeeCode", "EffectiveFrom" }
        };
        var result = new BulkImportPreviewResponseDTO
        {
            Master = master,
            SourceColumns = table.Columns.ToList()
        };
        var mapping = BulkImportPreviewService.ResolveColumnMapping(table, mappingJson, columns);
        result.ColumnMapping = mapping;
        foreach (var field in required.Where(x => !mapping.ContainsKey(x)))
        {
            result.Errors.Add($"Map the required column {field}.");
        }

        foreach (var source in table.Rows)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var row = new BulkImportPreviewRowDTO
            {
                RowNumber = source.RowNumber,
                Values = mapping.ToDictionary(pair => pair.Key,
                    pair => source.Values[table.Columns.IndexOf(pair.Value)].Trim()),
                Status = BulkImportRowStatus.Ready
            };
            foreach (var field in required.Where(field => string.IsNullOrWhiteSpace(row.Values.GetValueOrDefault(field))))
            {
                row.Errors.Add($"{field} is required.");
            }

            if (row.Errors.Count == 0)
            {
                if (master == BulkImportMaster.PolicyType)
                {
                    await ValidatePolicyTypeRowAsync(row, tenantId, cancellationToken);
                }
                else if (master == BulkImportMaster.PolicyDefinition)
                {
                    await ValidatePolicyDefinitionRowAsync(row, tenantId, cancellationToken);
                }
                else
                {
                    await ValidatePolicyAssignmentRowAsync(row, tenantId, cancellationToken);
                }
            }
            if (row.Errors.Count > 0)
            {
                row.Status = BulkImportRowStatus.Invalid;
            }
            result.Rows.Add(row);
        }

        var keyName = master == BulkImportMaster.PolicyAssignment
            ? null
            : master == BulkImportMaster.PolicyType ? "PolicyTypeCode" : "PolicyCode";
        var duplicateGroups = result.Rows.GroupBy(row => master == BulkImportMaster.PolicyAssignment
                ? $"{row.Values.GetValueOrDefault("PolicyCode")}|{row.Values.GetValueOrDefault("VersionNumber")}|{row.Values.GetValueOrDefault("EmployeeCode")}|{row.Values.GetValueOrDefault("EffectiveFrom")}".ToUpperInvariant()
                : row.Values.GetValueOrDefault(keyName!, string.Empty).ToUpperInvariant())
            .Where(group => group.Count() > 1);
        foreach (var group in duplicateGroups)
        {
            foreach (var row in group)
            {
                row.Errors.Add("Duplicate source rows: " + string.Join(", ", group.Select(x => x.RowNumber)));
                row.Status = BulkImportRowStatus.Invalid;
            }
        }
        return result;
    }

    private async Task ValidatePolicyTypeRowAsync(BulkImportPreviewRowDTO row, long tenantId, CancellationToken token)
    {
        ValidateLength(row, "PolicyTypeCode", 50);
        ValidateLength(row, "PolicyName", 150);
        ValidateLength(row, "Description", 500);
        ValidateCurrency(row);
        ValidateBoolean(row, BulkImportConstants.IsActive);
        var categoryCode = row.Values["PolicyCategoryCode"].Trim().ToUpperInvariant();
        if (!await context.PolicyCategories.AsNoTracking().AnyAsync(
            x => x.CategoryCode == categoryCode && x.IsActive, token))
        {
            row.Errors.Add("PolicyCategoryCode must match an active policy category.");
        }
        var code = NormalizePolicyCode(row.Values["PolicyTypeCode"]);
        var existing = await context.PolicyTypes.AsNoTracking().FirstOrDefaultAsync(
            x => x.TenantId == tenantId && x.PolicyTypeCode == code && x.IsSoftDelete != true, token);
        if (existing != null)
        {
            row.ExistingId = existing.Id;
            row.Status = BulkImportRowStatus.Existing;
            row.WillReactivate = existing.IsActive != true && RequestedActive(row);
        }
    }

    private async Task ValidatePolicyDefinitionRowAsync(BulkImportPreviewRowDTO row, long tenantId, CancellationToken token)
    {
        ValidateLength(row, "PolicyCode", 50);
        ValidateLength(row, "PolicyName", 200);
        ValidateLength(row, "Summary", 1000);
        ValidateCurrency(row);
        if (!TryDate(row, "EffectiveFrom", true, out var from)
            || !TryDate(row, "EffectiveTo", false, out var to)
            || to.HasValue && to < from)
        {
            row.Errors.Add("EffectiveFrom/EffectiveTo must be ISO yyyy-MM-dd and form a valid range.");
        }
        var typeCode = NormalizePolicyCode(row.Values["PolicyTypeCode"]);
        if (!await context.PolicyTypes.AsNoTracking().AnyAsync(x => x.TenantId == tenantId
            && x.PolicyTypeCode == typeCode && x.IsActive == true && x.IsSoftDelete != true, token))
        {
            row.Errors.Add("PolicyTypeCode must match an active tenant policy type.");
        }
        ValidatePolicyJson(row);
        if (row.Errors.Count == 0)
        {
            var rules = DeserializeArray<PolicyRuleInputDTO>(row.Values["RulesJson"]);
            var scopes = DeserializeArray<PolicyApplicabilityInputDTO>(row.Values["ApplicabilityJson"]);
            await ValidatePolicyImportReferencesAsync(row, tenantId, rules, scopes, token);
        }
        var code = NormalizePolicyCode(row.Values["PolicyCode"]);
        var existing = await context.Policies.AsNoTracking().FirstOrDefaultAsync(
            x => x.TenantId == tenantId && x.PolicyCode == code && !x.IsSoftDeleted, token);
        if (existing != null)
        {
            row.HostRecordId = existing.Id;
            row.Status = BulkImportRowStatus.Existing;
        }
    }

    private async Task ValidatePolicyImportReferencesAsync(
        BulkImportPreviewRowDTO row,
        long tenantId,
        IReadOnlyCollection<PolicyRuleInputDTO> rules,
        IReadOnlyCollection<PolicyApplicabilityInputDTO> scopes,
        CancellationToken token)
    {
        var ruleTypeIds = rules.Select(x => x.PolicyRuleTypeId).Distinct().ToList();
        if (await context.PolicyRuleTypes.AsNoTracking()
            .CountAsync(x => ruleTypeIds.Contains(x.Id) && x.IsActive, token) != ruleTypeIds.Count)
        {
            row.Errors.Add("RulesJson contains an inactive or unknown PolicyRuleTypeId.");
        }

        foreach (var scope in scopes)
        {
            var state = scope.StateId.HasValue
                ? await context.States.AsNoTracking().FirstOrDefaultAsync(x => x.Id == scope.StateId && x.IsActive == true, token)
                : null;
            var district = scope.DistrictId.HasValue
                ? await context.Districts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == scope.DistrictId && x.IsActive, token)
                : null;
            var locality = scope.LocalityId.HasValue
                ? await context.Localities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == scope.LocalityId && x.IsActive == true, token)
                : null;
            var location = scope.TenantLocationId.HasValue
                ? await context.TenantLocations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == scope.TenantLocationId
                    && x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, token)
                : null;

            if (scope.CountryId.HasValue && !await context.Countries.AsNoTracking()
                .AnyAsync(x => x.Id == scope.CountryId && x.IsActive == true, token))
            {
                row.Errors.Add("ApplicabilityJson contains an invalid CountryId.");
            }
            if (scope.StateId.HasValue && (state == null
                || scope.CountryId.HasValue && state.CountryId != scope.CountryId))
            {
                row.Errors.Add("ApplicabilityJson contains an invalid StateId hierarchy.");
            }
            if (scope.DistrictId.HasValue && (district == null
                || scope.StateId.HasValue && district.StateId != scope.StateId))
            {
                row.Errors.Add("ApplicabilityJson contains an invalid DistrictId hierarchy.");
            }
            if (scope.LocalityId.HasValue && (locality == null
                || scope.DistrictId.HasValue && locality.DistrictId != scope.DistrictId
                || scope.StateId.HasValue && locality.StateId != scope.StateId))
            {
                row.Errors.Add("ApplicabilityJson contains an invalid LocalityId hierarchy.");
            }
            if (scope.TenantLocationId.HasValue && (location == null
                || scope.CountryId.HasValue && location.CountryId != scope.CountryId
                || scope.StateId.HasValue && location.StateId != scope.StateId
                || scope.DistrictId.HasValue && location.DistrictId != scope.DistrictId
                || scope.LocalityId.HasValue && location.LocalityId != scope.LocalityId))
            {
                row.Errors.Add("ApplicabilityJson contains a TenantLocationId outside the tenant or supplied geography.");
            }
            if (scope.EmployeeTypeId.HasValue && !await context.EmployeeTypes.AsNoTracking().AnyAsync(
                x => x.Id == scope.EmployeeTypeId && x.TenantId == tenantId && x.IsSoftDeleted != true, token))
            {
                row.Errors.Add("ApplicabilityJson contains an invalid EmployeeTypeId for this tenant.");
            }
            if (scope.DepartmentId.HasValue && !await context.Departments.AsNoTracking().AnyAsync(
                x => x.Id == scope.DepartmentId && x.TenantId == tenantId && !x.IsSoftDeleted, token))
            {
                row.Errors.Add("ApplicabilityJson contains an invalid DepartmentId for this tenant.");
            }
            if (scope.DesignationId.HasValue)
            {
                var designation = await context.Designations.AsNoTracking().FirstOrDefaultAsync(
                    x => x.Id == scope.DesignationId && x.TenantId == tenantId && !x.IsSoftDeleted, token);
                if (designation == null || scope.DepartmentId.HasValue && designation.DepartmentId != scope.DepartmentId)
                {
                    row.Errors.Add("ApplicabilityJson contains an invalid DesignationId hierarchy for this tenant.");
                }
            }
            if (scope.EmployeeId.HasValue && !await context.Employees.AsNoTracking().AnyAsync(
                x => x.Id == scope.EmployeeId && x.TenantId == tenantId, token))
            {
                row.Errors.Add("ApplicabilityJson contains an invalid EmployeeId for this tenant.");
            }
            if (scope.GenderId.HasValue && !await context.Genders.AsNoTracking().AnyAsync(x => x.Id == scope.GenderId, token))
            {
                row.Errors.Add("ApplicabilityJson contains an invalid GenderId.");
            }
        }
    }

    private async Task ValidatePolicyAssignmentRowAsync(BulkImportPreviewRowDTO row, long tenantId, CancellationToken token)
    {
        if (!int.TryParse(row.Values["VersionNumber"], out var versionNumber) || versionNumber < 1)
        {
            row.Errors.Add("VersionNumber must be a positive integer.");
            return;
        }
        if (!TryDate(row, "EffectiveFrom", true, out var from)
            || !TryDate(row, "EffectiveTo", false, out var to)
            || to.HasValue && to < from)
        {
            row.Errors.Add("EffectiveFrom/EffectiveTo must be ISO yyyy-MM-dd and form a valid range.");
        }
        ValidateBoolean(row, "IsMandatory");
        var policyCode = NormalizePolicyCode(row.Values["PolicyCode"]);
        var version = await (from policy in context.Policies.AsNoTracking()
                             join candidate in context.PolicyVersions.AsNoTracking() on policy.Id equals candidate.PolicyId
                             where policy.TenantId == tenantId && policy.PolicyCode == policyCode
                                && !policy.IsSoftDeleted && candidate.TenantId == tenantId
                                && candidate.VersionNumber == versionNumber && candidate.PolicyStatusId == 4
                             select candidate).FirstOrDefaultAsync(token);
        if (version == null)
        {
            row.Errors.Add("PolicyCode and VersionNumber must identify a published tenant policy version.");
        }
        else
        {
            row.PolicyVersionId = version.Id;
        }
        var employeeCode = row.Values["EmployeeCode"].Trim().ToUpperInvariant();
        var employee = await context.Employees.AsNoTracking().FirstOrDefaultAsync(
            x => x.TenantId == tenantId && x.EmployementCode != null
                && x.EmployementCode.ToUpper() == employeeCode, token);
        if (employee == null)
        {
            row.Errors.Add("EmployeeCode must match an employee in this tenant.");
        }
        else
        {
            row.TargetEmployeeId = employee.Id;
        }
        if (version != null && employee != null && from.HasValue
            && await context.PolicyAssignments.AsNoTracking().AnyAsync(x => x.TenantId == tenantId
                && x.PolicyVersionId == version.Id && x.EmployeeId == employee.Id
                && x.EffectiveFrom == from.Value && x.IsActive, token))
        {
            row.Status = BulkImportRowStatus.Existing;
        }
    }

    private async Task ProcessPolicyBatchAsync(
        BulkImportJob job,
        BulkImportPreviewResponseDTO preview,
        CancellationToken cancellationToken)
    {
        var end = Math.Min(preview.Rows.Count, job.NextRow + Math.Clamp(options.Value.BatchSize, 1, 200));
        for (var index = job.NextRow; index < end; index++)
        {
            var row = preview.Rows[index];
            if (row.Status == BulkImportRowStatus.Created)
            {
                job.NextRow = index + 1;
                continue;
            }
            var table = new BulkImportTableDTO
            {
                Columns = row.Values.Keys.ToList(),
                Rows = [new BulkImportSourceRowDTO { RowNumber = row.RowNumber, Values = row.Values.Values.ToList() }]
            };
            var fresh = (await BuildPolicyPreviewAsync((BulkImportMaster)job.Master, table, null,
                job.TenantId, cancellationToken)).Rows[0];
            preview.Rows[index] = fresh;
            if (fresh.Status == BulkImportRowStatus.Ready)
            {
                await context.Database.CurrentTransaction!.CreateSavepointAsync("policy_bulk_row", cancellationToken);
                try
                {
                    await InsertPolicyRowAsync(job, fresh, cancellationToken);
                    fresh.Status = BulkImportRowStatus.Created;
                    await context.Database.CurrentTransaction.ReleaseSavepointAsync("policy_bulk_row", cancellationToken);
                }
                catch (Exception error) when (error is ValidationErrorException
                    || error is DbUpdateException { InnerException: PostgresException })
                {
                    await context.Database.CurrentTransaction.RollbackToSavepointAsync("policy_bulk_row", cancellationToken);
                    context.ChangeTracker.Clear();
                    fresh.Status = BulkImportRowStatus.Failed;
                    fresh.Errors.Add("Data changed during import. Create a fresh preview and retry.");
                    await context.Database.CurrentTransaction.ReleaseSavepointAsync("policy_bulk_row", cancellationToken);
                }
            }
            else if (fresh.Status == BulkImportRowStatus.Existing && fresh.WillReactivate
                && job.Master == (int)BulkImportMaster.PolicyType)
            {
                var affected = await context.PolicyTypes.Where(x => x.Id == fresh.ExistingId
                        && x.TenantId == job.TenantId && x.IsSoftDelete != true && x.IsActive != true)
                    .ExecuteUpdateAsync(update => update.SetProperty(x => x.IsActive, true)
                        .SetProperty(x => x.UpdateById, job.ActorId)
                        .SetProperty(x => x.UpdateDateTime, DateTime.UtcNow), cancellationToken);
                fresh.WasReactivated = affected == 1;
            }
            if (fresh.Status == BulkImportRowStatus.Invalid)
            {
                fresh.Status = BulkImportRowStatus.Failed;
            }
            fresh.Processed = true;
            job.NextRow = index + 1;
        }
        job.Status = job.NextRow >= preview.Rows.Count
            ? (int)(preview.Rows.Any(x => x.Status == BulkImportRowStatus.Failed)
                ? BulkImportJobStatus.CompletedWithErrors : BulkImportJobStatus.Completed)
            : (int)BulkImportJobStatus.Running;
        await SaveProgress(job, preview, cancellationToken);
    }

    private async Task InsertPolicyRowAsync(BulkImportJob job, BulkImportPreviewRowDTO row, CancellationToken token)
    {
        if (job.Master == (int)BulkImportMaster.PolicyType)
        {
            var categoryCode = row.Values["PolicyCategoryCode"].Trim().ToUpperInvariant();
            var categoryId = await context.PolicyCategories.Where(x => x.CategoryCode == categoryCode && x.IsActive)
                .Select(x => x.Id).SingleAsync(token);
            var entity = new PolicyType
            {
                TenantId = job.TenantId,
                PolicyTypeCode = NormalizePolicyCode(row.Values["PolicyTypeCode"]),
                PolicyName = row.Values["PolicyName"].Trim(),
                PolicyCategoryId = categoryId,
                Description = EmptyToNull(row.Values.GetValueOrDefault("Description")),
                DefaultCurrencyCode = NormalizeCurrency(row.Values.GetValueOrDefault("DefaultCurrencyCode")),
                IsStructured = true,
                IsActive = RequestedActive(row),
                IsSoftDelete = false,
                AddedById = job.ActorId,
                AddedDateTime = DateTime.UtcNow
            };
            context.PolicyTypes.Add(entity);
            await context.SaveChangesAsync(token);
            row.ExistingId = entity.Id;
            return;
        }
        if (job.Master == (int)BulkImportMaster.PolicyDefinition)
        {
            await InsertPolicyDefinitionAsync(job, row, token);
            return;
        }

        var from = DateOnly.ParseExact(row.Values["EffectiveFrom"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var assignment = new PolicyAssignment
        {
            TenantId = job.TenantId,
            PolicyVersionId = row.PolicyVersionId!.Value,
            EmployeeId = row.TargetEmployeeId!.Value,
            AssignmentSource = 2,
            EffectiveFrom = from,
            EffectiveTo = ParseOptionalDate(row.Values.GetValueOrDefault("EffectiveTo")),
            IsMandatory = ParseOptionalBoolean(row.Values.GetValueOrDefault("IsMandatory"), true),
            IsActive = true,
            AssignedById = job.ActorId,
            AssignedDateTime = DateTime.UtcNow
        };
        context.PolicyAssignments.Add(assignment);
        if (!await context.PolicyAcknowledgements.AnyAsync(x => x.PolicyVersionId == assignment.PolicyVersionId
            && x.EmployeeId == assignment.EmployeeId, token))
        {
            context.PolicyAcknowledgements.Add(new PolicyAcknowledgement
            {
                TenantId = job.TenantId,
                PolicyVersionId = assignment.PolicyVersionId,
                EmployeeId = assignment.EmployeeId,
                AcknowledgementStatus = 1,
                AssignedDateTime = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync(token);
        row.HostRecordId = assignment.Id;
    }

    private async Task InsertPolicyDefinitionAsync(BulkImportJob job, BulkImportPreviewRowDTO row, CancellationToken token)
    {
        var typeCode = NormalizePolicyCode(row.Values["PolicyTypeCode"]);
        var typeId = await context.PolicyTypes.Where(x => x.TenantId == job.TenantId
                && x.PolicyTypeCode == typeCode && x.IsActive == true && x.IsSoftDelete != true)
            .Select(x => x.Id).SingleAsync(token);
        var now = DateTime.UtcNow;
        var policy = new Policy
        {
            TenantId = job.TenantId,
            PolicyTypeId = typeId,
            PolicyCode = NormalizePolicyCode(row.Values["PolicyCode"]),
            PolicyName = row.Values["PolicyName"].Trim(),
            Summary = EmptyToNull(row.Values.GetValueOrDefault("Summary")),
            DefaultCurrencyCode = NormalizeCurrency(row.Values.GetValueOrDefault("DefaultCurrencyCode")),
            IsActive = true,
            AddedById = job.ActorId,
            AddedDateTime = now
        };
        context.Policies.Add(policy);
        await context.SaveChangesAsync(token);
        var version = new PolicyVersion
        {
            TenantId = job.TenantId,
            PolicyId = policy.Id,
            VersionNumber = 1,
            PolicyStatusId = 1,
            EffectiveFrom = DateOnly.ParseExact(row.Values["EffectiveFrom"], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            EffectiveTo = ParseOptionalDate(row.Values.GetValueOrDefault("EffectiveTo")),
            RuleSchemaVersion = 1,
            IsActive = true,
            AddedById = job.ActorId,
            AddedDateTime = now
        };
        context.PolicyVersions.Add(version);
        await context.SaveChangesAsync(token);
        var rules = DeserializeArray<PolicyRuleInputDTO>(row.Values["RulesJson"]);
        var scopes = DeserializeArray<PolicyApplicabilityInputDTO>(row.Values["ApplicabilityJson"]);
        context.PolicyRules.AddRange(rules.Select(x => new PolicyRule
        {
            TenantId = job.TenantId,
            PolicyVersionId = version.Id,
            PolicyRuleTypeId = x.PolicyRuleTypeId,
            RuleName = x.RuleName.Trim(),
            RuleOrder = x.RuleOrder,
            RuleConfiguration = x.RuleConfiguration,
            IsActive = true,
            AddedById = job.ActorId,
            AddedDateTime = now
        }));
        context.PolicyApplicabilities.AddRange(scopes.Select(x => new PolicyApplicability
        {
            TenantId = job.TenantId,
            PolicyVersionId = version.Id,
            ApplicabilityMode = x.ApplicabilityMode,
            CountryId = x.CountryId,
            StateId = x.StateId,
            DistrictId = x.DistrictId,
            LocalityId = x.LocalityId,
            TenantLocationId = x.TenantLocationId,
            EmployeeTypeId = x.EmployeeTypeId,
            DepartmentId = x.DepartmentId,
            DesignationId = x.DesignationId,
            EmployeeId = x.EmployeeId,
            GenderId = x.GenderId,
            WorkArrangementType = x.WorkArrangementType,
            EmploymentStatus = x.EmploymentStatus,
            MinimumServiceDays = x.MinimumServiceDays,
            Priority = x.Priority,
            EffectiveFrom = x.EffectiveFrom,
            EffectiveTo = x.EffectiveTo,
            IsActive = true,
            AddedById = job.ActorId,
            AddedDateTime = now
        }));
        context.PolicyChangeAudits.Add(new PolicyChangeAudit
        {
            TenantId = job.TenantId,
            PolicyId = policy.Id,
            PolicyVersionId = version.Id,
            EntityName = "Policy",
            EntityId = policy.Id,
            ActionName = "BULK_CREATE",
            ChangedById = job.ActorId,
            ChangedDateTime = now,
            CorrelationId = job.Id
        });
        await context.SaveChangesAsync(token);
        row.HostRecordId = policy.Id;
        row.PolicyVersionId = version.Id;
    }

    private static IReadOnlyCollection<string> PolicyColumns(BulkImportMaster master) => master switch
    {
        BulkImportMaster.PolicyType => BulkImportConstants.PolicyTypeColumns,
        BulkImportMaster.PolicyDefinition => BulkImportConstants.PolicyDefinitionColumns,
        BulkImportMaster.PolicyAssignment => BulkImportConstants.PolicyAssignmentColumns,
        _ => throw new ValidationErrorException("Unsupported policy import target.")
    };

    private static void ValidatePolicyJson(BulkImportPreviewRowDTO row)
    {
        try
        {
            var rules = DeserializeArray<PolicyRuleInputDTO>(row.Values["RulesJson"]);
            var scopes = DeserializeArray<PolicyApplicabilityInputDTO>(row.Values["ApplicabilityJson"]);
            if (rules.GroupBy(x => x.RuleOrder).Any(x => x.Count() > 1)
                || rules.Any(x => x.PolicyRuleTypeId <= 0 || string.IsNullOrWhiteSpace(x.RuleName)))
            {
                throw new JsonException();
            }
            foreach (var rule in rules)
            {
                using var document = JsonDocument.Parse(rule.RuleConfiguration);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new JsonException();
                }
            }
            if (scopes.Any(x => x.ApplicabilityMode is not (1 or 2)
                || x.EffectiveFrom == default || x.EffectiveTo.HasValue && x.EffectiveTo < x.EffectiveFrom))
            {
                throw new JsonException();
            }
        }
        catch (JsonException)
        {
            row.Errors.Add("RulesJson and ApplicabilityJson must match the documented JSON-array schema.");
        }
    }

    private static List<T> DeserializeArray<T>(string value)
    {
        return JsonSerializer.Deserialize<List<T>>(value, PolicyJsonOptions) ?? throw new JsonException();
    }

    private static void ValidateLength(BulkImportPreviewRowDTO row, string field, int maximum)
    {
        if (row.Values.GetValueOrDefault(field, string.Empty).Length > maximum)
        {
            row.Errors.Add($"{field} exceeds {maximum} characters.");
        }
    }

    private static void ValidateCurrency(BulkImportPreviewRowDTO row)
    {
        var currency = row.Values.GetValueOrDefault("DefaultCurrencyCode");
        if (!string.IsNullOrWhiteSpace(currency) && currency.Trim().Length != 3)
        {
            row.Errors.Add("DefaultCurrencyCode must be a three-character ISO code.");
        }
    }

    private static void ValidateBoolean(BulkImportPreviewRowDTO row, string field)
    {
        var value = row.Values.GetValueOrDefault(field);
        if (!string.IsNullOrWhiteSpace(value) && !bool.TryParse(value, out _))
        {
            row.Errors.Add($"{field} must be true or false.");
        }
    }

    private static bool TryDate(BulkImportPreviewRowDTO row, string field, bool required, out DateOnly? value)
    {
        var raw = row.Values.GetValueOrDefault(field);
        if (string.IsNullOrWhiteSpace(raw))
        {
            value = null;
            return !required;
        }
        var success = DateOnly.TryParseExact(raw, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var parsed);
        value = success ? parsed : null;
        return success;
    }

    private static string NormalizePolicyCode(string value) => value.Trim().ToUpperInvariant().Replace(' ', '_');
    private static string? NormalizeCurrency(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    private static string? EmptyToNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static bool RequestedActive(BulkImportPreviewRowDTO row) =>
        ParseOptionalBoolean(row.Values.GetValueOrDefault(BulkImportConstants.IsActive), true);
    private static bool ParseOptionalBoolean(string? value, bool defaultValue) =>
        string.IsNullOrWhiteSpace(value) ? defaultValue : bool.Parse(value);
    private static DateOnly? ParseOptionalDate(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
}

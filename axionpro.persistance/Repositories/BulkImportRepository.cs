using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Models;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace axionpro.persistance.Repositories;

/// <summary>Durable PostgreSQL queue; a job lock, its business inserts and cursor share one transaction.</summary>
public sealed partial class BulkImportRepository(
    WorkforceDbContext context,
    IMapper mapper,
    IStoreProcedureRepository permissions,
    IOptions<BulkImportOptions> options,
    IBaseEmployeeRepository? employeeRepository = null) : IBulkImportRepository
{
    #region Draft and user actions

    public async Task<BulkImportPreviewResponseDTO> SaveDraftAsync(
        BulkImportPreviewResponseDTO preview,
        BulkImportPreviewRequestDTO request,
        CommonDecodedResult actor,
        CancellationToken cancellationToken)
    {
        var id = request.RequestId ?? Guid.NewGuid();
        if (id == Guid.Empty)
        {
            throw new ValidationErrorException("RequestId must not be empty.");
        }
        var sourceHash = request.File is null
            ? SHA256.HashData(Encoding.UTF8.GetBytes(request.PastedText ?? string.Empty))
            : await HashFile(request, cancellationToken);
        var identity = $"{(int)preview.Master}:{Convert.ToHexString(sourceHash)}:{request.SheetName}:{request.ColumnMappingJson}";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)));
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        // Serialize repeated HTTP previews using the same client RequestId across all server instances.
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtextextended({id.ToString()}, 0))", cancellationToken);
        var existing = await context.Set<BulkImportJob>().AsNoTracking()
            .SingleOrDefaultAsync(job => job.Id == id, cancellationToken);
        if (existing is not null)
        {
            if (existing.TenantId != actor.TenantId || existing.ActorId != actor.LoggedInEmployeeId ||
                existing.Master != (int)preview.Master || existing.InputHash != hash)
            {
                throw new ConflictException("RequestId already belongs to a different preview. Use a new RequestId for corrected data.");
            }
            var saved = Deserialize(existing);
            RemovePrivateEmployeeFields(saved);
            saved.JobId = id;
            saved.ConfirmationAvailable = existing.Status == (int)BulkImportJobStatus.Draft;
            await transaction.CommitAsync(cancellationToken);
            return saved;
        }
        preview.JobId = id;
        preview.ConfirmationAvailable = true;
        context.Add(new BulkImportJob
        {
            Id = id,
            TenantId = actor.TenantId,
            ActorId = actor.LoggedInEmployeeId,
            RoleId = actor.RoleId,
            ModuleId = request.ModuleId,
            OperationId = request.OperationId,
            Master = (int)preview.Master,
            Status = (int)BulkImportJobStatus.Draft,
            PreviewJson = JsonSerializer.Serialize(preview),
            InputHash = hash,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        });
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return preview;
    }

    public async Task<List<BulkImportJobResponseDTO>> ListAsync(
        BulkImportMaster master, BulkImportJobRequestDTO request,
        CommonDecodedResult actor, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1 || request.PageSize is < 1 or > 100)
        {
            throw new ValidationErrorException("PageNumber must be positive and PageSize must be 1 to 100.");
        }
        var jobs = await Owned(master, actor).OrderByDescending(job => job.CreatedAtUtc)
            .Skip(checked((request.PageNumber - 1) * request.PageSize)).Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return jobs.Select(job => Response(job, includeRows: false)).ToList();
    }

    public async Task<BulkImportJobResponseDTO> ActAsync(
        BulkImportMaster master, BulkImportAction action,
        BulkImportJobRequestDTO request, CommonDecodedResult actor,
        CancellationToken cancellationToken)
    {
        if (request.JobId == Guid.Empty)
        {
            throw new ValidationErrorException("JobId is required.");
        }
        if (action == BulkImportAction.Get)
        {
            var found = await Owned(master, actor).SingleOrDefaultAsync(job => job.Id == request.JobId, cancellationToken)
                ?? throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
            return Response(found);
        }
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var jobs = await context.Set<BulkImportJob>().FromSqlInterpolated($"""
            SELECT * FROM axionpro."BulkImportJob"
            WHERE "Id" = {request.JobId} AND "TenantId" = {actor.TenantId}
              AND "ActorId" = {actor.LoggedInEmployeeId} AND "Master" = {(int)master}
            FOR UPDATE
            """).ToListAsync(cancellationToken);
        var job = jobs.SingleOrDefault() ?? throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        var preview = Deserialize(job);
        switch (action)
        {
            case BulkImportAction.Confirm:
                if (job.Status != (int)BulkImportJobStatus.Draft)
                {
                    // Confirmation retries return the already-created job, never enqueue another.
                    break;
                }
                if (!preview.IsValid)
                {
                    throw new ValidationErrorException("Correct all invalid rows and submit a new preview before confirmation.");
                }
                if (master == BulkImportMaster.Employee)
                {
                    await ConfirmEmployeesAsync(job, preview, cancellationToken);
                }
                Queue(job, request, actor);
                break;
            case BulkImportAction.Retry:
                if (job.Status is not ((int)BulkImportJobStatus.Failed) and not ((int)BulkImportJobStatus.CompletedWithErrors))
                {
                    throw new ConflictException("Only failed jobs or jobs completed with errors can be retried.");
                }
                foreach (var row in preview.Rows.Where(row => row.Status == BulkImportRowStatus.Failed))
                {
                    row.Status = BulkImportRowStatus.Ready;
                    row.Processed = false;
                    row.Errors.Clear();
                }
                job.PreviewJson = JsonSerializer.Serialize(preview);
                job.NextRow = 0;
                Queue(job, request, actor);
                break;
            case BulkImportAction.Cancel:
                if (job.Status is (int)BulkImportJobStatus.Draft or (int)BulkImportJobStatus.Queued or (int)BulkImportJobStatus.Running)
                {
                    job.Status = (int)BulkImportJobStatus.Cancelled;
                }
                break;
            default:
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }
        job.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Response(job);
    }

    private static void Queue(BulkImportJob job, BulkImportJobRequestDTO request, CommonDecodedResult actor)
    {
        if (request.ScheduledAtUtc.HasValue && request.ScheduledAtUtc.Value <= DateTimeOffset.UtcNow)
        {
            throw new ValidationErrorException("ScheduledAtUtc must be in the future, or omitted for Run now.");
        }
        job.Status = (int)BulkImportJobStatus.Queued;
        job.ScheduledAtUtc = request.ScheduledAtUtc?.UtcDateTime ?? DateTime.UtcNow;
        job.RoleId = actor.RoleId;
        job.ModuleId = request.ModuleId;
        job.OperationId = request.OperationId;
        job.Error = null;
    }

    private IQueryable<BulkImportJob> Owned(BulkImportMaster master, CommonDecodedResult actor)
    {
        return context.Set<BulkImportJob>().AsNoTracking().Where(job =>
            job.TenantId == actor.TenantId && job.ActorId == actor.LoggedInEmployeeId && job.Master == (int)master);
    }

    #endregion

    #region Durable worker

    public async Task<bool> ProcessNextBatchAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var jobs = await context.Set<BulkImportJob>().FromSqlInterpolated($"""
            SELECT * FROM axionpro."BulkImportJob"
            WHERE "Status" IN ({(int)BulkImportJobStatus.Queued}, {(int)BulkImportJobStatus.Running})
              AND "ScheduledAtUtc" <= {now}
            ORDER BY "UpdatedAtUtc", "CreatedAtUtc"
            LIMIT 1 FOR UPDATE SKIP LOCKED
            """).AsNoTracking().ToListAsync(cancellationToken);
        var job = jobs.SingleOrDefault();
        if (job is null)
        {
            return false;
        }
        var preview = Deserialize(job);
        try
        {
            await EnsureWorkerPermissionAsync(job, cancellationToken);
        }
        catch (Exception error) when (error is UnauthorizedAccessException or ForbiddenAccessException or ValidationErrorException)
        {
            job.Status = (int)BulkImportJobStatus.Failed;
            job.Error = "Import permission is no longer available. Restore access and retry.";
            await SaveProgress(job, preview, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        // Cross-job serialization by tenant/master plus unique DB indexes also protects manual-create races.
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtextextended({$"bulk:{job.TenantId}:{job.Master}"}, 0))", cancellationToken);
        if (job.Master == (int)BulkImportMaster.Employee)
        {
            await ProcessEmployeeBatchAsync(job, preview, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        var current = await CurrentMasters(job, cancellationToken);
        var end = Math.Min(preview.Rows.Count, job.NextRow + Math.Clamp(options.Value.BatchSize, 1, 200));
        for (var index = job.NextRow; index < end; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var row = preview.Rows[index];
            // Created rows are never repeated, even when a failed job is retried from cursor zero.
            if (row.Status == BulkImportRowStatus.Created)
            {
                job.NextRow = index + 1;
                continue;
            }
            var single = new BulkImportTableDTO
            {
                Columns = row.Values.Keys.ToList(),
                Rows = new List<BulkImportSourceRowDTO>
                {
                    new() { RowNumber = row.RowNumber, Values = row.Values.Values.ToList() }
                }
            };
            var fresh = BulkImportPreviewService.Build((BulkImportMaster)job.Master, null, single,
                current.Departments, current.Designations, current.Roles, current.EmployeeTypes).Rows[0];
            if (row.DepartmentId.HasValue && fresh.DepartmentId.HasValue && row.DepartmentId != fresh.DepartmentId)
            {
                fresh.Status = BulkImportRowStatus.Invalid;
                fresh.DepartmentId = row.DepartmentId;
                fresh.Errors.Add("Matched department changed since preview. Create a new preview to confirm the new department.");
            }
            preview.Rows[index] = fresh;
            if (fresh.Status == BulkImportRowStatus.Invalid)
            {
                fresh.Status = BulkImportRowStatus.Failed;
                fresh.DepartmentId ??= row.DepartmentId;
            }
            else if (fresh.Status == BulkImportRowStatus.Ready)
            {
                await transaction.CreateSavepointAsync("bulk_row", cancellationToken);
                try
                {
                    await Insert(job, fresh, cancellationToken);
                    fresh.Status = BulkImportRowStatus.Created;
                    // Later rows/batches must see every committed-to-this-transaction new master.
                    if (job.Master == (int)BulkImportMaster.Department)
                    {
                        current.Departments.Add(new GetDepartmentResponseDTO
                        {
                            Id = fresh.ExistingId!.Value,
                            DepartmentName = fresh.Values[BulkImportConstants.DepartmentName],
                            IsActive = IsActive(fresh)
                        });
                    }
                    else if (job.Master == (int)BulkImportMaster.Designation)
                    {
                        current.Designations.Add(new GetDesignationResponseDTO
                        {
                            Id = fresh.ExistingId!.Value,
                            DepartmentId = fresh.DepartmentId!.Value,
                            DesignationName = fresh.Values[BulkImportConstants.DesignationName],
                            IsActive = IsActive(fresh)
                        });
                    }
                    else if (job.Master == (int)BulkImportMaster.EmployeeType)
                    {
                        current.EmployeeTypes.Add(new axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO
                        {
                            Id = fresh.ExistingId!.Value,
                            TypeName = fresh.Values[BulkImportConstants.TypeName],
                            IsActive = IsActive(fresh)
                        });
                    }
                    else
                    {
                        current.Roles.Add(new GetRoleResponseDTO
                        {
                            Id = fresh.ExistingId!.Value,
                            RoleName = fresh.Values[BulkImportConstants.RoleName],
                            RoleType = int.Parse(fresh.Values[BulkImportConstants.RoleType]),
                            IsActive = IsActive(fresh)
                        });
                    }
                    await transaction.ReleaseSavepointAsync("bulk_row", cancellationToken);
                }
                catch (Exception error) when (error is ValidationErrorException ||
                    (error is DbUpdateException updateError && updateError.InnerException is PostgresException pg &&
                    pg.SqlState is PostgresErrorCodes.UniqueViolation or PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.CheckViolation))
                {
                    await transaction.RollbackToSavepointAsync("bulk_row", cancellationToken);
                    context.ChangeTracker.Clear();
                    fresh.Status = BulkImportRowStatus.Failed;
                    fresh.Errors.Add("Data changed during import. Review the existing record/reference and retry.");
                    await transaction.ReleaseSavepointAsync("bulk_row", cancellationToken);
                }
            }
            fresh.Processed = true;
            job.NextRow = index + 1;
        }
        job.Status = job.NextRow >= preview.Rows.Count
            ? (int)(preview.Rows.Any(row => row.Status == BulkImportRowStatus.Failed)
                ? BulkImportJobStatus.CompletedWithErrors : BulkImportJobStatus.Completed)
            : (int)BulkImportJobStatus.Running;
        await SaveProgress(job, preview, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    private async Task EnsureWorkerPermissionAsync(BulkImportJob job, CancellationToken cancellationToken)
    {
        var module = await context.Modules.AsNoTracking().SingleOrDefaultAsync(item => item.Id == job.ModuleId, cancellationToken);
        var expected = (BulkImportMaster)job.Master switch
        {
            BulkImportMaster.Department => "TENANT_DEPARTMENTS",
            BulkImportMaster.Designation => "TENANT_DESIGNATIONS",
            BulkImportMaster.EmployeeType => BulkImportConstants.EmployeeTypeModuleCode,
            BulkImportMaster.Employee => BulkImportConstants.EmployeeModuleCode,
            _ => "TENANT_ROLES_PERMISSIONS"
        };
        var operation = await context.Operations.AsNoTracking().SingleOrDefaultAsync(item => item.Id == job.OperationId, cancellationToken);
        if (module?.ModuleCode != expected || operation?.IsActive != true ||
            (operation.OperationType != (int)OperationType.Add && operation.OperationType != (int)OperationType.Import))
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }
        // Same persisted permission function and result validator as the existing MediatR pipelines.
        var result = await permissions.CheckTenantEmployeePermissionAsync(
            job.TenantId, job.ActorId, job.RoleId, job.ModuleId, job.OperationId, cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(result);
    }

    private async Task Insert(BulkImportJob job, BulkImportPreviewRowDTO row, CancellationToken cancellationToken)
    {
        var values = row.Values;
        var active = IsActive(row);
        if (job.Master == (int)BulkImportMaster.Department)
        {
            var entity = mapper.Map<Department>(new CreateDepartmentRequestDTO
            {
                UserEmployeeId = string.Empty,
                DepartmentName = values[BulkImportConstants.DepartmentName],
                Description = values.GetValueOrDefault(BulkImportConstants.Description),
                Remark = values.GetValueOrDefault(BulkImportConstants.Remark),
                IsActive = active
            });
            entity.TenantId = job.TenantId;
            entity.AddedById = job.ActorId;
            entity.AddedDateTime = DateTime.UtcNow;
            entity.IsExecutiveOffice = false;
            entity.IsSoftDeleted = false;
            context.Departments.Add(entity);
            await context.SaveChangesAsync(cancellationToken);
            row.ExistingId = entity.Id;
        }
        else if (job.Master == (int)BulkImportMaster.Designation)
        {
            var parents = await context.Departments.FromSqlInterpolated($"""
                SELECT * FROM axionpro."Department"
                WHERE "Id" = {row.DepartmentId!.Value} AND "TenantId" = {job.TenantId}
                  AND "IsActive" = TRUE AND "IsSoftDeleted" IS NOT TRUE
                FOR SHARE
                """).AsNoTracking().ToListAsync(cancellationToken);
            if (parents.Count != 1)
            {
                throw new ValidationErrorException("Department is no longer active or available.");
            }
            var entity = mapper.Map<Designation>(new CreateDesignationRequestDTO
            {
                DepartmentId = row.DepartmentId!.Value,
                DesignationName = values[BulkImportConstants.DesignationName],
                Description = values.GetValueOrDefault(BulkImportConstants.Description),
                IsActive = active
            });
            entity.TenantId = job.TenantId;
            entity.AddedById = job.ActorId;
            entity.AddedDateTime = DateTime.UtcNow;
            entity.IsSoftDeleted = false;
            context.Designations.Add(entity);
            await context.SaveChangesAsync(cancellationToken);
            row.ExistingId = entity.Id;
        }
        else if (job.Master == (int)BulkImportMaster.EmployeeType)
        {
            var entity = mapper.Map<EmployeeType>(new axionpro.application.DTOS.Employee.Type.CreateEmployeeTypeDTO
            {
                TypeName = values[BulkImportConstants.TypeName],
                Description = values.GetValueOrDefault(BulkImportConstants.Description),
                Remark = values.GetValueOrDefault(BulkImportConstants.Remark),
                IsActive = active
            });
            entity.TenantId = job.TenantId;
            entity.AddedById = job.ActorId;
            entity.AddedDateTime = DateTime.UtcNow;
            entity.IsSoftDeleted = false;
            context.EmployeeTypes.Add(entity);
            await context.SaveChangesAsync(cancellationToken);
            row.ExistingId = entity.Id;
        }
        else
        {
            var entity = mapper.Map<Role>(new CreateRoleRequestDTO
            {
                RoleName = values[BulkImportConstants.RoleName],
                RoleType = int.Parse(values[BulkImportConstants.RoleType]),
                Remark = values.GetValueOrDefault(BulkImportConstants.Remark),
                IsActive = active
            });
            entity.TenantId = job.TenantId;
            entity.AddedById = job.ActorId;
            entity.AddedDateTime = DateTime.UtcNow;
            entity.IsSystemDefault = false;
            entity.IsSoftDeleted = false;
            context.Roles.Add(entity);
            await context.SaveChangesAsync(cancellationToken);
            row.ExistingId = entity.Id;
        }
        context.ChangeTracker.Clear();
    }

    private async Task<(List<GetDepartmentResponseDTO> Departments, List<GetDesignationResponseDTO> Designations,
        List<GetRoleResponseDTO> Roles, List<axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO> EmployeeTypes)> CurrentMasters(BulkImportJob job, CancellationToken cancellationToken)
    {
        var departments = await context.Departments.AsNoTracking()
            .Where(item => item.TenantId == job.TenantId && !item.IsSoftDeleted)
            .Select(item => new GetDepartmentResponseDTO { Id = item.Id, DepartmentName = item.DepartmentName, IsActive = item.IsActive })
            .ToListAsync(cancellationToken);
        var designations = job.Master == (int)BulkImportMaster.Designation
            ? await context.Designations.AsNoTracking()
                .Where(item => item.TenantId == job.TenantId && !item.IsSoftDeleted)
                .Select(item => new GetDesignationResponseDTO
                {
                    Id = item.Id, DepartmentId = item.DepartmentId,
                    DesignationName = item.DesignationName, IsActive = item.IsActive
                }).ToListAsync(cancellationToken)
            : new List<GetDesignationResponseDTO>();
        var roles = job.Master == (int)BulkImportMaster.Role
            ? await context.Roles.AsNoTracking()
                .Where(item => item.TenantId == job.TenantId && item.IsSoftDeleted != true)
                .Select(item => new GetRoleResponseDTO
                {
                    Id = item.Id, RoleName = item.RoleName, RoleType = item.RoleType, IsActive = item.IsActive
                }).ToListAsync(cancellationToken)
            : new List<GetRoleResponseDTO>();
        var employeeTypes = job.Master == (int)BulkImportMaster.EmployeeType
            ? await context.EmployeeTypes.AsNoTracking()
                .Where(item => item.TenantId == job.TenantId && item.IsSoftDeleted != true)
                .Select(item => new axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO
                {
                    Id = item.Id, TypeName = item.TypeName, IsActive = item.IsActive
                }).ToListAsync(cancellationToken)
            : new List<axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO>();
        return (departments, designations, roles, employeeTypes);
    }

    private async Task SaveProgress(BulkImportJob job, BulkImportPreviewResponseDTO preview, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(preview);
        var now = DateTime.UtcNow;
        await context.Set<BulkImportJob>().Where(item => item.Id == job.Id).ExecuteUpdateAsync(update => update
            .SetProperty(item => item.PreviewJson, json)
            .SetProperty(item => item.Status, job.Status)
            .SetProperty(item => item.NextRow, job.NextRow)
            .SetProperty(item => item.Error, job.Error)
            .SetProperty(item => item.UpdatedAtUtc, now), cancellationToken);
    }

    private static BulkImportPreviewResponseDTO Deserialize(BulkImportJob job)
    {
        return JsonSerializer.Deserialize<BulkImportPreviewResponseDTO>(job.PreviewJson)
            ?? throw new InvalidOperationException("Stored import snapshot is invalid.");
    }

    private static BulkImportJobResponseDTO Response(BulkImportJob job, bool includeRows = true)
    {
        var preview = Deserialize(job);
        preview.ConfirmationAvailable = job.Status == (int)BulkImportJobStatus.Draft;
        RemovePrivateEmployeeFields(preview);
        return new BulkImportJobResponseDTO
        {
            JobId = job.Id,
            Master = (BulkImportMaster)job.Master,
            Status = (BulkImportJobStatus)job.Status,
            CreatedAtUtc = job.CreatedAtUtc,
            UpdatedAtUtc = job.UpdatedAtUtc,
            ScheduledAtUtc = job.ScheduledAtUtc,
            TotalRows = preview.Rows.Count,
            ProcessedRows = preview.Rows.Count(row => row.Processed),
            CreatedCount = preview.Rows.Count(row => row.Status == BulkImportRowStatus.Created),
            ExistingCount = preview.Rows.Count(row => row.Processed && row.Status == BulkImportRowStatus.Existing),
            FailedCount = preview.Rows.Count(row => row.Status == BulkImportRowStatus.Failed),
            Error = job.Error,
            Preview = includeRows ? preview : null
        };
    }

    private static void RemovePrivateEmployeeFields(BulkImportPreviewResponseDTO preview)
    {
        foreach (var row in preview.Rows)
        {
            row.ImportedEmployeeId = null;
            row.InvitationAttemptId = null;
        }
    }

    #endregion

    private static bool IsActive(BulkImportPreviewRowDTO row)
    {
        return !row.Values.TryGetValue(BulkImportConstants.IsActive, out var raw) ||
            string.IsNullOrWhiteSpace(raw) || bool.Parse(raw);
    }

    private static async Task<byte[]> HashFile(BulkImportPreviewRequestDTO request, CancellationToken cancellationToken)
    {
        using var stream = request.File!.OpenReadStream();
        return await SHA256.HashDataAsync(stream, cancellationToken);
    }
}

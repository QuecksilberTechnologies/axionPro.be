using System.Text.Json;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Models.Security;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using axionpro.domain.Entity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace axionpro.persistance.Repositories;

public sealed partial class BulkImportRepository
{
    #region Host preview

    public async Task<BulkImportPreviewResponseDTO> PreviewHostAsync(
        BulkImportMaster master, BulkImportTableDTO table, string? mappingJson,
        long tenantId, HostUserRequestContext host, CancellationToken ct)
    {
        if (master is not (BulkImportMaster.DeviceMaster or BulkImportMaster.TenantCard))
        {
            throw new ValidationErrorException("Unsupported Host import target.");
        }
        if (master == BulkImportMaster.TenantCard)
        {
            await EnsureCardTenantAsync(tenantId, ct);
        }
        var columns = master == BulkImportMaster.DeviceMaster
            ? HostBulkImportTableMapper.DeviceColumns : HostBulkImportTableMapper.CardColumns;
        var mapping = HostBulkImportTableMapper.ResolveColumns(table.Columns, columns, mappingJson);
        var result = new BulkImportPreviewResponseDTO
        {
            Master = master, SourceColumns = table.Columns, ColumnMapping = mapping
        };
        var required = master == BulkImportMaster.DeviceMaster
            ? new[] { "SNo", "DeviceCode", "DeviceName", "CompanyName", "ModelNo", "DeviceType" }
            : new[] { "CardNumber" };
        foreach (var name in required.Where(name => !mapping.ContainsKey(name)))
        {
            result.Errors.Add($"Map the required {name} column.");
        }
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var source in table.Rows)
        {
            var row = new BulkImportPreviewRowDTO { RowNumber = source.RowNumber, Status = BulkImportRowStatus.Ready };
            result.Rows.Add(row);
            try
            {
                foreach (var pair in mapping)
                {
                    var index = table.Columns.FindIndex(column => string.Equals(column, pair.Value, StringComparison.OrdinalIgnoreCase));
                    row.Values[pair.Key] = index < source.Values.Count ? source.Values[index].Trim() : string.Empty;
                }
                if (master == BulkImportMaster.DeviceMaster)
                {
                    var dto = HostBulkImportTableMapper.ReadRow<CreateDeviceMasterRequestDTO>(table, source, mapping);
                    CreateDeviceMasterCommandHandler.Validate(dto);
                    ValidateLengths<DeviceMaster>(dto);
                    var code = "code:" + dto.DeviceCode;
                    var model = "model:" + JsonSerializer.Serialize(new[] { dto.CompanyName.ToUpperInvariant(), dto.ModelNo.ToUpperInvariant() });
                    if (!seen.Add(code) || !seen.Add(model))
                    {
                        throw new ValidationErrorException("Duplicate device code or company/model in this upload.");
                    }
                    row.HostRecordId = await ExistingDeviceAsync(dto, ct);
                }
                else
                {
                    var dto = HostBulkImportTableMapper.ReadRow<CreateTenantCardMasterRequestDTO>(table, source, mapping);
                    TenantCardMasterHandlerBase.ValidateImport(dto, true);
                    ValidateLengths<TenantCardMaster>(dto);
                    var number = dto.CardNumber;
                    if (!seen.Add(number))
                    {
                        throw new ValidationErrorException("Duplicate card in this upload.");
                    }
                    row.CardLookupHash = TenantCardMasterHandlerBase.ImportHash(number, host.TenantEncryptionKey);
                    row.CardCiphertext = (encryption ?? throw new InvalidOperationException("Card encryption service is unavailable."))
                        .Encrypt(number, host.TenantEncryptionKey);
                    row.HostRecordId = await ExistingCardAsync(tenantId, row.CardLookupHash, ct);
                }
                if (row.HostRecordId.HasValue)
                {
                    row.Status = BulkImportRowStatus.Existing;
                }
            }
            catch (ValidationErrorException error)
            {
                row.Status = BulkImportRowStatus.Invalid;
                row.Errors.Add(error.Message);
            }
            finally
            {
                // Even invalid card rows must never place raw card numbers in durable JSON or responses.
                if (master == BulkImportMaster.TenantCard && row.Values.TryGetValue("CardNumber", out var raw))
                {
                    row.Values["CardNumber"] = raw.Length <= 4 ? "****" : "****" + raw[^4..];
                }
            }
        }
        return result;
    }

    private void ValidateLengths<TEntity>(object dto)
    {
        var entity = context.Model.FindEntityType(typeof(TEntity))!;
        foreach (var property in dto.GetType().GetProperties())
        {
            var maximum = entity.FindProperty(property.Name)?.GetMaxLength();
            if (maximum.HasValue && property.GetValue(dto) is string text && text.Length > maximum.Value)
            {
                throw new ValidationErrorException($"{property.Name} exceeds {maximum.Value} characters.");
            }
        }
    }

    private Task<long?> ExistingDeviceAsync(CreateDeviceMasterRequestDTO dto, CancellationToken ct)
    {
        return context.DeviceMasters.AsNoTracking().Where(item => !item.IsSoftDeleted &&
            (item.DeviceCode.ToLower() == dto.DeviceCode.ToLower() ||
             (item.CompanyName.ToLower() == dto.CompanyName.ToLower() && item.ModelNo.ToLower() == dto.ModelNo.ToLower())))
            .OrderBy(item => item.Id).Select(item => (long?)item.Id).FirstOrDefaultAsync(ct);
    }

    private Task<long?> ExistingCardAsync(long tenantId, string hash, CancellationToken ct)
    {
        return context.TenantCardMasters.AsNoTracking().Where(item => item.TenantId == tenantId &&
            item.CardNumberLookupHash == hash && !item.IsSoftDeleted)
            .Select(item => (long?)item.Id).FirstOrDefaultAsync(ct);
    }

    private async Task EnsureCardTenantAsync(long tenantId, CancellationToken ct)
    {
        if (!await context.Set<Tenant>().AsNoTracking().AnyAsync(item => item.Id == tenantId &&
            item.IsActive && item.IsSoftDeleted != true, ct))
        {
            throw new ValidationErrorException("Selected tenant is inactive or unavailable.");
        }
    }

    #endregion

    #region Host durable worker

    private async Task ProcessHostBatchAsync(BulkImportJob job, BulkImportPreviewResponseDTO preview, CancellationToken ct)
    {
        var transaction = context.Database.CurrentTransaction!;
        var end = Math.Min(preview.Rows.Count, job.NextRow + Math.Clamp(options.Value.BatchSize, 1, 200));
        for (var index = job.NextRow; index < end; index++)
        {
            var row = preview.Rows[index];
            if (row.Status == BulkImportRowStatus.Created)
            {
                job.NextRow = index + 1;
                continue;
            }
            await transaction.CreateSavepointAsync("host_bulk_row", ct);
            try
            {
                var table = new BulkImportTableDTO { Columns = row.Values.Keys.ToList() };
                var source = new BulkImportSourceRowDTO { RowNumber = row.RowNumber, Values = row.Values.Values.ToList() };
                var mapping = table.Columns.Where(name => job.Master != (int)BulkImportMaster.TenantCard || name != "CardNumber")
                    .ToDictionary(name => name, name => name);
                if (job.Master == (int)BulkImportMaster.DeviceMaster)
                {
                    var dto = HostBulkImportTableMapper.ReadRow<CreateDeviceMasterRequestDTO>(table, source, mapping);
                    CreateDeviceMasterCommandHandler.Validate(dto);
                    ValidateLengths<DeviceMaster>(dto);
                    row.HostRecordId = await ExistingDeviceAsync(dto, ct);
                    if (!row.HostRecordId.HasValue)
                    {
                        var entity = mapper.Map<DeviceMaster>(dto);
                        entity.IsOccupied = false;
                        entity.IsSoftDeleted = false;
                        entity.AddedById = job.ActorId;
                        entity.AddedDateTime = DateTime.UtcNow;
                        context.DeviceMasters.Add(entity);
                        await context.SaveChangesAsync(ct);
                        row.HostRecordId = entity.Id;
                        row.Status = BulkImportRowStatus.Created;
                    }
                    else
                    {
                        row.Status = BulkImportRowStatus.Existing;
                    }
                }
                else
                {
                    await EnsureCardTenantAsync(job.TenantId, ct);
                    var dto = HostBulkImportTableMapper.ReadRow<CreateTenantCardMasterRequestDTO>(table, source, mapping);
                    TenantCardMasterHandlerBase.ValidateImport(dto, false);
                    ValidateLengths<TenantCardMaster>(dto);
                    if (string.IsNullOrWhiteSpace(row.CardCiphertext) || string.IsNullOrWhiteSpace(row.CardLookupHash))
                    {
                        throw new ValidationErrorException("Protected card snapshot is missing. Create a new preview.");
                    }
                    row.HostRecordId = await ExistingCardAsync(job.TenantId, row.CardLookupHash, ct);
                    if (!row.HostRecordId.HasValue)
                    {
                        var entity = new TenantCardMaster
                        {
                            TenantId = job.TenantId, CardNumberEncrypted = row.CardCiphertext,
                            CardNumberLookupHash = row.CardLookupHash, CardStatus = (short)TenantCardStatus.Available,
                            AddedById = job.ActorId, AddedDateTime = DateTime.UtcNow
                        };
                        TenantCardMasterHandlerBase.ApplyImport(entity, dto);
                        context.TenantCardMasters.Add(entity);
                        await context.SaveChangesAsync(ct);
                        row.HostRecordId = entity.Id;
                        row.Status = BulkImportRowStatus.Created;
                    }
                    else
                    {
                        row.Status = BulkImportRowStatus.Existing;
                    }
                }
                row.Errors.Clear();
                await transaction.ReleaseSavepointAsync("host_bulk_row", ct);
            }
            catch (Exception error) when (error is ValidationErrorException ||
                error is DbUpdateException { InnerException: PostgresException })
            {
                await transaction.RollbackToSavepointAsync("host_bulk_row", ct);
                context.ChangeTracker.Clear();
                row.Status = BulkImportRowStatus.Failed;
                row.Errors = new List<string> { error is ValidationErrorException
                    ? error.Message : "Data changed or a database constraint rejected this row. Review and retry." };
                await transaction.ReleaseSavepointAsync("host_bulk_row", ct);
            }
            row.Processed = true;
            job.NextRow = index + 1;
        }
        job.Status = job.NextRow < preview.Rows.Count ? (int)BulkImportJobStatus.Running
            : (int)(preview.Rows.Any(row => row.Status == BulkImportRowStatus.Failed)
                ? BulkImportJobStatus.CompletedWithErrors : BulkImportJobStatus.Completed);
        await SaveProgress(job, preview, ct);
    }

    #endregion
}

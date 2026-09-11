using axionpro.application.Common.Enums;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories;

public sealed partial class BulkImportRepository
{
    #region Invitation Claims And Results

    public async Task<EmployeeImportInvitationDTO?> ClaimEmployeeInvitationAsync(
        BulkImportJobRequestDTO request, CommonDecodedResult actor, IReadOnlyCollection<int> attemptedRows,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var job = await LockOwnedEmployeeJobAsync(request.JobId, actor, cancellationToken);
        if (job.Status is not ((int)BulkImportJobStatus.Completed) and not ((int)BulkImportJobStatus.CompletedWithErrors))
        {
            throw new ConflictException("Review the completed Employee import before sending invitations.");
        }
        await EnsureWorkerPermissionAsync(job, cancellationToken);
        var preview = Deserialize(job);
        EmployeeImportInvitationDTO? invitation = null;
        foreach (var row in preview.Rows.Where(row => row.Status == BulkImportRowStatus.Created &&
                     !attemptedRows.Contains(row.RowNumber) &&
                     (request.RowNumbers is null || request.RowNumbers.Contains(row.RowNumber)) &&
                     row.InvitationStatus is BulkImportInvitationStatus.Pending or BulkImportInvitationStatus.Failed))
        {
            var login = await context.LoginCredentials.AsNoTracking()
                .Where(item => item.TenantId == actor.TenantId && item.EmployeeId == row.ImportedEmployeeId &&
                    item.IsActive && item.IsSoftDeleted != true && item.Employee.IsActive && !item.Employee.IsSoftDeleted &&
                    item.HasFirstLogin && (item.Password == null || item.Password == ""))
                .Select(item => new { item.EmployeeId, item.LoginId, item.Employee.FirstName, item.Employee.LastName })
                .SingleOrDefaultAsync(cancellationToken);
            if (login is null)
            {
                row.InvitationStatus = BulkImportInvitationStatus.NotRequired;
                row.InvitationError = "Account is inactive, unavailable or already has a password.";
                continue;
            }
            row.InvitationStatus = BulkImportInvitationStatus.Sending;
            row.InvitationAttemptId = Guid.NewGuid();
            row.InvitationAttemptedAtUtc = DateTime.UtcNow;
            row.InvitationError = null;
            invitation = new EmployeeImportInvitationDTO(row.RowNumber, row.InvitationAttemptId.Value,
                login.EmployeeId, login.LoginId, $"{login.FirstName} {login.LastName}".Trim());
            break;
        }
        await SaveProgress(job, preview, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return invitation;
    }

    public async Task CompleteEmployeeInvitationAsync(BulkImportJobRequestDTO request, CommonDecodedResult actor,
        EmployeeImportInvitationDTO invitation, BulkImportInvitationStatus status, CancellationToken cancellationToken)
    {
        if (status is not (BulkImportInvitationStatus.Sent or BulkImportInvitationStatus.Failed or BulkImportInvitationStatus.DeliveryUnknown))
        {
            throw new ValidationErrorException("Invalid invitation delivery result.");
        }
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var job = await LockOwnedEmployeeJobAsync(request.JobId, actor, cancellationToken);
        var preview = Deserialize(job);
        var row = preview.Rows.SingleOrDefault(item => item.RowNumber == invitation.RowNumber);
        if (row?.InvitationAttemptId != invitation.AttemptId || row.InvitationStatus != BulkImportInvitationStatus.Sending)
        {
            throw new ConflictException("Invitation attempt is no longer current.");
        }
        row.InvitationStatus = status;
        row.InvitationError = status switch
        {
            BulkImportInvitationStatus.Failed => "Email service reported failure. Retry invitations without importing again.",
            BulkImportInvitationStatus.DeliveryUnknown => "Delivery could not be confirmed. Check the email log before any further send.",
            _ => null
        };
        await SaveProgress(job, preview, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<BulkImportJob> LockOwnedEmployeeJobAsync(Guid id, CommonDecodedResult actor, CancellationToken token)
    {
        var jobs = await context.Set<BulkImportJob>().FromSqlInterpolated($"""
            SELECT * FROM axionpro."BulkImportJob" WHERE "Id" = {id}
            AND "TenantId" = {actor.TenantId} AND "ActorId" = {actor.LoggedInEmployeeId}
            AND "Master" = {(int)BulkImportMaster.Employee} FOR UPDATE
            """).AsNoTracking().ToListAsync(token);
        return jobs.SingleOrDefault() ?? throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
    }

    #endregion
}

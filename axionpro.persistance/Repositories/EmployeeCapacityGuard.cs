using axionpro.application.Exceptions;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories;

/// <summary>Shared subscription capacity calculation for imports and ordinary Employee creation.</summary>
internal static class EmployeeCapacityGuard
{
    public static async Task<int> AvailableSeatsAsync(WorkforceDbContext context, long tenantId, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        var limits = await context.TenantSubscriptions.AsNoTracking().Where(item => item.TenantId == tenantId && item.IsActive &&
                item.SubscriptionStartDate <= now && item.SubscriptionEndDate > now && item.SubscriptionPlan.IsActive && !item.SubscriptionPlan.IsSoftDeleted)
            .Select(item => item.SubscriptionPlan.MaxUsers).ToListAsync(token);
        if (limits.Count != 1 || limits[0] <= 0)
        {
            return -1;
        }
        // Suspended employees retain their seat; only soft-deleted employees are excluded.
        var used = await context.Employees.CountAsync(item => item.TenantId == tenantId && !item.IsSoftDeleted, token);
        return limits[0] - used;
    }

    public static async Task EnsureSpaceAsync(WorkforceDbContext context, long tenantId, CancellationToken token)
    {
        if (context.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException("Employee insertion requires its account-creation transaction.");
        }
        // Keep subscription/plan edits from changing capacity between the final check and insert.
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT \"Id\" FROM axionpro.\"TenantSubscription\" WHERE \"TenantId\" = {tenantId} FOR SHARE", token);
        await context.Database.ExecuteSqlInterpolatedAsync($"""
            SELECT p."Id" FROM axionpro."SubscriptionPlan" p
            JOIN axionpro."TenantSubscription" s ON s."SubscriptionPlanId" = p."Id"
            WHERE s."TenantId" = {tenantId} FOR SHARE OF p
            """, token);
        if (await AvailableSeatsAsync(context, tenantId, token) <= 0)
        {
            throw new ValidationErrorException("Subscription MaxUsers capacity is unavailable or exhausted, including the initial Tenant Admin seat.");
        }
    }
}

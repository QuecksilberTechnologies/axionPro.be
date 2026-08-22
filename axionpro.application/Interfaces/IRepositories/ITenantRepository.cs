// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines Tenant persistence operations, including Host-managed lifecycle transitions.
// ================================================================

using axionpro.application.DTOs.Registration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByIdAsync(long? id, bool isActive);
        Task<bool> CheckTenantByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Tenant?> GetByCodeAsync(string tenantCode, CancellationToken cancellationToken = default);
        Task<List<Tenant>> GetAllTenantBySubscriptionIdAsync(Tenant tenant, CancellationToken cancellationToken = default);

        Task AddTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
        Task AddTenantProfileAsync(TenantProfile tenantProfile, CancellationToken cancellationToken = default);

        Task<Tenant?> UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
        Task DeleteTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);

        #region Host Management

        /// <summary>
        /// Retrieves a tracked, non-soft-deleted Tenant for a Host-managed operation.
        /// </summary>
        /// <param name="tenantId">The authoritative Tenant identifier.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The Tenant entity, or <see langword="null"/> when unavailable.</returns>
        Task<Tenant?> GetHostManagedTenantByIdAsync(long tenantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether another non-soft-deleted Tenant uses the supplied email address.
        /// </summary>
        /// <param name="tenantEmail">The email address to check.</param>
        /// <param name="excludedTenantId">The current Tenant excluded from the check.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns><see langword="true"/> when another Tenant uses the email.</returns>
        Task<bool> IsTenantEmailInUseAsync(string tenantEmail, long excludedTenantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether another non-soft-deleted Tenant uses the supplied Tenant code.
        /// </summary>
        /// <param name="tenantCode">The Tenant code to check.</param>
        /// <param name="excludedTenantId">The current Tenant excluded from the check.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns><see langword="true"/> when another Tenant uses the code.</returns>
        Task<bool> IsTenantCodeInUseAsync(string tenantCode, long excludedTenantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the legitimate onboarding credential and Employee required by the existing Tenant welcome-email flow.
        /// </summary>
        /// <param name="tenantId">The Tenant identifier.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The onboarding credential with its Employee, or <see langword="null"/> when unavailable.</returns>
        Task<LoginCredential?> GetTenantOnboardingCredentialAsync(long tenantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stages a Host-managed Tenant update prepared by the handler.
        /// </summary>
        /// <param name="tenant">The tracked Tenant entity with validated editable changes.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A task representing the staging operation.</returns>
        Task StageHostManagedUpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stages a Tenant status transition and synchronizes valid, non-soft-deleted credentials.
        /// </summary>
        /// <param name="tenant">The tracked Tenant whose active state was prepared by the handler.</param>
        /// <param name="hostUserId">The validated Host user used for credential audit fields.</param>
        /// <param name="utcNow">The single UTC audit timestamp captured for the Host request.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A task representing the staging operation.</returns>
        Task SynchronizeTenantStatusAsync(
            Tenant tenant,
            long hostUserId,
            DateTime utcNow,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Stages Tenant soft deletion and deactivates all related login credentials without deleting historical records.
        /// </summary>
        /// <param name="tenant">The tracked Tenant prepared for soft deletion.</param>
        /// <param name="hostUserId">The validated Host user used for credential audit fields.</param>
        /// <param name="utcNow">The single UTC audit timestamp captured for the Host request.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>A task representing the staging operation.</returns>
        Task SoftDeleteTenantAndDeactivateCredentialsAsync(
            Tenant tenant,
            long hostUserId,
            DateTime utcNow,
            CancellationToken cancellationToken = default);

        #endregion
    }

 }

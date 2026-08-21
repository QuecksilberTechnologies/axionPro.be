using axionpro.application.DTOs.UserLogin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines Tenant LoginCredential persistence operations.
// ============================================================================

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IUserLoginReopsitory
    {
        #region Authentication Queries

        Task<LoginCredential> AuthenticateUser(string loginId);

        /// <summary>
        /// Retrieves an active, non-soft-deleted Tenant login credential by its immutable owner identifier.
        /// </summary>
        /// <param name="loginCredentialId">The Tenant LoginCredential primary key stored on the refresh token.</param>
        /// <returns>The active Tenant login credential, or <see langword="null"/> when it is no longer valid.</returns>
        Task<LoginCredential?> GetActiveByIdAsync(long loginCredentialId);

        /// <summary>
        /// Retrieves the minimal active Tenant Employee bootstrap projection for an authenticated login credential.
        /// </summary>
        /// <param name="loginCredentialId">The active Tenant login credential identifier.</param>
        /// <param name="cancellationToken">A token used to cancel the database query.</param>
        /// <returns>The validated bootstrap projection, or <see langword="null"/> when its employee or tenant is no longer valid.</returns>
        Task<NewLoginBootstrapReadModel?> GetNewLoginBootstrapAsync(
            long loginCredentialId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Login Metadata

        /// <summary>
        /// Persists the permitted login-device metadata for an active Tenant login credential.
        /// </summary>
        /// <param name="loginCredential">The credential containing only the login metadata values to persist.</param>
        /// <param name="cancellationToken">A token used to cancel the persistence operation.</param>
        /// <returns><see langword="true"/> when the active credential metadata was updated; otherwise, <see langword="false"/>.</returns>
        Task<bool> UpdateLoginMetadataAsync(
            LoginCredential loginCredential,
            CancellationToken cancellationToken = default);

        #endregion

        Task<long> CreateUser(LoginCredential loginRequest);
        Task<bool> UpdatePassword(long empId, string password, long UpdatedById);
        Task<bool> SetNewPassword(LoginCredential setRequest);
        Task<LoginCredential> GetEmployeeIdByUserLogin(string userLoing);
       
    }

}

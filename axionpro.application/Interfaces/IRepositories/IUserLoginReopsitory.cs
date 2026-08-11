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
        #region Login Credential Queries

        Task<LoginCredential> AuthenticateUser(string loginId);

        /// <summary>
        /// Retrieves an active, non-soft-deleted Tenant login credential by its immutable owner identifier.
        /// </summary>
        /// <param name="loginCredentialId">The Tenant LoginCredential primary key stored on the refresh token.</param>
        /// <returns>The active Tenant login credential, or <see langword="null"/> when it is no longer valid.</returns>
        Task<LoginCredential?> GetActiveByIdAsync(long loginCredentialId);

        #endregion

        Task<long> CreateUser(LoginCredential loginRequest);
        Task<bool> UpdatePassword(long empId, string password, long UpdatedById);
        Task<bool> SetNewPassword(LoginCredential setRequest);
        Task<LoginCredential> GetEmployeeIdByUserLogin(string userLoing);
       
    }

}

using axionpro.application.DTOs.UserLogin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IUserLoginReopsitory
    {
        Task<LoginCredential> AuthenticateUser(string loginId);
        Task<long> CreateUser(LoginCredential loginRequest);
        Task<bool> UpdatePassword(long empId, string password, long UpdatedById);
        Task<bool> SetNewPassword(LoginCredential setRequest);
        Task<LoginCredential> GetEmployeeIdByUserLogin(string userLoing);
       
    }

}

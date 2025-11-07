using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class UserLoginReopsitory : IUserLoginReopsitory
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<UserLoginReopsitory> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;

        public UserLoginReopsitory(WorkforceDbContext context, ILogger<UserLoginReopsitory> logger, IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory, IPasswordService passwordService , IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _configuration = configuration;
        }

        public async Task<LoginCredential?> AuthenticateUser(LoginRequestDTO loginRequest)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation("🔐 Authenticating user with LoginId: {LoginId}", loginRequest.LoginId);

                var user = await context.LoginCredentials
                    .FirstOrDefaultAsync(u => u.LoginId == loginRequest.LoginId);

                if (user == null)
                {
                    _logger.LogWarning("❌ Login failed: No user found for LoginId: {LoginId}", loginRequest.LoginId);
                    return null;
                }

                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    _logger.LogWarning("⚠️ Login failed: User {LoginId} has no password stored", loginRequest.LoginId);
                    return null;
                }

                var passwordMatch =  _passwordService.VerifyPassword( user.Password, loginRequest.Password);

                if (!passwordMatch)
                {
                    _logger.LogWarning("🚫 Login failed: Incorrect password for LoginId: {LoginId}", loginRequest.LoginId);
                    return null;
                }

                _logger.LogInformation("✅ User authenticated successfully for LoginId: {LoginId}", loginRequest.LoginId);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception occurred while authenticating LoginId: {LoginId}", loginRequest.LoginId);
                throw;
            }
        }

        public async Task<long> CreateUser(LoginCredential loginRequest)
        {
            try
            {
                if (_context == null)
                {
                    _logger?.LogError("DbContext is null in CreateUser.");
                    throw new ArgumentNullException(nameof(_context), "DbContext is not initialized.");
                }

               

                await _context.LoginCredentials.AddAsync(loginRequest); // Add LoginCredential
                await _context.SaveChangesAsync(); // Save changes

                _logger?.LogInformation("User created successfully with ID: {UserId}", loginRequest.Id);

                return loginRequest.Id; // Return auto-generated ID
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while creating user.");
                throw;
            }
        }

        public async Task<LoginCredential> GetEmployeeIdByUserLogin(string userLogin)
        {
            var login = await _context.LoginCredentials.FirstOrDefaultAsync(x => x.LoginId == userLogin && x.IsActive == true);

            if (login == null)
                return null;


            return login;

        }
        public async Task<bool> UpdateNewPassword(LoginCredential setRequest)
        {
            try
            {

                var user = await _context.LoginCredentials
                    .FirstOrDefaultAsync(x =>
                        x.LoginId == setRequest.LoginId &&
                        x.IsActive == true &&
                        x.HasFirstLogin == true &&// ✅ Only allow update if it's first login
                        x.IsPasswordChangeRequired == true);

                if (user == null)
                {
                    return false; // User not found or first login already done
                }

                user.Password = setRequest.Password;
                user.HasFirstLogin = false; 
                user.IsPasswordChangeRequired = false; 
                // Mark as password updated
                                            // user.UpdatedById = setRequest.UpdatedById;
                                            // user.UpdatedDateTime = DateTime.UtcNow;

                _context.LoginCredentials.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating password for LoginId: {LoginId}", setRequest.LoginId);
                return false;
            }
        }


        public async Task<bool> SetNewPassword(LoginCredential setRequest)
        {
            try
            {

                var user = await _context.LoginCredentials
                    .FirstOrDefaultAsync(x =>
                        x.LoginId == setRequest.LoginId &&
                        x.IsActive == true); // ✅ Only allow update if it's first login

                if (user == null)
                {
                    return false; // User not found or first login already done
                }

                user.Password = setRequest.Password;
                
                                            // user.UpdatedById = setRequest.UpdatedById;
                                            // user.UpdatedDateTime = DateTime.UtcNow;

                _context.LoginCredentials.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating password for LoginId: {LoginId}", setRequest.LoginId);
                return false;
            }
        }



        private bool VerifyPassword(string providedPassword, string storedPassword)
        {
            // Secure hashing and comparison logic should be implemented here
            return providedPassword == storedPassword;
        }
    }

}

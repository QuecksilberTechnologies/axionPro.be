using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    class ForgotPasswordOtpRepository : IForgotPasswordOtpRepository
    {
        private readonly WorkforceDbContext _context;         
        private ILogger<ForgotPasswordOtpRepository> _logger;
        public ForgotPasswordOtpRepository(WorkforceDbContext context, ILogger<ForgotPasswordOtpRepository> logger)
        {
            this._context = context;
            this._logger = logger;
        }
        
             public async Task<ForgotPasswordOTPDetail?> GetOtpValidateTrueAndUsedFalseByEmployeeIdAsync(long userId, long? tenantId)
              {
               try
                {
                var otpDetail = await _context.ForgotPasswordOTPDetails
                    .Where(x => x.UserId == userId
                                && x.TenantId == tenantId
                                && x.IsUsed==false
                                && x.IsValidate == true
                                && x.OtpexpireDateTime > DateTime.Now)
                    .OrderByDescending(x => x.CreatedDateTime)
                    .FirstOrDefaultAsync();

                if (otpDetail == null)
                {
                    _logger.LogInformation("No valid OTP found for UserId: {UserId}, TenantId: {TenantId}", userId, tenantId);
                }
                else
                {
                    _logger.LogInformation("Valid OTP retrieved for UserId: {UserId}, OTP ID: {OtpId}", userId, otpDetail.Id);
                }

                return otpDetail;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching OTP for UserId: {UserId}, TenantId: {TenantId}", userId, tenantId);
                return null;
            }
        }

        public async Task<ForgotPasswordOTPDetail?> GetValidOtpByEmployeeIdAsync(long userId, long? tenantId)
        {
            try
            {
                var otpDetail = await _context.ForgotPasswordOTPDetails
                    .Where(x => x.UserId == userId
                                && x.TenantId == tenantId
                                && !x.IsUsed
                                &&  x.IsValidate==false
                                && x.OtpexpireDateTime > DateTime.Now)
                    .OrderByDescending(x => x.CreatedDateTime)
                    .FirstOrDefaultAsync();

                if (otpDetail == null)
                {
                    _logger.LogInformation("No valid OTP found for UserId: {UserId}, TenantId: {TenantId}", userId, tenantId);
                }
                else
                {
                    _logger.LogInformation("Valid OTP retrieved for UserId: {UserId}, OTP ID: {OtpId}", userId, otpDetail.Id);
                }

                return otpDetail;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching OTP for UserId: {UserId}, TenantId: {TenantId}", userId, tenantId);
                return null;
            }
        }

        public async Task<bool> UpdateOTPAsync(ForgotPasswordOTPDetail otp)
        {
            try
            {
                 
                _context.ForgotPasswordOTPDetails.Update(otp);
                await _context.SaveChangesAsync();
                _logger.LogInformation("OTP updated successfully for EmployeeId {EmpId}", otp.EmployeeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating OTP for EmployeeId {EmpId}", otp.EmployeeId);
                return false;
            }
        }

        public async Task<long> AddAsync(ForgotPasswordOTPDetail otp)
        {
            try
            {
                await _context.ForgotPasswordOTPDetails.AddAsync(otp);
                await _context.SaveChangesAsync();

                // Return the inserted OTP record's Id (assuming Id is auto-incremented primary key)
                return otp.Id;
            }
            catch (Exception ex)
            {
                // Log the exception with context (adjust logger as per your project structure)
                _logger.LogError(ex, "Error occurred while adding ForgotPasswordOTPDetail for EmployeeId: {EmployeeId}", otp.EmployeeId);

                // Re-throw or handle gracefully
                throw;
            }
        }

        public async Task DeleteAsync(ForgotPasswordOTPDetail otp)
        {
            _context.ForgotPasswordOTPDetails.Remove(otp);
        }

        public async Task<ForgotPasswordOTPDetail?> GetByOtpAndEmployeeIdAsync(string otp, long employeeId)
        {
            return await _context.ForgotPasswordOTPDetails
                .FirstOrDefaultAsync(x => x.Otp == otp && x.EmployeeId == employeeId && !x.IsUsed);
        }

      
    }
}
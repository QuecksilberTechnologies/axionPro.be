

using AutoMapper;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{

    public class EmployeeIdentityRepository : IEmployeeIdentityRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeIdentityRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeIdentityRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeIdentityRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }


        //public async Task<GetIdentityResponseDTO> CreateAsync(EmployeePersonalDetail entity)
        //{
        //    try
        //    {
        //        // 1️⃣ Validation
        //        if (entity == null)
        //            throw new ArgumentNullException(nameof(entity), "Personal info entity cannot be null.");

        //        if (entity.EmployeeId <= 0)
        //            throw new ArgumentException("Invalid EmployeeId provided.");

        //        // 2️⃣ Insert Record
        //        await _context.EmployeePersonalDetails.AddAsync(entity);
        //        await _context.SaveChangesAsync();

        //        // 3️⃣ Fetch LATEST record → SINGLE OBJECT
        //        var response = await _context.EmployeePersonalDetails
        //            .AsNoTracking()
        //            .Where(x =>
        //                x.EmployeeId == entity.EmployeeId &&
        //                x.IsSoftDeleted != true &&
        //                x.IsActive == true)
        //            .OrderByDescending(x => x.Id)
        //            .Select(x => new GetIdentityResponseDTO
        //            {
        //                EmployeeId = x.EmployeeId.ToString(),

        //                AadhaarNumber = x.AadhaarNumber,
        //                PanNumber = x.PanNumber,
        //                PassportNumber = x.PassportNumber,
        //                DrivingLicenseNumber = x.DrivingLicenseNumber,
        //                VoterId = x.VoterId,

        //                BloodGroup = x.BloodGroup,
        //                MaritalStatus = x.MaritalStatus,
        //                Nationality = x.Nationality,

        //                HasEPFAccount = x.HasEPFAccount,
        //                UANNumber = x.UANNumber,

        //                EmergencyContactName = x.EmergencyContactName,
        //                EmergencyContactNumber = x.EmergencyContactNumber,
        //                EmergencyContactRelation = x.EmergencyContactRelation,

        //                // 📎 Upload flags
        //                hasAadharIdUploaded = !string.IsNullOrEmpty(x.AadhaarDocPath),
        //                hasPanIdUploaded = !string.IsNullOrEmpty(x.PanDocPath),
        //                hasPassportIdUploaded = !string.IsNullOrEmpty(x.PassportDocPath),

        //                // 📂 Paths
        //                aadharFilePath = x.AadhaarDocPath,
        //                panFilePath = x.PanDocPath,
        //                passportFilePath = x.PassportDocPath,

        //                IsInfoVerified = x.IsInfoVerified ?? false,
        //                IsEditAllowed = x.IsEditAllowed,

        //                CompletionPercentage =
        //                    Math.Round(
        //                        (new[]
        //                        {
        //                    string.IsNullOrEmpty(x.AadhaarNumber) ? 0 : 1,
        //                    string.IsNullOrEmpty(x.PanNumber) ? 0 : 1,
        //                    string.IsNullOrEmpty(x.DrivingLicenseNumber) ? 0 : 1,
        //                    string.IsNullOrEmpty(x.VoterId) ? 0 : 1,
        //                    string.IsNullOrEmpty(x.BloodGroup) ? 0 : 1,
        //                    string.IsNullOrEmpty(x.Nationality) ? 0 : 1,
        //                    string.IsNullOrEmpty(x.EmergencyContactName) ? 0 : 1,
        //                    string.IsNullOrEmpty(x.EmergencyContactRelation) ? 0 : 1,
        //                    !string.IsNullOrEmpty(x.AadhaarDocPath) ? 1 : 0,
        //                    !string.IsNullOrEmpty(x.PanDocPath) ? 1 : 0
        //                        }.Sum() / 10.0) * 100, 0)
        //            })
        //            .FirstOrDefaultAsync();   // 🔥 THIS IS THE KEY

        //        return response!;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,
        //            "❌ Error occurred while adding/fetching personal info for EmployeeId: {EmployeeId}",
        //            entity.EmployeeId);

        //        throw;
        //    }
        //}

        public async Task<bool> UpdateIdentity(EmployeePersonalDetail employeePersonal)
        {
            try
            {
                
                _context.EmployeePersonalDetails.Update(employeePersonal);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error while updating employee identity info  (Id: {employeePersonal?.Id})");
                return false;
            }
        }

        //public async Task<GetIdentityResponseDTO?> GetInfo(GetIdentityRequestDTO dto)
        //{
        //    try
        //    {
        //        // 🧭 Base Query
        //        var query = _context.EmployeePersonalDetails
        //            .AsNoTracking()
        //            .Where(x =>
        //                x.EmployeeId == dto.Prop.EmployeeId &&
        //                x.IsActive == dto.IsActive &&
        //                x.IsSoftDeleted != true);
             

        //        // 🔍 Keyword Search
        //        if (!string.IsNullOrWhiteSpace(dto.SortBy))
        //        {
        //            var keyword = dto.SortBy.Trim().ToLower();
        //            query = query.Where(x =>
        //                (x.BloodGroup ?? "").ToLower().Contains(keyword) ||                        
        //                (x.Nationality ?? "").ToLower().Contains(keyword) ||
        //                (x.EmergencyContactName ?? "").ToLower().Contains(keyword) ||
        //                (x.PanNumber ?? "").ToLower().Contains(keyword) ||
        //                (x.AadhaarNumber ?? "").ToLower().Contains(keyword) ||
        //                (x.PassportNumber ?? "").ToLower().Contains(keyword));
        //        }

        //        // 🔽 Sorting (latest record preferred)
        //        query = query.OrderByDescending(x => x.Id);

        //        // 🎯 SINGLE RECORD
        //        var identity = await query
        //            .Select(identity => new GetIdentityResponseDTO
        //            {
        //                EmployeeId = identity.EmployeeId.ToString(),
        //                BloodGroup = identity.BloodGroup,
        //                MaritalStatus = identity.MaritalStatus,
        //                Nationality = identity.Nationality,
        //                EmergencyContactName = identity.EmergencyContactName,
        //                EmergencyContactNumber = identity.EmergencyContactNumber,
        //                EmergencyContactRelation = identity.EmergencyContactRelation,

        //                PanNumber = identity.PanNumber,
        //                AadhaarNumber = identity.AadhaarNumber,
        //                PassportNumber = identity.PassportNumber,
        //                DrivingLicenseNumber = identity.DrivingLicenseNumber,

        //                HasEPFAccount = identity.HasEPFAccount,
        //                UANNumber = identity.UANNumber,

        //                hasAadharIdUploaded = identity.HasAadhaarIdUploaded,
        //                hasPanIdUploaded = identity.HasPanIdUploaded,
        //                hasPassportIdUploaded = identity.HasPassportIdUploaded,

        //                aadharFilePath = identity.AadhaarDocPath,
        //                panFilePath = identity.PanDocPath,
        //                passportFilePath = identity.PassportDocPath,

        //                CompletionPercentage =
        //                    Math.Round(
        //                        (new[]
        //                        {
        //                    string.IsNullOrEmpty(identity.AadhaarNumber) ? 0 : 1,
        //                    string.IsNullOrEmpty(identity.PanNumber) ? 0 : 1,
        //                    string.IsNullOrEmpty(identity.DrivingLicenseNumber) ? 0 : 1,
        //                    string.IsNullOrEmpty(identity.VoterId) ? 0 : 1,
        //                    string.IsNullOrEmpty(identity.BloodGroup) ? 0 : 1,
        //                    string.IsNullOrEmpty(identity.Nationality) ? 0 : 1,
        //                    string.IsNullOrEmpty(identity.EmergencyContactName) ? 0 : 1,
        //                    string.IsNullOrEmpty(identity.EmergencyContactRelation) ? 0 : 1,
        //                    identity.HasAadhaarIdUploaded ? 1 : 0,
        //                    identity.HasPanIdUploaded ? 1 : 0
        //                        }.Sum() / 10.0) * 100, 0)
        //            })
        //            .FirstOrDefaultAsync();

        //        return identity; // ✅ single object or null
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,
        //            "❌ Error while fetching identity info | EmployeeId={EmployeeId}",
        //            dto.Prop.EmployeeId);

        //        throw;
        //    }
        //}

        public async Task<EmployeePersonalDetail?> GetSingleRecordAsync(long id, bool isActive)
        {
            try
            {
                IQueryable<EmployeePersonalDetail> query =
                    _context.EmployeePersonalDetails
                        .AsNoTracking()
                        .Where(x =>
                            x.Id == id &&
                            (x.IsSoftDeleted != true));

                // Apply IsActive ONLY when true is explicitly required
                if (isActive)
                {
                    query = query.Where(x => x.IsActive == true);
                }

                // SINGLE RECORD
                return await query
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error while fetching EmployeePersonalDetail | Id={Id}, IsActive={IsActive}",
                    id, isActive);

                throw;
            }
        }


        public async Task<bool> IsEmployeePersonalDetailExistsAsync(long id, bool? isActive)
        {
            try
            {
                IQueryable<EmployeePersonalDetail> query =
                    _context.EmployeePersonalDetails
                        .AsNoTracking()
                        .Where(x =>
                            x.EmployeeId == id &&
                            (x.IsSoftDeleted !=true));

                // Apply IsActive ONLY if client sends it
                if (isActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == isActive.Value);
                }

                // SINGLE SQL HIT
                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error in IsEmployeePersonalDetailExistsAsync | Id={Id}, IsActive={IsActive}",
                    id, isActive);

                throw;
            }
        }

        public Task<GetEmployeeIdentityResponseDTO> GetInfo(GetIdentityRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<GetIdentityResponseDTO> CreateAsync(EmployeePersonalDetail entity)
        {
            throw new NotImplementedException();
        }
    }



}












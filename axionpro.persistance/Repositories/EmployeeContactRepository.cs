using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.PercentageHelper;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Drawing.Printing;

namespace axionpro.persistance.Repositories
{
    public class EmployeeContactRepository : IEmployeeContactRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeContactRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public EmployeeContactRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeContactRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        public async Task<PagedResponseDTO<GetContactResponseDTO>> CreateAsync(EmployeeContact entity)
        {
            try
            {
                // ✅ 1️⃣ Validation
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                if (entity.EmployeeId <= 0)
                    throw new ArgumentException("Invalid EmployeeId");

                // 🔐 2️⃣ SINGLE PRIMARY RULE (VERY IMPORTANT)
                if (entity.IsPrimary == true)
                {
                    var existingPrimaryContacts = await _context.EmployeeContacts
                        .Where(x =>
                            x.EmployeeId == entity.EmployeeId &&
                            x.IsPrimary == true &&
                            x.IsSoftDeleted != true)
                        .ToListAsync();

                    if (existingPrimaryContacts.Any())
                    {
                        foreach (var contact in existingPrimaryContacts)
                        {
                            contact.IsPrimary = false;
                        }

                        _context.EmployeeContacts.UpdateRange(existingPrimaryContacts);
                        await _context.SaveChangesAsync();
                    }
                }

                // ✅ 3️⃣ Insert new contact
                await _context.EmployeeContacts.AddAsync(entity);
                await _context.SaveChangesAsync();

                // ✅ 4️⃣ Fetch list with LEFT JOINs
                // ✅ 4️⃣ Fetch list with LEFT JOINs
                var query =
                    from c in _context.EmployeeContacts.AsNoTracking()

                    join country in _context.Countries
                        on (c.CountryId > 0 ? c.CountryId : null)
                        equals (int?)country.Id into countryJoin
                    from country in countryJoin.DefaultIfEmpty()

                    join state in _context.States
                        on (c.StateId > 0 ? c.StateId : null)
                        equals (int?)state.Id into stateJoin
                    from state in stateJoin.DefaultIfEmpty()

                    join district in _context.Districts
                        on (c.DistrictId > 0 ? c.DistrictId : null)
                        equals (int?)district.Id into districtJoin
                    from district in districtJoin.DefaultIfEmpty()

                    where c.EmployeeId == entity.EmployeeId
                          && c.IsSoftDeleted != true
                    // ❌ IsActive filter hata diya

                    orderby c.Id descending

                    select new GetContactResponseDTO
                    {
                        Id = c.Id,
                        EmployeeId = c.EmployeeId.ToString(),

                        ContactName = c.ContactName,
                        ContactNumber = c.ContactNumber,
                        AlternateNumber = c.AlternateNumber,
                        Email = c.Email,
                        IsPrimary = c.IsPrimary,
                        ContactType = c.ContactType,
                        Relation = c.Relation,

                        CountryId = c.CountryId,
                        CountryName = country != null ? country.CountryName : string.Empty,

                        StateId = c.StateId,
                        StateName = state != null ? state.StateName : string.Empty,

                        DistrictId = c.DistrictId,
                        DistrictName = district != null ? district.DistrictName : string.Empty,

                        HouseNo = c.HouseNo,
                        LandMark = c.LandMark,
                        Street = c.Street,
                        Address = c.Address,

                        Remark = c.Remark,
                        Description = c.Description,

                        // ✅ Completion %
                        CompletionPercentage = CompletionCalculatorHelper.ContactPropCalculate(
                                 new GetContactResponseDTO
                                         {
                                     Id = c.Id,
             ContactType = c.ContactType,
             Relation = c.Relation,
             ContactName = c.ContactName,
             ContactNumber = c.ContactNumber,
             AlternateNumber = c.AlternateNumber,
             Email = c.Email,
             IsPrimary = c.IsPrimary,
             HouseNo = c.HouseNo,
             CountryId = c.CountryId,
             StateId = c.StateId,
             DistrictId = c.DistrictId,
             Address = c.Address
         })
                    };


                // ✅ Pagination (same filter)
                var totalRecords = await _context.EmployeeContacts
                    .AsNoTracking()
                    .Where(x =>
                        x.EmployeeId == entity.EmployeeId &&
                        x.IsSoftDeleted != true)
                    .CountAsync();

                var records = await query.Take(10).ToListAsync();


                return new PagedResponseDTO<GetContactResponseDTO>
                {
                    Items = records,
                    TotalCount = totalRecords,
                    PageNumber = 1,
                    PageSize = 10
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while adding/fetching Contact info for EmployeeId: {EmployeeId}",
                    entity.EmployeeId);

                throw;
            }
        }
        public async Task<PagedResponseDTO<GetContactResponseDTO>> GetInfo(GetContactRequestDTO dto)
        {
            try
            {
                // 🔒 SAFETY
                if (dto?.Prop == null || dto.Prop.EmployeeId <= 0)
                    throw new Exception("Invalid EmployeeId.");
 

                // 🔹 QUERY (NULL SAFE LEFT JOINS)
                var records = await
                (
                    from c in _context.EmployeeContacts.AsNoTracking()

                        // ✅ COUNTRY LEFT JOIN (NULL SAFE)
                    join country in _context.Countries
                        on c.CountryId equals (int?)country.Id into countryJoin
                    from country in countryJoin.DefaultIfEmpty()

                        // ✅ STATE LEFT JOIN
                    join state in _context.States
                        on c.StateId equals (int?)state.Id into stateJoin
                    from state in stateJoin.DefaultIfEmpty()

                        // ✅ DISTRICT LEFT JOIN (0 / NULL SAFE)
                    join district in _context.Districts
                        on c.DistrictId equals (int?)district.Id into districtJoin
                    from district in districtJoin.DefaultIfEmpty()

                    where
                        c.EmployeeId == dto.Prop.EmployeeId &&
                        (c.IsSoftDeleted == false || c.IsSoftDeleted == null)

                    orderby c.Id descending   // ✅ LATEST FIRST

                    select new GetContactResponseDTO
                    {
                        Id = c.Id,                // ✅ STRING
                        EmployeeId = c.EmployeeId.ToString(),

                        ContactName = c.ContactName ?? string.Empty,
                        ContactNumber = c.ContactNumber ?? string.Empty,
                        AlternateNumber = c.AlternateNumber,
                        Email = c.Email,
                        Relation = c.Relation,
                        ContactType = c.ContactType,

                        Address = c.Address,
                        HouseNo = c.HouseNo,
                        Street = c.Street,
                        LandMark = c.LandMark,

                        CountryId = c.CountryId,
                        CountryName = country != null ? country.CountryName : string.Empty,

                        StateId = c.StateId,
                        StateName = state != null ? state.StateName : string.Empty,

                        DistrictId = c.DistrictId,
                        DistrictName = district != null ? district.DistrictName : string.Empty,

                        IsPrimary = c.IsPrimary,
                        IsActive = c.IsActive,
                        IsEditAllowed = c.IsEditAllowed,
                        IsInfoVerified = c.IsInfoVerified,

                        Remark = c.Remark,
                        Description = c.Description
                    }
                )
                .Take(10)
                .ToListAsync();

                // 🔹 COMPLETION %
                foreach (var item in records)
                {
                    item.CompletionPercentage =
                        CompletionCalculatorHelper.ContactPropCalculate(item);
                }

                double avgCompletion = records.Any()
                    ? Math.Round(records.Average(x => x.CompletionPercentage), 0)
                    : 0;

                return new PagedResponseDTO<GetContactResponseDTO>
                {
                    Items = records,
                    TotalCount = records.Count,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 1,
                    CompletionPercentage = avgCompletion,
                    HasUploadedAll = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error fetching contacts | EmployeeId: {EmployeeId}",
                    dto?.Prop?.EmployeeId);

                throw;
            }
        }


        public async Task<EmployeeContact>c(long id, bool isActive)
        {
            try
            {
                // 🧩 Validation
                if (id <= 0)
                    throw new ArgumentException("Invalid Id provided for fetching Contact record.");

                // 🔍 Fetch single record safely
                var record = await _context.EmployeeContacts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id
                                           && x.IsActive == isActive
                                           && x.IsSoftDeleted != true);

                // 🚫 Handle null result
                if (record == null)
                {
                    _logger.LogWarning("⚠️ No Contact record found for Id: {Id} (IsActive: {IsActive})", id, isActive);
                    throw new InvalidOperationException($"No Contact record found for Id: {id} (IsActive: {isActive})");
                }

                // ✅ Return valid record
                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching single Contact record for Id: {Id}", id);
                throw new Exception($"Failed to fetch single Contact record: {ex.Message}");
            }
        }


      
        public Task<PagedResponseDTO<GetContactResponseDTO>> AutoCreatedAsync(EmployeeContact entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(EmployeeContact employeeContact)
        {
            try
            {


                int affectedRows = await _context.SaveChangesAsync();

                if (affectedRows > 0)
                {
                    _logger.LogInformation("✔ Contact record updated successfully");

                    return true;
                }

                // 🚧 No Row Updated means something unexpected (maybe no actual new values)
                _logger.LogWarning("⚠ No changes detected | ContactId: {Id}", employeeContact.Id);
                return false;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "❌ Concurrency conflict while updating Contact record | Id: {Id}", employeeContact.Id);
                throw new Exception("Record update failed due to concurrency conflict. Please retry.");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "❌ Database error during update | Id: {Id}", employeeContact.Id);
                throw new Exception("Database error while updating Contact record.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating Contact record | Id: {Id}", employeeContact.Id);
                throw new Exception($"Unexpected update failure: {ex.Message}");
            }
        }

        public async Task<bool> UpdateContactAsync(EmployeeContact employeeContact)
        {
            try
            {
                // 🔥 SINGLE PRIMARY RULE (Repository level safety)
                if (employeeContact.IsPrimary == true)
                {
                    var existingPrimaries = await _context.EmployeeContacts
                        .Where(x =>
                            x.EmployeeId == employeeContact.EmployeeId &&
                            x.IsPrimary == true &&
                            x.Id != employeeContact.Id &&
                            x.IsSoftDeleted != true)
                        .ToListAsync();

                    if (existingPrimaries.Any())
                    {
                        foreach (var item in existingPrimaries)
                        {
                            item.IsPrimary = false;
                            item.UpdatedById = employeeContact.UpdatedById;
                            item.UpdatedDateTime = DateTime.UtcNow;
                        }
                    }
                }

                // ✅ Update current contact
                _context.EmployeeContacts.Update(employeeContact);

                int affectedRows = await _context.SaveChangesAsync();

                if (affectedRows > 0)
                {
                    _logger.LogInformation(
                        "✔ Contact updated successfully | ContactId: {Id} | EmployeeId: {EmployeeId}",
                        employeeContact.Id,
                        employeeContact.EmployeeId);

                    return true;
                }

                _logger.LogWarning(
                    "⚠ No changes detected | ContactId: {Id}",
                    employeeContact.Id);

                return false;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Concurrency conflict while updating Contact | Id: {Id}",
                    employeeContact.Id);

                throw new Exception(
                    "Record update failed due to concurrency conflict. Please retry.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Database error while updating Contact | Id: {Id}",
                    employeeContact.Id);

                throw new Exception(
                    "Database error while updating Contact record.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Unexpected error while updating Contact | Id: {Id}",
                    employeeContact.Id);

                throw;
            }
        }

        public async Task<EmployeeContact?> GetPrimaryLocationAsync( long employeeId,  bool isActive, bool track = true )
        {
            {
                IQueryable<EmployeeContact> query = _context.EmployeeContacts.Where(x => x.IsSoftDeleted != true && x.EmployeeId == employeeId);

                if (!track)
                    query = query.AsNoTracking();

                return await query.FirstOrDefaultAsync(x => x.IsActive == isActive && x.IsPrimary== true);
            }

        }

        public async Task<EmployeeContact?> GetSingleRecordAsync(long id, bool track = true)
        {
            IQueryable<EmployeeContact> query =
                _context.EmployeeContacts
                    .Where(x =>
                        x.Id == id &&
                        (x.IsSoftDeleted == false || x.IsSoftDeleted == null));

            if (!track)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }



    }
}
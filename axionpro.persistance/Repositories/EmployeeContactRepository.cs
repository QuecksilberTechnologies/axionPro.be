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
                        Id = c.Id.ToString(),
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
      
        public async Task<PagedResponseDTO<GetContactResponseDTO>> GetInfo(GetContactRequestDTO dto, long EmployeeId, int id)
        {
            try
            {
                // 🔹 Pagination & Sorting defaults
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;
                string sortBy = dto.SortBy?.ToLower() ?? "id";
                string sortOrder = dto.SortOrder?.ToLower() ?? "desc";
                bool isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

                // 🔹 Base query
                var baseQuery = _context.EmployeeContacts
                    .AsNoTracking()
                    .Where(contact =>
                        contact.EmployeeId == EmployeeId &&
                        (dto.IsActive == null || contact.IsActive == dto.IsActive) &&
                        contact.IsSoftDeleted != true);

                // 🔹 Optional filters
                int countryId = !string.IsNullOrWhiteSpace(dto.CountryId) ? SafeParser.TryParseInt(dto.CountryId) : 0;
                int stateId = !string.IsNullOrWhiteSpace(dto.StateId) ? SafeParser.TryParseInt(dto.StateId) : 0;
                int districtId = !string.IsNullOrWhiteSpace(dto.DistrictId) ? SafeParser.TryParseInt(dto.DistrictId) : 0;

                if (id > 0)
                    baseQuery = baseQuery.Where(x => x.Id == id);
                if (countryId > 0)
                    baseQuery = baseQuery.Where(x => x.CountryId == countryId);
                if (stateId > 0)
                    baseQuery = baseQuery.Where(x => x.StateId == stateId);
                if (districtId > 0)
                    baseQuery = baseQuery.Where(x => x.DistrictId == districtId);
                if (dto.IsPrimary.HasValue)
                    baseQuery = baseQuery.Where(x => x.IsPrimary == dto.IsPrimary);

                // 🔹 Sorting
                baseQuery = sortBy switch
                {
                    "countryid" => isDescending ? baseQuery.OrderByDescending(x => x.CountryId) : baseQuery.OrderBy(x => x.CountryId),
                    "stateid" => isDescending ? baseQuery.OrderByDescending(x => x.StateId) : baseQuery.OrderBy(x => x.StateId),
                    "districtid" => isDescending ? baseQuery.OrderByDescending(x => x.DistrictId) : baseQuery.OrderBy(x => x.DistrictId),
                    "contactnumber" => isDescending ? baseQuery.OrderByDescending(x => x.ContactNumber) : baseQuery.OrderBy(x => x.ContactNumber),
                    _ => isDescending ? baseQuery.OrderByDescending(x => x.Id) : baseQuery.OrderBy(x => x.Id)
                };

                // 🔹 Total records before pagination
                var totalRecords = await baseQuery.CountAsync();

                // ⭐ Check if employee has ANY primary contact
                bool hasPrimary = await baseQuery.AnyAsync(x => x.IsPrimary == true);

                // 🔹 Fetch Contacts (Before Mapping)
                var list = await baseQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 🔹 Extract Related IDs To Avoid DB Query Inside Loop
                var countryIds = list.Where(x => x.CountryId.HasValue).Select(x => x.CountryId.Value).Distinct().ToList();
                var stateIds = list.Where(x => x.StateId.HasValue).Select(x => x.StateId.Value).Distinct().ToList();
                var districtIds = list.Where(x => x.DistrictId.HasValue).Select(x => x.DistrictId.Value).Distinct().ToList();

                // 🔹 Get Names in Single DB Call (Not in loop)
                var countryMap = _context.Countries
                    .Where(c => countryIds.Contains(c.Id))
                    .ToDictionary(c => c.Id, c => c.CountryName);

                var stateMap = _context.States
                    .Where(s => stateIds.Contains(s.Id))
                    .ToDictionary(s => s.Id, s => s.StateName);

                var districtMap = _context.Districts
                    .Where(d => districtIds.Contains(d.Id))
                    .ToDictionary(d => d.Id, d => d.DistrictName);

                // 🔹 Mapping + Completion Score
                var data = list.Select(contact =>
                {
                    int primaryValue = hasPrimary ? (contact.IsPrimary == true ? 1 : 0) : 0;

                    double completion = (
                        (new[]
                        {
                  string.IsNullOrWhiteSpace(contact.Address) ? 0 : 1,
                   contact.CountryId.HasValue ? 1 : 0,
                  contact.StateId.HasValue ? 1 : 0,
                  contact.DistrictId.HasValue ? 1 : 0,
                  string.IsNullOrWhiteSpace(contact.LandMark) ? 0 : 1,
                  string.IsNullOrWhiteSpace(contact.Street) ? 0 : 1,
                  string.IsNullOrWhiteSpace(contact.HouseNo) ? 0 : 1,
                  string.IsNullOrWhiteSpace(contact.Email) ? 0 : 1,
                   primaryValue
                        }).Sum() / 9.0
                    ) * 100;

                    return new GetContactResponseDTO
                    {
                        Id = contact.Id.ToString(),
                        EmployeeId = contact.EmployeeId.ToString(),
                        Address = contact.Address,
                        LandMark = contact.LandMark,
                        HouseNo = contact.HouseNo,
                        
                        Street = contact.Street, 
                        ContactName  = contact.ContactName,
                        CountryId = contact.CountryId,
                        StateId = contact.StateId,
                        DistrictId = contact.DistrictId,
                        IsPrimary = contact.IsPrimary,
                        IsActive = contact.IsActive,
                        IsInfoVerified = contact.IsInfoVerified,
                        IsEditAllowed = contact.IsEditAllowed,
                        ContactType = contact.ContactType,
                        ContactNumber = contact.ContactNumber,
                        Relation = contact.Relation,
                        AlternateNumber = contact.AlternateNumber,
                        Email = contact.Email,
                        Remark = contact.Remark,
                        Description = contact.Description,
                        InfoVerifiedById = contact.InfoVerifiedById?.ToString(),
                        InfoVerifiedDateTime = contact.InfoVerifiedDateTime,

                        // 🔹 Get Names (From Cached Dictionaries)
                        CountryName = contact.CountryId.HasValue && countryMap.ContainsKey(contact.CountryId.Value) ? countryMap[contact.CountryId.Value] : null,
                        StateName = contact.StateId.HasValue && stateMap.ContainsKey(contact.StateId.Value) ? stateMap[contact.StateId.Value] : null,
                        DistrictName = contact.DistrictId.HasValue && districtMap.ContainsKey(contact.DistrictId.Value) ? districtMap[contact.DistrictId.Value] : null,

                        CompletionPercentage = completion
                    };
                })
                .DistinctBy(x => x.Id) // safety
                .ToList();

                // ⭐ Average completion for page
                double averagePercentage = data.Count > 0
                    ? data.Average(x => x.CompletionPercentage)
                    : 0;

                // 🔹 Return Response
                return new PagedResponseDTO<GetContactResponseDTO>
                {
                    Items = data,
                    TotalCount = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    HasUploadedAll = null,
                    CompletionPercentage = averagePercentage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching contacts for EmployeeId: {EmployeeId}", EmployeeId);
                throw new Exception($"Failed to fetch contacts: {ex.Message}");
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
            {
                IQueryable<EmployeeContact> query = _context.EmployeeContacts.Where(x => x.Id == id && x.IsSoftDeleted != true);

                if (!track)
                    query = query.AsNoTracking();

                return await query.FirstOrDefaultAsync(x => x.Id == id);
            }

        }


    }
}
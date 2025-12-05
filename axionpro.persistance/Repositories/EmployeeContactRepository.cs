using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
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
                    throw new ArgumentNullException(nameof(entity), "Contact info entity cannot be null.");

                if (entity.EmployeeId <= 0)
                    throw new ArgumentException("Invalid EmployeeId provided.");

                // ✅ 2️⃣ Record insert karo
                await _context.EmployeeContacts.AddAsync(entity);
                await _context.SaveChangesAsync();

                // ✅ 3️⃣ Fetch updated list (latest record ke sath)
                var query = _context.EmployeeContacts
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == entity.EmployeeId && x.IsSoftDeleted != true)
                    .OrderByDescending(x => x.Id);

                var totalRecords = await query.CountAsync();

                // ✅ 4️⃣ Fetch paginated data (default 1 page only since just added)
                var records = await query
                    .Take(10)
                    .ToListAsync();

                // ✅ 5️⃣ Map to DTOs
                var responseData = _mapper.Map<List<GetContactResponseDTO>>(records);

                // ✅ 6️⃣ Prepare PagedResponse
                return new PagedResponseDTO<GetContactResponseDTO>
                {
                    Items = responseData,
                    TotalCount = totalRecords,
                    PageNumber = 1,
                    PageSize = 10,

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while adding/fetching Contact info for EmployeeId: {EmployeeId}", entity.EmployeeId);
                throw new Exception($"Failed to add or fetch Contact info: {ex.Message}");
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
        public async Task<float> GetContactCompletionPercentageAsync(long employeeId)
        {
            try
            {
                // Only needed fields → PROJECTION ONLY → ultra light query
                var record = await _context.EmployeeBankDetails
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == employeeId && (x.IsSoftDeleted != true))
                    .OrderByDescending(x => x.IsPrimaryAccount)  // primary ko first laao
                    .Select(x => new
                    {
                        x.BankName,
                        x.AccountNumber,
                        x.IFSCCode,
                        x.BranchName,
                        x.AccountType,
                        x.IsPrimaryAccount,
                        x.HasChequeDocUploaded
                    })
                    .FirstOrDefaultAsync();

                if (record == null)
                    return 0;

                // 🧮 Completion % logic (same as your existing)
                var completion = Math.Round(
                (
                    record.IsPrimaryAccount
                    ? (new[]
                    {
                string.IsNullOrEmpty(record.BankName) ? 0 : 1,
                string.IsNullOrEmpty(record.AccountNumber) ? 0 : 1,
                string.IsNullOrEmpty(record.IFSCCode) ? 0 : 1,
                string.IsNullOrEmpty(record.BranchName) ? 0 : 1,
                string.IsNullOrEmpty(record.AccountType) ? 0 : 1,
                record.HasChequeDocUploaded ? 1 : 0,
                1
                    }).Sum() / 7.0
                    : (new[]
                    {
                string.IsNullOrEmpty(record.BankName) ? 0 : 1,
                string.IsNullOrEmpty(record.AccountNumber) ? 0 : 1,
                string.IsNullOrEmpty(record.IFSCCode) ? 0 : 1,
                string.IsNullOrEmpty(record.BranchName) ? 0 : 1,
                string.IsNullOrEmpty(record.AccountType) ? 0 : 1,
                1
                    }).Sum() / 6.0
                ) * 100, 0);

                return (int)completion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating bank completion for EmployeeId {EmployeeId}", employeeId);
                return 0;
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

                        CountryId = contact.CountryId?.ToString(),
                        StateId = contact.StateId?.ToString(),
                        DistrictId = contact.DistrictId?.ToString(),
                        IsPrimary = contact.IsPrimary,
                        IsActive = contact.IsActive,
                        IsInfoVerified = contact.IsInfoVerified,
                        IsEditAllowed = contact.IsEditAllowed,
                        ContactType = contact.ContactType?.ToString(),
                        ContactNumber = contact.ContactNumber,
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
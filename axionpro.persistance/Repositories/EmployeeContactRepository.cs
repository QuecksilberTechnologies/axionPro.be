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

                // 🔹 Optional filters with SafeParser
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

                // 🔹 Fetch data to memory for client-side mapping
                var data = baseQuery
                    .AsEnumerable() // Client-side for null-safe conversions
                    .Select(contact => new GetContactResponseDTO
                    {
                        Id = contact.Id.ToString(),
                        EmployeeId = contact.EmployeeId.ToString(),

                        // Contact Info
                        LocalAddress = contact.LocalAddress,
                        LandMark = contact.LandMark,
                        CountryId = contact.CountryId.ToString(),
                        StateId = contact.StateId.ToString(),
                        DistrictId = contact.DistrictId.ToString(),
                        IsPrimary = contact.IsPrimary,
                        IsActive = contact.IsActive,
                        IsInfoVerified = contact.IsInfoVerified,
                        IsEditAllowed = contact.IsEditAllowed,
                        ContactType = contact.ContactType.HasValue ? contact.ContactType.Value.ToString() : null,
                        PermanentAddress = contact.PermanentAddress,
                        ContactNumber = contact.ContactNumber,
                        AlternateNumber = contact.AlternateNumber,
                        Email = contact.Email,
                        Remark = contact.Remark,
                        Description = contact.Description,

                        InfoVerifiedById = contact.InfoVerifiedById?.ToString(),
                        InfoVerifiedDateTime = contact.InfoVerifiedDateTime
                    })
                    .DistinctBy(x => x.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // 🔹 Return paged response
                return new PagedResponseDTO<GetContactResponseDTO>
                {
                    Items = data,
                    TotalCount = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
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

       
        public Task<bool> UpdateFieldAsync(long Id, string entity, string fieldName, object? fieldValue, long updatedById)
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive)
        {
            throw new NotImplementedException();
        }

       
    }
}
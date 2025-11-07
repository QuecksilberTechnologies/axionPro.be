using AutoMapper;
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

        public async Task<PagedResponseDTO<GetContactResponseDTO>> GetInfo(GetContactRequestDTO dto, long EmployeeId, long Id)
        {
            try
            {
                // 🧭 Base query with mandatory filters
                var baseQuery = _context.EmployeeContacts
                    .AsNoTracking()
                    .Where(contact =>
                        contact.EmployeeId == EmployeeId &&
                        (dto.IsActive == null || contact.IsActive == dto.IsActive) &&
                        contact.IsSoftDeleted != true);

                // 🗺️ Optional filters
                long id = 0;
                if (!string.IsNullOrWhiteSpace(dto.Id))
                {
                    long.TryParse(dto.Id, out id);
                }

                if (id > 0)
                    baseQuery = baseQuery.Where(x => x.Id == id);

                if (dto.CountryId.HasValue)
                    baseQuery = baseQuery.Where(x => x.CountryId == dto.CountryId);

                if (dto.StateId.HasValue)
                    baseQuery = baseQuery.Where(x => x.StateId == dto.StateId);

                if (dto.DistrictId.HasValue)
                    baseQuery = baseQuery.Where(x => x.DistrictId == dto.DistrictId);

                if (dto.IsPrimary.HasValue)
                    baseQuery = baseQuery.Where(x => x.IsPrimary == dto.IsPrimary);

                // 🔍 Search filter
                if (!string.IsNullOrEmpty(dto.SortBy))
                {
                    var keyword = dto.SortBy.Trim().ToLower();
                    baseQuery = baseQuery.Where(x =>
                        (x.LocalAddress != null && x.LocalAddress.ToLower().Contains(keyword)) ||
                        (x.LandMark != null && x.LandMark.ToLower().Contains(keyword)));
                }

                // 🔽 Sorting
                bool isDescending = string.Equals(dto.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
                if (!string.IsNullOrEmpty(dto.SortBy))
                {
                    baseQuery = dto.SortBy.ToLower() switch
                    {
                        "countryid" => isDescending
                            ? baseQuery.OrderByDescending(x => x.CountryId)
                            : baseQuery.OrderBy(x => x.CountryId),

                        "stateid" => isDescending
                            ? baseQuery.OrderByDescending(x => x.StateId)
                            : baseQuery.OrderBy(x => x.StateId),

                        "districtid" => isDescending
                            ? baseQuery.OrderByDescending(x => x.DistrictId)
                            : baseQuery.OrderBy(x => x.DistrictId),

                        _ => isDescending
                            ? baseQuery.OrderByDescending(x => x.Id)
                            : baseQuery.OrderBy(x => x.Id)
                    };

                }
                else
                {
                    baseQuery = baseQuery.OrderByDescending(x => x.Id);
                }

                // 📄 Total Count
                var totalRecords = await baseQuery.CountAsync();

                // 🧩 Join Query (Country, State, District)
                var query = from contact in baseQuery
                            join country in _context.Countries on contact.CountryId equals country.Id into countryGroup
                            from country in countryGroup.DefaultIfEmpty()
                            join state in _context.States on contact.StateId equals state.Id into stateGroup
                            from state in stateGroup.DefaultIfEmpty()
                            join district in _context.Districts on contact.DistrictId equals district.Id into districtGroup
                            from district in districtGroup.DefaultIfEmpty()
                            select new GetContactResponseDTO
                            {
                                // 🆔 Encrypted Ids
                                Id =  contact.Id.ToString(),
                                EmployeeId  =  contact.EmployeeId.ToString(),

                                // 📋 Contact Info
                                LocalAddress = contact.LocalAddress,
                                LandMark = contact.LandMark,
                                CountryId = contact.CountryId,
                                Country = country != null ? country.CountryName : null,
                                StateId = contact.StateId,
                                State = state != null ? state.StateName : null,
                                DistrictId = contact.DistrictId,
                                District = district != null ? district.DistrictName : null,
                                IsPrimary = contact.IsPrimary,
                                IsActive = contact.IsActive,
                                IsInfoVerified = contact.IsInfoVerified,
                                IsEditAllowed = contact.IsEditAllowed,

                                // 📞 Additional Info
                                ContactType = contact.ContactType,
                                PermanentAddress = contact.PermanentAddress,
                                ContactNumber = contact.ContactNumber,
                                AlternateNumber = contact.AlternateNumber,
                                Email = contact.Email,
                                Remark = contact.Remark,
                                Description = contact.Description,

                                // 🕒 Audit Fields
                                AddedById = contact.AddedById.ToString(),
                                AddedDateTime = contact.AddedDateTime,
                                UpdatedById = contact.UpdatedById.ToString(),
                                UpdatedDateTime = contact.UpdatedDateTime,
                                InfoVerifiedById = contact.InfoVerifiedById.ToString(),
                                InfoVerifiedDateTime = contact.InfoVerifiedDateTime
                            };

                // 🚫 Remove duplicates
                var distinctQuery = query.DistinctBy(x => x.Id);

                // 📜 Pagination
                var pagedRecords = await distinctQuery
                    .Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToListAsync();

                // 📦 Final Response
                return new PagedResponseDTO<GetContactResponseDTO>
                {
                    Items = pagedRecords ?? new List<GetContactResponseDTO>(),
                    TotalCount = totalRecords,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
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
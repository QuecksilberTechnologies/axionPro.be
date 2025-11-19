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
                bool hasPrimary = baseQuery.Any(x => x.IsPrimary == true);

                // 🔹 Fetch and map
                var data = baseQuery
                    .AsEnumerable()
                    .Select(contact =>
                    {
                        // ⭐ Primary Rule
                        int primaryValue = hasPrimary ? (contact.IsPrimary.HasValue ? 1 : 0) : 0;

                        // ⭐ Completion Calculation
                        double completion = (
                            (new[]
                            {
                        string.IsNullOrEmpty(contact.Address) ? 0 : 1,
                        string.IsNullOrEmpty(contact.CountryId?.ToString()) ? 0 : 1,
                        string.IsNullOrEmpty(contact.StateId?.ToString()) ? 0 : 1,
                        string.IsNullOrEmpty(contact.DistrictId?.ToString()) ? 0 : 1,
                        string.IsNullOrEmpty(contact.ContactNumber) ? 0 : 1,
                        string.IsNullOrEmpty(contact.Email) ? 0 : 1,
                        primaryValue
                            }).Sum() / 7.0
                        ) * 100;

                        return new GetContactResponseDTO
                        {
                            Id = contact.Id.ToString(),
                            EmployeeId = contact.EmployeeId.ToString(),
                            Address = contact.Address,
                            LandMark = contact.LandMark,
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

                            // ⭐ Final Completion
                            CompletionPercentage = completion
                        };
                    })
                    .DistinctBy(x => x.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
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